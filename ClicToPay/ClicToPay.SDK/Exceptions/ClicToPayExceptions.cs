using ClicToPay.SDK.Enums;

namespace ClicToPay.SDK.Exceptions
{
    public class ClicToPayException : Exception
    {
        public ClicToPayErrorCode ErrorCode { get; }
        public string ApiErrorMessage { get; }

        public ClicToPayException(ClicToPayErrorCode errorCode, string apiErrorMessage, string message, Exception innerException = null)
            : base(message, innerException)
        {
            ErrorCode = errorCode;
            ApiErrorMessage = apiErrorMessage;
        }
    }

    public class ClicToPayApiException : ClicToPayException
    {
        public ClicToPayApiException(ClicToPayErrorCode errorCode, string apiErrorMessage, string message = "Une erreur est survenue lors de l'appel à l'API ClicToPay.")
            : base(errorCode, apiErrorMessage, message) { }
    }

    public class ClicToPayDuplicateOrderException : ClicToPayApiException
    {
        public ClicToPayDuplicateOrderException(string apiErrorMessage)
            : base(ClicToPayErrorCode.DuplicateOrder, apiErrorMessage, "La commande avec ce numéro a déjà été traitée.") { }
    }

    public class ClicToPayPaymentCredentialsException : ClicToPayApiException
    {
        public ClicToPayPaymentCredentialsException(string apiErrorMessage)
            : base(ClicToPayErrorCode.PaymentCredentialsError, apiErrorMessage, "L'ordre est refusé en raison d'une erreur dans les informations d'identification de paiement.") { }
    }

    public class ClicToPayUnknownCurrencyException : ClicToPayApiException
    {
        public ClicToPayUnknownCurrencyException(string apiErrorMessage)
            : base(ClicToPayErrorCode.UnknownCurrency, apiErrorMessage, "La monnaie spécifiée est inconnue.") { }
    }

    public class ClicToPayInvalidParameterException : ClicToPayApiException
    {
        public ClicToPayInvalidParameterException(ClicToPayErrorCode errorCode, string apiErrorMessage)
            : base(errorCode, apiErrorMessage, "Un ou plusieurs paramètres de la requête sont invalides ou manquants.") { }
    }

    public class ClicToPayAccessDeniedException : ClicToPayApiException
    {
        public ClicToPayAccessDeniedException(string apiErrorMessage)
            : base(ClicToPayErrorCode.IncorrectParameterValue, apiErrorMessage, "Accès refusé. Vérifiez vos identifiants ou permissions.") { }
    }

    public class ClicToPayOrderNotFoundException : ClicToPayApiException
    {
        public ClicToPayOrderNotFoundException(string apiErrorMessage)
            : base(ClicToPayErrorCode.OrderNotFound, apiErrorMessage, "La commande spécifiée n'a pas été trouvée.") { }
    }

    public class ClicToPaySystemErrorException : ClicToPayApiException
    {
        public ClicToPaySystemErrorException(string apiErrorMessage)
            : base(ClicToPayErrorCode.SystemError, apiErrorMessage, "Une erreur système interne est survenue sur l'API ClicToPay.") { }
    }
}
