# ClicToPay.SDK

ClicToPay.SDK est un client .NET moderne pour l’API ClicToPay, facilitant l’intégration des paiements, pré-autorisations, remboursements et la gestion des statuts de commande dans vos applications .NET (ASP.NET Core, services, etc.).

## 1. Installation

### Via le gestionnaire de packages NuGet (Visual Studio)

1. Clic droit sur votre projet > **Gérer les packages NuGet**
2. Recherchez `ClicToPay.SDK` et installez le package officiel.

### Via la ligne de commande .NET CLI
dotnet add package ClicToPay.SDK
## 2. Configuration et Injection de Dépendances

### Configuration des options

Définissez vos identifiants dans `ClicToPayOptions` :
using ClicToPay.SDK.Options;

var options = new ClicToPayOptions
{
    Username = "VOTRE_IDENTIFIANT",
    Password = "VOTRE_MOTDEPASSE",
    UseSandbox = true // ou false pour la production
};
### Enregistrement dans le conteneur d’injection de dépendances (ASP.NET Core)

Pour .NET 6+ (Program.cs) :
using ClicToPay.SDK.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Ajout via l'extension officielle (recommandé)
builder.Services.AddClicToPayClient(options =>
{
    options.Username = builder.Configuration["ClicToPay:Username"];
    options.Password = builder.Configuration["ClicToPay:Password"];
    options.UseSandbox = builder.Configuration.GetValue<bool>("ClicToPay:UseSandbox");
});

var app = builder.Build();
// ...
> Vous pouvez aussi utiliser `AddClicToPayOptions` pour binder automatiquement la configuration :
>
> ```csharp
> builder.Services.AddClicToPayOptions(builder.Configuration);
> builder.Services.AddClicToPayClient(_ => { });
> ```

### Exemple d’utilisation dans un contrôleur ASP.NET Core
using ClicToPay.SDK;
using ClicToPay.SDK.Dtos;
using Microsoft.AspNetCore.Mvc;

public class PaiementController : ControllerBase
{
    private readonly IClicToPayClient _clicToPayClient;
    public PaiementController(IClicToPayClient clicToPayClient)
    {
        _clicToPayClient = clicToPayClient;
    }

    [HttpPost("/paiement/register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest req)
    {
        try
        {
            var result = await _clicToPayClient.RegisterPaymentAsync(req);
            return Ok(result);
        }
        catch (ClicToPay.SDK.Exceptions.ClicToPayApiException ex)
        {
            // Gestion des erreurs métier ClicToPay
            return BadRequest(new { ex.ErrorCode, ex.ApiErrorMessage });
        }
        catch (HttpRequestException ex)
        {
            // Gestion des erreurs réseau/HTTP
            return StatusCode(502, ex.Message);
        }
    }
}
## 3. Utilisation du Client API

Chaque méthode de l’interface `IClicToPayClient` correspond à une opération de l’API ClicToPay. Voici la documentation détaillée :

---
### `Task<RegisterResponse> RegisterPaymentAsync(RegisterRequest request)`
**Description :** Enregistre une nouvelle transaction de paiement (autorisation immédiate).

- **Paramètres :**
  - `RegisterRequest` : contient `OrderNumber` (string, requis), `Amount` (long, requis), `Currency` (int, requis), `ReturnUrl` (string, requis), et options (`FailUrl`, `Description`, etc.).
- **Retour :**
  - `RegisterResponse` : inclut `OrderId` (string), `FormUrl` (string), `ErrorCode`, `ErrorMessage`.
- **Exceptions :**
  - `ClicToPayDuplicateOrderException`, `ClicToPayInvalidParameterException`, `ClicToPayAccessDeniedException`, `ClicToPayUnknownCurrencyException`, `ClicToPayPaymentCredentialsException`, `ClicToPayApiException`, `HttpRequestException`.
- **Exemple :**using ClicToPay.SDK;
using ClicToPay.SDK.Dtos;

try
{
    var req = new RegisterRequest { OrderNumber = "ORD-001", Amount = 1000, Currency = 978, ReturnUrl = "https://retour" };
    var resp = await clicToPayClient.RegisterPaymentAsync(req);
    Console.WriteLine($"Commande enregistrée : {resp.OrderId}, FormUrl : {resp.FormUrl}");
}
catch (ClicToPay.SDK.Exceptions.ClicToPayApiException ex)
{
    // Gestion métier
}
catch (HttpRequestException ex)
{
    // Gestion réseau
}
---
### `Task<RegisterResponse> RegisterPreAuthAsync(PreAuthRequest request)`
**Description :** Enregistre une pré-autorisation (paiement en deux temps).

- **Paramètres :**
  - `PreAuthRequest` : identique à `RegisterRequest`.
- **Retour :**
  - `RegisterResponse`.
- **Exceptions :**
  - Identiques à `RegisterPaymentAsync`.
- **Exemple :**var req = new PreAuthRequest { OrderNumber = "ORD-002", Amount = 500, Currency = 978, ReturnUrl = "https://retour" };
var resp = await clicToPayClient.RegisterPreAuthAsync(req);
---
### `Task<BaseResponse> ConfirmPreAuthAsync(ConfirmRequest request)`
**Description :** Confirme une pré-autorisation (débit effectif après blocage).

- **Paramètres :**
  - `ConfirmRequest` : `OrderId` (string, requis), `Amount` (long?, optionnel).
- **Retour :**
  - `BaseResponse` : `ErrorCode`, `ErrorMessage`.
- **Exceptions :**
  - `ClicToPayOrderNotFoundException`, `ClicToPayInvalidParameterException`, `ClicToPaySystemErrorException`, `ClicToPayApiException`, `HttpRequestException`.
- **Exemple :**var resp = await clicToPayClient.ConfirmPreAuthAsync(new ConfirmRequest { OrderId = "OID-123" });
---
### `Task<BaseResponse> CancelPaymentAsync(CancelRequest request)`
**Description :** Annule un paiement ou une pré-autorisation.

- **Paramètres :**
  - `CancelRequest` : `OrderId` (string, requis), `Language` (string, optionnel).
- **Retour :**
  - `BaseResponse`.
- **Exceptions :**
  - `ClicToPayOrderNotFoundException`, `ClicToPayInvalidParameterException`, `ClicToPaySystemErrorException`, `ClicToPayApiException`, `HttpRequestException`.
- **Exemple :**await clicToPayClient.CancelPaymentAsync(new CancelRequest { OrderId = "OID-123" });
---
### `Task<BaseResponse> RefundPaymentAsync(RefundRequest request)`
**Description :** Rembourse un paiement existant.

- **Paramètres :**
  - `RefundRequest` : `OrderId` (string, requis), `Amount` (long, requis).
- **Retour :**
  - `BaseResponse`.
- **Exceptions :**
  - `ClicToPayOrderNotFoundException`, `ClicToPayInvalidParameterException`, `ClicToPaySystemErrorException`, `ClicToPayApiException`, `HttpRequestException`.
- **Exemple :**await clicToPayClient.RefundPaymentAsync(new RefundRequest { OrderId = "OID-123", Amount = 100 });
---
### `Task<OrderStatusResponse> GetOrderStatusAsync(StatusRequest request)`
**Description :** Récupère le statut d’une commande par son OrderId.

- **Paramètres :**
  - `StatusRequest` : `OrderId` (string, requis), `Language` (string, optionnel).
- **Retour :**
  - `OrderStatusResponse` : inclut `OrderStatus`, `OrderNumber`, `Amount`, etc.
- **Exceptions :**
  - `ClicToPayOrderNotFoundException`, `ClicToPayApiException`, `HttpRequestException`.
- **Exemple :**var status = await clicToPayClient.GetOrderStatusAsync(new StatusRequest { OrderId = "OID-123" });
---
### `Task<OrderStatusExtendedResponse> GetOrderStatusExtendedAsync(StatusExtendedRequest request)`
**Description :** Récupère le statut détaillé d’une commande par son OrderNumber.

- **Paramètres :**
  - `StatusExtendedRequest` : `OrderNumber` (string, requis), `Language` (string, optionnel).
- **Retour :**
  - `OrderStatusExtendedResponse` : informations détaillées sur la commande.
- **Exceptions :**
  - `ClicToPayOrderNotFoundException`, `ClicToPayApiException`, `HttpRequestException`.
- **Exemple :**var extStatus = await clicToPayClient.GetOrderStatusExtendedAsync(new StatusExtendedRequest { OrderNumber = "ORD-001" });
---

## Ressources complémentaires
- [Documentation API ClicToPay (officielle)](https://www.clictopay.com/documentation)
- [NuGet Gallery | ClicToPay.SDK](https://www.nuget.org/packages/ClicToPay.SDK)

Pour toute contribution ou signalement de bug, merci d’ouvrir une issue sur le dépôt GitHub officiel.
