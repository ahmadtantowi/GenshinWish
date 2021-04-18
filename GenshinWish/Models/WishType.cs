using System.Collections.Generic;

namespace GenshinWish.Models
{
    public class WishType
    {
        public string Id { get; set; }
        public string Key { get; set; }
        public string Name { get; set; }
    }

    public class WishTypeCollection
    {
        public IEnumerable<WishType> GachaTypeList { get; set; }
    }
}