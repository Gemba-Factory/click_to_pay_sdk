using System.ComponentModel.DataAnnotations;

namespace ClicToPay.SDK.Options
{
    public class ClicToPayOptions
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        public bool UseSandbox { get; set; } = true;

        private const string SandboxUrl = "https://test.clictopay.com";
        private const string ProductionUrl = "https://api.clictopay.com";

        public string GetBaseUrl()
        {
            return UseSandbox ? SandboxUrl : ProductionUrl;
        }
    }
}
