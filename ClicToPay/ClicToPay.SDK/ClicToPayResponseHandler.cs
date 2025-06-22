using ClicToPay.SDK.Dtos;
using ClicToPay.SDK.Enums;
using ClicToPay.SDK.Exceptions;

namespace ClicToPay.SDK
{
    public static class ClicToPayResponseHandler
    {
        public static T HandleResponse<T>(T response) where T : BaseResponse
        {
            if (response == null)
            {
                throw new ArgumentNullException(nameof(response), "La réponse de l'API est nulle.");
            }

            if (response.IsSuccess)
            {
                return response;
            }

            // Mappage des codes d'erreur aux exceptions spécifiques
            switch (response.ErrorCode)
            {
                case ClicToPayErrorCode.DuplicateOrder:
                    throw new ClicToPayDuplicateOrderException(response.ErrorMessage);
                case ClicToPayErrorCode.PaymentCredentialsError:
                    throw new ClicToPayPaymentCredentialsException(response.ErrorMessage);
                case ClicToPayErrorCode.UnknownCurrency:
                    throw new ClicToPayUnknownCurrencyException(response.ErrorMessage);
                case ClicToPayErrorCode.RequiredParameterNotSpecified:
                case ClicToPayErrorCode.IncorrectParameterValue:
                    // Cas spécial pour "Accès refusé" qui est un message d'erreur pour errorCode 5
                    if (response.ErrorMessage?.Contains("Accès refusé", StringComparison.OrdinalIgnoreCase) == true)
                    {
                        throw new ClicToPayAccessDeniedException(response.ErrorMessage);
                    }
                    throw new ClicToPayInvalidParameterException(response.ErrorCode, response.ErrorMessage);
                case ClicToPayErrorCode.OrderNotFound:
                    throw new ClicToPayOrderNotFoundException(response.ErrorMessage);
                case ClicToPayErrorCode.SystemError:
                    throw new ClicToPaySystemErrorException(response.ErrorMessage);
                default:
                    // Pour tout autre code d'erreur non explicitement mappé
                    throw new ClicToPayApiException(ClicToPayErrorCode.UnknownError, response.ErrorMessage, $"Erreur API inconnue (Code: {response.RawErrorCode}): {response.ErrorMessage}");
            }
        }
    }
}
