namespace ClicToPay.SDK.Enums
{
    public enum ClicToPayErrorCode
    {
        /// <summary>
        /// 0: Pas d'erreur système.
        /// </summary>
        Success = 0,

        /// <summary>
        /// 1: Numéro de commande dupliqué, commande avec le numéro de commande donné est déjà traitée.
        /// </summary>
        DuplicateOrder = 1,

        /// <summary>
        /// 2: L'ordre est refusé en raison d'une erreur dans les informations d'identification de paiement.
        /// </summary>
        PaymentCredentialsError = 2,

        /// <summary>
        /// 3: Monnaie inconnue.
        /// </summary>
        UnknownCurrency = 3,

        /// <summary>
        /// 4: Paramètre obligatoire n'a pas été spécifié.
        /// </summary>
        RequiredParameterNotSpecified = 4,

        /// <summary>
        /// 5: Valeur erronée d'un paramètre de la requête.
        /// </summary>
        IncorrectParameterValue = 5,

        /// <summary>
        /// 6: Identifiant commande non enregistré.
        /// </summary>
        OrderNotFound = 6,

        /// <summary>
        /// 7: Erreur système.
        /// </summary>
        SystemError = 7,

        /// <summary>
        /// Code d'erreur inconnu non documenté.
        /// </summary>
        UnknownError = -1 // Pour les codes non mappés
    }
    public enum ClicToPayOrderStatus
    {
        /// <summary>
        /// 0: Commande enregistrée, mais pas payé.
        /// </summary>
        RegisteredNotPaid = 0,

        /// <summary>
        /// 1: Montant pré-autorisation bloqué (pour le paiement en deux phases).
        /// </summary>
        PreAuthorizationBlocked = 1,

        /// <summary>
        /// 2: Le montant a été déposé avec succès.
        /// </summary>
        AmountDepositedSuccessfully = 2,

        /// <summary>
        /// 3: Annulation d'autorisation.
        /// </summary>
        AuthorizationCancelled = 3,

        /// <summary>
        /// 4: Transaction remboursée.
        /// </summary>
        TransactionRefunded = 4,

        /// <summary>
        /// 5: Autorisation par le biais du ACS de l'émetteur initié.
        /// </summary>
        AuthorizationViaACS = 5,

        /// <summary>
        /// 6: Autorisation refusée.
        /// </summary>
        AuthorizationRefused = 6,

        /// <summary>
        /// Statut de commande inconnu non documenté.
        /// </summary>
        Unknown = -1 // Pour les statuts non mappés
    }
}
