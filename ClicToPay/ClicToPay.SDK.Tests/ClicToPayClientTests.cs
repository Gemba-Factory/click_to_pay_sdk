using ClicToPay.SDK.Dtos;
using ClicToPay.SDK.Options;
using ClicToPay.SDK.Enums;
using ClicToPay.SDK.Exceptions;
using Moq;
using Moq.Protected;
using System.Net;
using Xunit;

namespace ClicToPay.SDK.Tests
{
    public class ClicToPayClientTests
    {
        private ClicToPayOptions _options;
        private Mock<HttpMessageHandler> _handlerMock;
        private HttpClient _httpClient;
        private ClicToPayClient _client;

        public ClicToPayClientTests()
        {
            _options = new ClicToPayOptions
            {
                Username = "testuser",
                Password = "testpass",
                UseSandbox = true
            };
            _handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            _httpClient = new HttpClient(_handlerMock.Object);
            _client = new ClicToPayClient(_httpClient, _options);
        }

        private void SetupHttpResponse(string urlContains, string responseContent, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            _handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => req.RequestUri.ToString().Contains(urlContains)),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = statusCode,
                    Content = new StringContent(responseContent)
                });
        }

        // Helper for error responses
        private string ErrorResponse(int errorCode, string errorMessage)
        {
            return System.Text.Json.JsonSerializer.Serialize(new RegisterResponse
            {
                RawErrorCode = errorCode,
                ErrorMessage = errorMessage
            });
        }

        [Fact]
        public async Task RegisterPaymentAsync_NullRequest_Throws()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _client.RegisterPaymentAsync(null));
        }

        [Fact]
        public async Task RegisterPaymentAsync_InvalidArguments_Throws()
        {
            var req = new RegisterRequest();
            await Assert.ThrowsAsync<ArgumentNullException>(() => _client.RegisterPaymentAsync(req));
            req.OrderNumber = "ord";
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => _client.RegisterPaymentAsync(req));
            req.Amount = 1;
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => _client.RegisterPaymentAsync(req));
            req.Currency = 1;
            await Assert.ThrowsAsync<ArgumentNullException>(() => _client.RegisterPaymentAsync(req));
        }

        [Fact]
        public async Task RegisterPaymentAsync_ValidRequest_ReturnsResponse()
        {
            var req = new RegisterRequest
            {
                OrderNumber = "ord",
                Amount = 100,
                Currency = 978,
                ReturnUrl = "https://return.url"
            };
            var expected = new RegisterResponse { OrderId = "123", FormUrl = "url" };
            SetupHttpResponse("register.do", System.Text.Json.JsonSerializer.Serialize(expected));
            var result = await _client.RegisterPaymentAsync(req);
            Assert.Equal(expected.OrderId, result.OrderId);
            Assert.Equal(expected.FormUrl, result.FormUrl);
        }

        [Theory]
        [InlineData((int)ClicToPayErrorCode.DuplicateOrder, typeof(ClicToPayDuplicateOrderException))]
        [InlineData((int)ClicToPayErrorCode.PaymentCredentialsError, typeof(ClicToPayPaymentCredentialsException))]
        [InlineData((int)ClicToPayErrorCode.UnknownCurrency, typeof(ClicToPayUnknownCurrencyException))]
        [InlineData((int)ClicToPayErrorCode.RequiredParameterNotSpecified, typeof(ClicToPayInvalidParameterException))]
        [InlineData((int)ClicToPayErrorCode.IncorrectParameterValue, typeof(ClicToPayInvalidParameterException))]
        [InlineData((int)ClicToPayErrorCode.OrderNotFound, typeof(ClicToPayOrderNotFoundException))]
        [InlineData((int)ClicToPayErrorCode.SystemError, typeof(ClicToPaySystemErrorException))]
        [InlineData(99, typeof(ClicToPayApiException))]
        public async Task RegisterPaymentAsync_ErrorCodes_ThrowsCorrectException(int errorCode, Type expectedException)
        {
            var req = new RegisterRequest
            {
                OrderNumber = "ord",
                Amount = 100,
                Currency = 978,
                ReturnUrl = "https://return.url"
            };
            SetupHttpResponse("register.do", ErrorResponse(errorCode, "error message"));
            var ex = await Record.ExceptionAsync(() => _client.RegisterPaymentAsync(req));
            Assert.NotNull(ex);
            Assert.IsType(expectedException, ex);
        }

        [Fact]
        public async Task RegisterPaymentAsync_AccessDenied_ThrowsAccessDeniedException()
        {
            var req = new RegisterRequest
            {
                OrderNumber = "ord",
                Amount = 100,
                Currency = 978,
                ReturnUrl = "https://return.url"
            };
            // errorCode 5 with 'Accès refusé' triggers ClicToPayAccessDeniedException
            SetupHttpResponse("register.do", ErrorResponse((int)ClicToPayErrorCode.IncorrectParameterValue, "Accès refusé: credentials invalid"));
            var ex = await Record.ExceptionAsync(() => _client.RegisterPaymentAsync(req));
            Assert.NotNull(ex);
            Assert.IsType<ClicToPayAccessDeniedException>(ex);
        }

        [Fact]
        public async Task RegisterPreAuthAsync_ValidRequest_ReturnsResponse()
        {
            var req = new PreAuthRequest
            {
                OrderNumber = "ord",
                Amount = 100,
                Currency = 978,
                ReturnUrl = "https://return.url"
            };
            var expected = new RegisterResponse { OrderId = "123", FormUrl = "url" };
            SetupHttpResponse("registerPreAuth.do", System.Text.Json.JsonSerializer.Serialize(expected));
            var result = await _client.RegisterPreAuthAsync(req);
            Assert.Equal(expected.OrderId, result.OrderId);
        }

        [Fact]
        public async Task RegisterPreAuthAsync_InvalidArguments_Throws()
        {
            var req = new PreAuthRequest();
            await Assert.ThrowsAsync<ArgumentNullException>(() => _client.RegisterPreAuthAsync(req));
            req.OrderNumber = "ord";
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => _client.RegisterPreAuthAsync(req));
            req.Amount = 1;
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => _client.RegisterPreAuthAsync(req));
            req.Currency = 1;
            await Assert.ThrowsAsync<ArgumentNullException>(() => _client.RegisterPreAuthAsync(req));
        }

        [Fact]
        public async Task ConfirmPreAuthAsync_ValidRequest_ReturnsResponse()
        {
            var req = new ConfirmRequest { OrderId = "oid" };
            var expected = new BaseResponse { RawErrorCode = 0 };
            SetupHttpResponse("deposit.do", System.Text.Json.JsonSerializer.Serialize(expected));
            var result = await _client.ConfirmPreAuthAsync(req);
            Assert.Equal(expected.ErrorCode, result.ErrorCode);
        }

        [Fact]
        public async Task ConfirmPreAuthAsync_InvalidArguments_Throws()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _client.ConfirmPreAuthAsync(null));
            await Assert.ThrowsAsync<ArgumentNullException>(() => _client.ConfirmPreAuthAsync(new ConfirmRequest()));
        }

        [Fact]
        public async Task CancelPaymentAsync_ValidRequest_ReturnsResponse()
        {
            var req = new CancelRequest { OrderId = "oid" };
            var expected = new BaseResponse { RawErrorCode = 0 };
            SetupHttpResponse("reverse.do", System.Text.Json.JsonSerializer.Serialize(expected));
            var result = await _client.CancelPaymentAsync(req);
            Assert.Equal(expected.ErrorCode, result.ErrorCode);
        }

        [Fact]
        public async Task CancelPaymentAsync_InvalidArguments_Throws()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _client.CancelPaymentAsync(null));
            await Assert.ThrowsAsync<ArgumentNullException>(() => _client.CancelPaymentAsync(new CancelRequest()));
        }

        [Fact]
        public async Task RefundPaymentAsync_ValidRequest_ReturnsResponse()
        {
            var req = new RefundRequest { OrderId = "oid", Amount = 100 };
            var expected = new BaseResponse { RawErrorCode = 0 };
            SetupHttpResponse("refund.do", System.Text.Json.JsonSerializer.Serialize(expected));
            var result = await _client.RefundPaymentAsync(req);
            Assert.Equal(expected.ErrorCode, result.ErrorCode);
        }

        [Fact]
        public async Task RefundPaymentAsync_InvalidArguments_Throws()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _client.RefundPaymentAsync(null));
            await Assert.ThrowsAsync<ArgumentNullException>(() => _client.RefundPaymentAsync(new RefundRequest()));
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => _client.RefundPaymentAsync(new RefundRequest { OrderId = "oid", Amount = 0 }));
        }

        [Fact]
        public async Task GetOrderStatusAsync_ValidRequest_ReturnsResponse()
        {
            var req = new StatusRequest { OrderId = "oid" };
            var expected = new OrderStatusResponse { RawErrorCode = 0 };
            SetupHttpResponse("getOrderStatus.do", System.Text.Json.JsonSerializer.Serialize(expected));
            var result = await _client.GetOrderStatusAsync(req);
            Assert.Equal(expected.ErrorCode, result.ErrorCode);
        }

        [Fact]
        public async Task GetOrderStatusAsync_InvalidArguments_Throws()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _client.GetOrderStatusAsync(null));
            await Assert.ThrowsAsync<ArgumentNullException>(() => _client.GetOrderStatusAsync(new StatusRequest()));
        }

        [Fact]
        public async Task GetOrderStatusExtendedAsync_ValidRequest_ReturnsResponse()
        {
            var req = new StatusExtendedRequest { OrderNumber = "ord" };
            var expected = new OrderStatusExtendedResponse { RawErrorCode = 0 };
            SetupHttpResponse("getOrderStatusExtended.do", System.Text.Json.JsonSerializer.Serialize(expected));
            var result = await _client.GetOrderStatusExtendedAsync(req);
            Assert.Equal(expected.ErrorCode, result.ErrorCode);
        }

        [Fact]
        public async Task GetOrderStatusExtendedAsync_InvalidArguments_Throws()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _client.GetOrderStatusExtendedAsync(null));
            await Assert.ThrowsAsync<ArgumentNullException>(() => _client.GetOrderStatusExtendedAsync(new StatusExtendedRequest()));
        }

        [Fact]
        public async Task RegisterPaymentAsync_HttpError_Throws()
        {
            var req = new RegisterRequest
            {
                OrderNumber = "ord",
                Amount = 100,
                Currency = 978,
                ReturnUrl = "https://return.url"
            };
            SetupHttpResponse("register.do", "{}", HttpStatusCode.BadRequest);
            await Assert.ThrowsAsync<HttpRequestException>(() => _client.RegisterPaymentAsync(req));
        }
    }
}
