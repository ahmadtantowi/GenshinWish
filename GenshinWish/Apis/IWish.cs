using System;
using System.Threading.Tasks;
using GenshinWish.Models;
using Refit;

namespace GenshinWish.Apis
{
    public interface IWish : IGenshinWish
    {
        [QueryUriFormat(UriFormat.Unescaped)]
        [Get("/event/gacha_info/api/{**getConfigList}")]
        Task<Response<WishTypeCollection>> GetWishConfig(string getConfigList);

        [QueryUriFormat(UriFormat.Unescaped)]
        [Get("/event/gacha_info/api/{**getGachaLog}")]
        Task<Response<Pagination<Wish>>> GetWishLog(string getGachaLog);
    }
}