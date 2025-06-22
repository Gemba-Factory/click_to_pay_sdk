using System.Text.Json.Serialization;

namespace ClicToPay.SDK.Dtos
{
    public class RegisterRequest
    {
        [JsonPropertyName("userName")]
        public string UserName { get; set; }

        [JsonPropertyName("password")]
        public string Password { get; set; }

        [JsonPropertyName("orderNumber")]
        public string OrderNumber { get; set; }

        [JsonPropertyName("amount")]
        public long Amount { get; set; }

        [JsonPropertyName("currency")]
        public int Currency { get; set; }

        [JsonPropertyName("returnUrl")]
        public string ReturnUrl { get; set; }

        [JsonPropertyName("failUrl")]
        public string FailUrl { get; set; } // Optionnel

        [JsonPropertyName("description")]
        public string Description { get; set; } // Optionnel

        [JsonPropertyName("language")]
        public string Language { get; set; } = "fr"; // Optionnel, par défaut "fr"

        [JsonPropertyName("pageView")]
        public string PageView { get; set; } = "DESKTOP"; // Optionnel, par défaut "DESKTOP"

        [JsonPropertyName("clientId")]
        public string ClientId { get; set; } // Optionnel

        [JsonPropertyName("jsonParams")]
        public Dictionary<string, string> JsonParams { get; set; } // Optionnel, sera sérialisé en JSON string

        [JsonPropertyName("sessionTimeoutSecs")]
        public int? SessionTimeoutSecs { get; set; } // Optionnel

        [JsonPropertyName("expirationDate")]
        public string ExpirationDate { get; set; } // Optionnel, format YYYYMMDD'T'HH:mm:ss

        [JsonPropertyName("bindingId")]
        public string BindingId { get; set; } // Optionnel
    }

    // PreAuthRequest utilise les mêmes paramètres que RegisterRequest
    public class PreAuthRequest : RegisterRequest { }

    public class ConfirmRequest
    {
        [JsonPropertyName("userName")]
        public string UserName { get; set; }

        [JsonPropertyName("password")]
        public string Password { get; set; }

        [JsonPropertyName("orderId")]
        public string OrderId { get; set; }

        [JsonPropertyName("amount")]
        public long? Amount { get; set; } // Optionnel, si non spécifié, confirme le montant total pré-autorisé
    }

    public class CancelRequest
    {
        [JsonPropertyName("userName")]
        public string UserName { get; set; }

        [JsonPropertyName("password")]
        public string Password { get; set; }

        [JsonPropertyName("orderId")]
        public string OrderId { get; set; }

        [JsonPropertyName("language")]
        public string Language { get; set; } = "fr"; // Optionnel, par défaut "fr"
    }

    public class RefundRequest
    {
        [JsonPropertyName("userName")]
        public string UserName { get; set; }

        [JsonPropertyName("password")]
        public string Password { get; set; }

        [JsonPropertyName("orderId")]
        public string OrderId { get; set; }

        [JsonPropertyName("amount")]
        public long Amount { get; set; }
    }

    public class StatusRequest
    {
        [JsonPropertyName("userName")]
        public string UserName { get; set; }

        [JsonPropertyName("password")]
        public string Password { get; set; }

        [JsonPropertyName("orderId")]
        public string OrderId { get; set; }

        [JsonPropertyName("language")]
        public string Language { get; set; } = "fr"; // Optionnel, par défaut "fr"
    }

    public class StatusExtendedRequest
    {
        [JsonPropertyName("userName")]
        public string UserName { get; set; }

        [JsonPropertyName("password")]
        public string Password { get; set; }

        [JsonPropertyName("orderNumber")]
        public string OrderNumber { get; set; }

        [JsonPropertyName("language")]
        public string Language { get; set; } = "fr"; 
    }
}
