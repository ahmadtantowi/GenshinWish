using System.Linq;
using System.Threading.Tasks;
using FetchData;
using FetchData.Extensions;
using GenshinWish.Apis;
using GenshinWish.Enums;
using GenshinWish.Extensions;
using GenshinWish.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GenshinWish.Console
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // configure console logging
            LoggerFactory
                .Create(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Debug))
                .SetFetchDataLoggerFactory();
            
            // setup dependency injection
            var services = new ServiceCollection()
                .AddLogging()
                .AddGenshinWish()
                .BuildServiceProvider();

            var wishParam = GenshinLog.GetWishParam();
            var wishService = services.GetService<IApiService<IWish>>();

            var wishConfigQuery = GenshinLog.GetWishQueryString(WishEndpoint.Config, wishParam);
            var wishConfig = await wishService.Initiated.GetWishConfig(wishConfigQuery).ConfigureAwait(false);
            
            var wishLogParam = GenshinLog.AddWishLogParam(wishParam, 0, 6, wishConfig.Data.GachaTypeList.First().Key);
            var wishLogQuery = GenshinLog.GetWishQueryString(WishEndpoint.Log, wishLogParam);
            var wishLog = await wishService.Initiated.GetWishLog(wishLogQuery);
            
            var logger = services.GetService<ILogger<Program>>();
            logger.LogInformation($"Wish Log UID: {wishLog.Data.List.First().Uid}");
        }
    }
}
