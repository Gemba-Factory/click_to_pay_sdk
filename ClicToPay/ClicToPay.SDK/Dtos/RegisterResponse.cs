using ClicToPay.SDK.Enums;
using System.Text.Json.Serialization;

namespace ClicToPay.SDK.Dtos
{
    public class BaseResponse
    {
        [JsonPropertyName("errorCode")]
        public int RawErrorCode { get; set; } // Conserve l'entier brut pour la désérialisation

        [JsonIgnore]
        public ClicToPayErrorCode ErrorCode => (ClicToPayErrorCode)RawErrorCode;

        [JsonPropertyName("errorMessage")]
        public string ErrorMessage { get; set; }

        public bool IsSuccess => RawErrorCode == (int)ClicToPayErrorCode.Success;
    }

    public class RegisterResponse : BaseResponse
    {
        [JsonPropertyName("orderId")]
        public string OrderId { get; set; }

        [JsonPropertyName("formUrl")]
        public string FormUrl { get; set; }
    }

    public class BindingInfo
    {
        [JsonPropertyName("clientId")]
        public string ClientId { get; set; }

        [JsonPropertyName("bindingId")]
        public string BindingId { get; set; }
    }

    public class OrderStatusResponse : BaseResponse
    {
        [JsonPropertyName("orderStatus")]
        public int? RawOrderStatus { get; set; } // Conserve l'entier brut

        [JsonIgnore]
        public ClicToPayOrderStatus? OrderStatus => RawOrderStatus.HasValue ? (ClicToPayOrderStatus)RawOrderStatus.Value : null;

        [JsonPropertyName("orderNumber")]
        public string OrderNumber { get; set; }

        [JsonPropertyName("pan")]
        public string Pan { get; set; }

        [JsonPropertyName("expiration")]
        public string Expiration { get; set; }

        [JsonPropertyName("cardholderName")]
        public string CardholderName { get; set; }

        [JsonPropertyName("amount")]
        public long? Amount { get; set; }

        [JsonPropertyName("currency")]
        public int? Currency { get; set; }

        [JsonPropertyName("approvalCode")]
        public string ApprovalCode { get; set; }

        [JsonPropertyName("authCode")]
        public string AuthCode { get; set; }

        [JsonPropertyName("ip")]
        public string Ip { get; set; }

        [JsonPropertyName("bindingInfo")]
        public BindingInfo BindingInfo { get; set; }
    }

    public class SecureAuthInfo
    {
        [JsonPropertyName("eci")]
        public int? Eci { get; set; }

        [JsonPropertyName("cavv")]
        public string Cavv { get; set; }

        [JsonPropertyName("xid")]
        public string Xid { get; set; }
    }

    public class CardAuthInfo
    {
        [JsonPropertyName("maskedPan")]
        public string MaskedPan { get; set; }

        [JsonPropertyName("expiration")]
        public string Expiration { get; set; }

        [JsonPropertyName("cardholderName")]
        public string CardholderName { get; set; }

        [JsonPropertyName("approvalCode")]
        public string ApprovalCode { get; set; }

        [JsonPropertyName("secureAuthInfo")]
        public SecureAuthInfo SecureAuthInfo { get; set; }
    }

    public class OrderStatusExtendedResponse : BaseResponse
    {
        [JsonPropertyName("orderNumber")]
        public string OrderNumber { get; set; }

        [JsonPropertyName("orderStatus")]
        public int? RawOrderStatus { get; set; } // Conserve l'entier brut

        [JsonIgnore]
        public ClicToPayOrderStatus? OrderStatus => RawOrderStatus.HasValue ? (ClicToPayOrderStatus)RawOrderStatus.Value : null;

        [JsonPropertyName("actionCode")]
        public int? ActionCode { get; set; }

        [JsonPropertyName("actionCodeDescription")]
        public string ActionCodeDescription { get; set; }

        [JsonPropertyName("amount")]
        public long? Amount { get; set; }

        [JsonPropertyName("currency")]
        public int? Currency { get; set; }

        [JsonPropertyName("date")]
        public long? Date { get; set; }

        [JsonPropertyName("orderDescription")]
        public string OrderDescription { get; set; }

        [JsonPropertyName("ip")]
        public string Ip { get; set; }

        [JsonPropertyName("cardAuthInfo")]
        public CardAuthInfo CardAuthInfo { get; set; }
    }
}
