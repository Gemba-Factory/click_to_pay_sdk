using ClicToPay.SDK.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClicToPay.SDK
{
    public interface IClicToPayClient
    {
        Task<RegisterResponse> RegisterPaymentAsync(RegisterRequest request);
        Task<RegisterResponse> RegisterPreAuthAsync(PreAuthRequest request);
        Task<BaseResponse> ConfirmPreAuthAsync(ConfirmRequest request);
        Task<BaseResponse> CancelPaymentAsync(CancelRequest request);
        Task<BaseResponse> RefundPaymentAsync(RefundRequest request);
        Task<OrderStatusResponse> GetOrderStatusAsync(StatusRequest request);
        Task<OrderStatusExtendedResponse> GetOrderStatusExtendedAsync(StatusExtendedRequest request);
    }
}
