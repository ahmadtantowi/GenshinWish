using System.Threading.Tasks;
using GenshinWish.Models;
using Refit;

namespace GenshinWish.Apis
{
    public interface IWish : IGenshinWish
    {
        [Get("/event/gacha_info/api/getConfigList")]
        Task<Response<WishTypeCollection>> GetWishConfig();

        [Get("/event/gacha_info/api/getGachaLog")]
        Task<Response<Pagination<Wish>>> GetWishLog();
    }
}