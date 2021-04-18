using FetchData;
using FetchData.Extensions;
using GenshinWish.Apis;
using Microsoft.Extensions.DependencyInjection;

namespace GenshinWish.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddGenshinWish(this IServiceCollection services)
        {
            var genshinApiServiceConfig = new ApiServiceConfiguration
            {
                Configuration = new ApiConfiguration
                {
                    Host = "https://hk4e-api.mihoyo.com",
                    SerializeMode = FetchData.Serialization.SerializeNamingProperty.SnakeCase
                },
                Modules = new[] { typeof(IGenshinWish) }
            };
            services.AddApiServices(genshinApiServiceConfig);

            return services;
        }
    }
}