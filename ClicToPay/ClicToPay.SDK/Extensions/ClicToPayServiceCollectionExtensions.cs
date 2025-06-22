using ClicToPay.SDK.Options;
using Microsoft.Extensions.Configuration;
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
                client.BaseAddress = new Uri(options.GetBaseUrl());
            })
            .AddTypedClient((httpClient, provider) =>
            {
                var options = provider.GetRequiredService<IOptions<ClicToPayOptions>>().Value;
                return new ClicToPayClient(httpClient, options);
            });

            return services;
        }

        public static IServiceCollection AddClicToPayOptions(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions<ClicToPayOptions>()
                .Bind(configuration.GetSection("ClicToPay"))
                .ValidateDataAnnotations()
                .Validate(options => !string.IsNullOrWhiteSpace(options.Username), "ClicToPay - Username is required")
                .Validate(options => !string.IsNullOrWhiteSpace(options.Password), "ClicToPay - Password is required");

            return services;
        }


    }
}
