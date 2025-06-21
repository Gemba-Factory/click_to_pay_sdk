using ClicToPay.SDK.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ClicToPay.SDK.Extensions
{
    public static class ClicToPayServiceCollectionExtensions
    {
        public static IServiceCollection AddClicToPayClient(this IServiceCollection services, Action<ClicToPayOptions> configureOptions)
        {
            services.Configure(configureOptions);

            services.AddHttpClient<IClicToPayClient, ClicToPayClient>((provider, client) =>
            {
                var options = provider.GetRequiredService<IOptions<ClicToPayOptions>>().Value;

                if (string.IsNullOrWhiteSpace(options.BaseUrl))
                {
                    throw new ArgumentException("BaseUrl for ClicToPay must be provided.");
                }

                // client.BaseAddress is optional — the ClicToPayClient uses full URLs
                client.BaseAddress = new Uri(options.BaseUrl);
            });

            return services;
        }
    }
}
