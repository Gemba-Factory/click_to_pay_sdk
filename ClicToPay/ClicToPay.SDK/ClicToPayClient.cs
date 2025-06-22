using ClicToPay.SDK.Dtos;
using ClicToPay.SDK.Options;
using System.Text.Json;

namespace ClicToPay.SDK
{
    public class ClicToPayClient : IClicToPayClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private readonly ClicToPayOptions _options;

        /// <summary>
        /// Initialise une nouvelle instance du client ClicToPay.
        /// </summary>
        /// <param name="httpClient">L'instance HttpClient à utiliser pour les requêtes HTTP.</param>
        /// <param name="options">Les options de configuration pour le client ClicToPay (nom d'utilisateur, mot de passe, environnement ).</param>
        /// <exception cref="ArgumentNullException">Lancée si httpClient ou options est nul.</exception>
        public ClicToPayClient(HttpClient httpClient, ClicToPayOptions options)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _baseUrl = options.GetBaseUrl();
        }

        /// <summary>
        /// Enregistre une demande de paiement (Autorisation).
        /// Endpoint: /payment/rest/register.do
        /// </summary>
        /// <param name="request">Les paramètres de la demande de paiement.</param>
        /// <returns>La réponse d'enregistrement du paiement.</returns>
        /// <exception cref="ArgumentNullException">Lancée si un paramètre requis est nul.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Lancée si un montant ou une devise est invalide.</exception>
        /// <exception cref="HttpRequestException">Lancée si la requête HTTP échoue au niveau du transport (ex: erreur réseau, 4xx/5xx HTTP).</exception>
        /// <exception cref="ClicToPayApiException">Lancée si l'API retourne une erreur métier (errorCode != 0).</exception>
        public async Task<RegisterResponse> RegisterPaymentAsync(RegisterRequest request)
        {
            // Validation des paramètres requis de la requête
            if (request == null) throw new ArgumentNullException(nameof(request));
            if (string.IsNullOrEmpty(request.OrderNumber)) throw new ArgumentNullException(nameof(request.OrderNumber), "OrderNumber est requis.");
            if (request.Amount <= 0) throw new ArgumentOutOfRangeException(nameof(request.Amount), "Amount doit être supérieur à 0.");
            if (request.Currency <= 0) throw new ArgumentOutOfRangeException(nameof(request.Currency), "Currency doit être spécifié.");
            if (string.IsNullOrEmpty(request.ReturnUrl)) throw new ArgumentNullException(nameof(request.ReturnUrl), "ReturnUrl est requis.");

            var queryParams = new Dictionary<string, string>
            {
                { "userName", _options.Username },
                { "password", _options.Password },
                { "orderNumber", request.OrderNumber },
                { "amount", request.Amount.ToString() },
                { "currency", request.Currency.ToString() },
                { "returnUrl", request.ReturnUrl },
                // Les paramètres optionnels sont ajoutés seulement s'ils ont une valeur
                { "failUrl", request.FailUrl },
                { "description", request.Description },
                { "language", request.Language },
                { "pageView", request.PageView },
                { "clientId", request.ClientId },
                { "expirationDate", request.ExpirationDate },
                { "bindingId", request.BindingId }
            };

            if (request.JsonParams != null && request.JsonParams.Any())
            {
                queryParams["jsonParams"] = JsonSerializer.Serialize(request.JsonParams);
            }

            if (request.SessionTimeoutSecs.HasValue)
            {
                queryParams["sessionTimeoutSecs"] = request.SessionTimeoutSecs.Value.ToString();
            }

            var response = await _httpClient.GetAsync($"{_baseUrl}/payment/rest/register.do?{ToQueryString(queryParams)}");
            response.EnsureSuccessStatusCode(); // Lance HttpRequestException pour les codes HTTP 4xx/5xx
            var content = await response.Content.ReadAsStringAsync();

            var apiResponse = JsonSerializer.Deserialize<RegisterResponse>(content);
            return ClicToPayResponseHandler.HandleResponse(apiResponse); // Gère les erreurs métier de l'API
        }

        /// <summary>
        /// Enregistre une demande de pré-autorisation.
        /// Endpoint: /payment/rest/registerPreAuth.do
        /// </summary>
        /// <param name="request">Les paramètres de la demande de pré-autorisation.</param>
        /// <returns>La réponse d'enregistrement de la pré-autorisation.</returns>
        /// <exception cref="ArgumentNullException">Lancée si un paramètre requis est nul.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Lancée si un montant ou une devise est invalide.</exception>
        /// <exception cref="HttpRequestException">Lancée si la requête HTTP échoue au niveau du transport.</exception>
        /// <exception cref="ClicToPayApiException">Lancée si l'API retourne une erreur métier.</exception>
        public async Task<RegisterResponse> RegisterPreAuthAsync(PreAuthRequest request)
        {
            // Validation des paramètres requis (identique à RegisterPaymentAsync)
            if (request == null) throw new ArgumentNullException(nameof(request));
            if (string.IsNullOrEmpty(request.OrderNumber)) throw new ArgumentNullException(nameof(request.OrderNumber), "OrderNumber est requis.");
            if (request.Amount <= 0) throw new ArgumentOutOfRangeException(nameof(request.Amount), "Amount doit être supérieur à 0.");
            if (request.Currency <= 0) throw new ArgumentOutOfRangeException(nameof(request.Currency), "Currency doit être spécifié.");
            if (string.IsNullOrEmpty(request.ReturnUrl)) throw new ArgumentNullException(nameof(request.ReturnUrl), "ReturnUrl est requis.");

            var queryParams = new Dictionary<string, string>
            {
                { "userName", _options.Username },
                { "password", _options.Password },
                { "orderNumber", request.OrderNumber },
                { "amount", request.Amount.ToString() },
                { "currency", request.Currency.ToString() },
                { "returnUrl", request.ReturnUrl },
                // Les paramètres optionnels sont ajoutés seulement s'ils ont une valeur
                { "failUrl", request.FailUrl },
                { "description", request.Description },
                { "language", request.Language },
                { "pageView", request.PageView },
                { "clientId", request.ClientId },
                { "expirationDate", request.ExpirationDate },
                { "bindingId", request.BindingId }
            };

            if (request.JsonParams != null && request.JsonParams.Any())
            {
                queryParams["jsonParams"] = JsonSerializer.Serialize(request.JsonParams);
            }

            if (request.SessionTimeoutSecs.HasValue)
            {
                queryParams["sessionTimeoutSecs"] = request.SessionTimeoutSecs.Value.ToString();
            }

            var response = await _httpClient.GetAsync($"{_baseUrl}/payment/rest/registerPreAuth.do?{ToQueryString(queryParams)}");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();

            var apiResponse = JsonSerializer.Deserialize<RegisterResponse>(content);
            return ClicToPayResponseHandler.HandleResponse(apiResponse);
        }

        /// <summary>
        /// Confirme une pré-autorisation.
        /// Endpoint: /payment/rest/deposit.do
        /// </summary>
        /// <param name="request">Les paramètres de la demande de confirmation.</param>
        /// <returns>La réponse de la confirmation.</returns>
        /// <exception cref="ArgumentNullException">Lancée si un paramètre requis est nul.</exception>
        /// <exception cref="HttpRequestException">Lancée si la requête HTTP échoue au niveau du transport.</exception>
        /// <exception cref="ClicToPayApiException">Lancée si l'API retourne une erreur métier.</exception>
        public async Task<BaseResponse> ConfirmPreAuthAsync(ConfirmRequest request)
        {
            // Validation des paramètres requis
            if (request == null) throw new ArgumentNullException(nameof(request));
            if (string.IsNullOrEmpty(request.OrderId)) throw new ArgumentNullException(nameof(request.OrderId), "OrderId est requis.");

            var queryParams = new Dictionary<string, string>
            {
                { "userName", _options.Username },
                { "password", _options.Password },
                { "orderId", request.OrderId }
            };

            if (request.Amount.HasValue)
            {
                queryParams["amount"] = request.Amount.Value.ToString();
            }

            var response = await _httpClient.GetAsync($"{_baseUrl}/payment/rest/deposit.do?{ToQueryString(queryParams)}");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();

            var apiResponse = JsonSerializer.Deserialize<BaseResponse>(content);
            return ClicToPayResponseHandler.HandleResponse(apiResponse);
        }

        /// <summary>
        /// Annule un paiement.
        /// Endpoint: /payment/rest/reverse.do
        /// </summary>
        /// <param name="request">Les paramètres de la demande d'annulation.</param>
        /// <returns>La réponse de l'annulation.</returns>
        /// <exception cref="ArgumentNullException">Lancée si un paramètre requis est nul.</exception>
        /// <exception cref="HttpRequestException">Lancée si la requête HTTP échoue au niveau du transport.</exception>
        /// <exception cref="ClicToPayApiException">Lancée si l'API retourne une erreur métier.</exception>
        public async Task<BaseResponse> CancelPaymentAsync(CancelRequest request)
        {
            // Validation des paramètres requis
            if (request == null) throw new ArgumentNullException(nameof(request));
            if (string.IsNullOrEmpty(request.OrderId)) throw new ArgumentNullException(nameof(request.OrderId), "OrderId est requis.");

            var queryParams = new Dictionary<string, string>
            {
                { "userName", _options.Username },
                { "password", _options.Password },
                { "orderId", request.OrderId },
                { "language", request.Language } // Optionnel, mais inclus par défaut
            };

            var response = await _httpClient.GetAsync($"{_baseUrl}/payment/rest/reverse.do?{ToQueryString(queryParams)}");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();

            var apiResponse = JsonSerializer.Deserialize<BaseResponse>(content);
            return ClicToPayResponseHandler.HandleResponse(apiResponse);
        }

        /// <summary>
        /// Rembourse un paiement.
        /// Endpoint: /payment/rest/refund.do
        /// </summary>
        /// <param name="request">Les paramètres de la demande de remboursement.</param>
        /// <returns>La réponse du remboursement.</returns>
        /// <exception cref="ArgumentNullException">Lancée si un paramètre requis est nul.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Lancée si un montant est invalide.</exception>
        /// <exception cref="HttpRequestException">Lancée si la requête HTTP échoue au niveau du transport.</exception>
        /// <exception cref="ClicToPayApiException">Lancée si l'API retourne une erreur métier.</exception>
        public async Task<BaseResponse> RefundPaymentAsync(RefundRequest request)
        {
            // Validation des paramètres requis
            if (request == null) throw new ArgumentNullException(nameof(request));
            if (string.IsNullOrEmpty(request.OrderId)) throw new ArgumentNullException(nameof(request.OrderId), "OrderId est requis.");
            if (request.Amount <= 0) throw new ArgumentOutOfRangeException(nameof(request.Amount), "Amount doit être supérieur à 0.");

            var queryParams = new Dictionary<string, string>
            {
                { "userName", _options.Username },
                { "password", _options.Password },
                { "orderId", request.OrderId },
                { "amount", request.Amount.ToString() }
            };

            var response = await _httpClient.GetAsync($"{_baseUrl}/payment/rest/refund.do?{ToQueryString(queryParams)}");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();

            var apiResponse = JsonSerializer.Deserialize<BaseResponse>(content);
            return ClicToPayResponseHandler.HandleResponse(apiResponse);
        }

        /// <summary>
        /// Récupère le statut d'une commande.
        /// Endpoint: /payment/rest/getOrderStatus.do
        /// </summary>
        /// <param name="request">Les paramètres de la demande de statut.</param>
        /// <returns>La réponse du statut de la commande.</returns>
        /// <exception cref="ArgumentNullException">Lancée si un paramètre requis est nul.</exception>
        /// <exception cref="HttpRequestException">Lancée si la requête HTTP échoue au niveau du transport.</exception>
        /// <exception cref="ClicToPayApiException">Lancée si l'API retourne une erreur métier.</exception>
        public async Task<OrderStatusResponse> GetOrderStatusAsync(StatusRequest request)
        {
            // Validation des paramètres requis
            if (request == null) throw new ArgumentNullException(nameof(request));
            if (string.IsNullOrEmpty(request.OrderId)) throw new ArgumentNullException(nameof(request.OrderId), "OrderId est requis.");

            var queryParams = new Dictionary<string, string>
            {
                { "userName", _options.Username },
                { "password", _options.Password },
                { "orderId", request.OrderId },
                { "language", request.Language } // Optionnel, mais inclus par défaut
            };

            var response = await _httpClient.GetAsync($"{_baseUrl}/payment/rest/getOrderStatus.do?{ToQueryString(queryParams)}");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();

            var apiResponse = JsonSerializer.Deserialize<OrderStatusResponse>(content);
            return ClicToPayResponseHandler.HandleResponse(apiResponse);
        }

        /// <summary>
        /// Récupère le statut étendu d'une commande.
        /// Endpoint: /payment/rest/getOrderStatusExtended.do
        /// </summary>
        /// <param name="request">Les paramètres de la demande de statut étendu.</param>
        /// <returns>La réponse du statut étendu de la commande.</returns>
        /// <exception cref="ArgumentNullException">Lancée si un paramètre requis est nul.</exception>
        /// <exception cref="HttpRequestException">Lancée si la requête HTTP échoue au niveau du transport.</exception>
        /// <exception cref="ClicToPayApiException">Lancée si l'API retourne une erreur métier.</exception>
        public async Task<OrderStatusExtendedResponse> GetOrderStatusExtendedAsync(StatusExtendedRequest request)
        {
            // Validation des paramètres requis
            if (request == null) throw new ArgumentNullException(nameof(request));
            if (string.IsNullOrEmpty(request.OrderNumber)) throw new ArgumentNullException(nameof(request.OrderNumber), "OrderNumber est requis.");

            var queryParams = new Dictionary<string, string>
            {
                { "userName", _options.Username },
                { "password", _options.Password },
                { "orderNumber", request.OrderNumber },
                { "language", request.Language } // Optionnel, mais inclus par défaut
            };

            var response = await _httpClient.GetAsync($"{_baseUrl}/payment/rest/getOrderStatusExtended.do?{ToQueryString(queryParams)}");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();

            var apiResponse = JsonSerializer.Deserialize<OrderStatusExtendedResponse>(content);
            return ClicToPayResponseHandler.HandleResponse(apiResponse);
        }

        /// <summary>
        /// Convertit un dictionnaire de paires clé-valeur en une chaîne de requête URL.
        /// Les valeurs nulles ou vides sont ignorées.
        /// </summary>
        /// <param name="dict">Le dictionnaire de paramètres.</param>
        /// <returns>La chaîne de requête URL.</returns>
        private static string ToQueryString(Dictionary<string, string> dict)
        {
            return string.Join("&", dict
                .Where(kvp => !string.IsNullOrEmpty(kvp.Value)) // Utilise String.IsNullOrEmpty pour les chaînes
                .Select(kvp => $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}"));
        }
    }
}
