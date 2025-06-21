using ClicToPay.SDK.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ClicToPay.SDK
{
    public class ClicToPayClient : IClicToPayClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public ClicToPayClient(HttpClient httpClient, string baseUrl)
        {
            _httpClient = httpClient;
            _baseUrl = baseUrl.TrimEnd('/');
        }

        //public Task<BaseResponse> CancelPaymentAsync(CancelRequest request)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<BaseResponse> ConfirmPreAuthAsync(ConfirmRequest request)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<OrderStatusResponse> GetOrderStatusAsync(StatusRequest request)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<OrderStatusExtendedResponse> GetOrderStatusExtendedAsync(StatusExtendedRequest request)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<BaseResponse> RefundPaymentAsync(RefundRequest request)
        //{
        //    throw new NotImplementedException();
        //}

        public async Task<RegisterResponse> RegisterPaymentAsync(RegisterRequest request)
        {
            var queryParams = new Dictionary<string, string>
        {
            { "userName", request.UserName },
            { "password", request.Password },
            { "orderNumber", request.OrderNumber },
            { "amount", request.Amount.ToString() },
            { "currency", request.Currency.ToString() },
            { "returnUrl", request.ReturnUrl },
            { "failUrl", request.FailUrl ?? "" },
            { "language", request.Language },
            { "pageView", request.PageView }
        };

            if (request.JsonParams != null)
            {
                queryParams["jsonParams"] = JsonSerializer.Serialize(request.JsonParams);
            }

            if (!string.IsNullOrEmpty(request.ExpirationDate))
            {
                queryParams["expirationDate"] = request.ExpirationDate;
            }

            var response = await _httpClient.GetAsync($"{_baseUrl}/payment/rest/register.do?{ToQueryString(queryParams)}");
            var content = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<RegisterResponse>(content);
        }

        //public Task<RegisterResponse> RegisterPreAuthAsync(PreAuthRequest request)
        //{
        //    throw new NotImplementedException();
        //}

        private static string ToQueryString(Dictionary<string, string> dict)
        {
            return string.Join("&", dict
                .Where(kvp => !string.IsNullOrWhiteSpace(kvp.Value))
                .Select(kvp => $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}"));
        }
    }
}
