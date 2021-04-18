using System.Collections.Generic;

namespace GenshinWish.Models
{
    public class Pagination<T>
    {
        public string Page { get; set; }
        public string Size { get; set; }
        public string Total { get; set; }
        public IEnumerable<T> List { get; set; }
        public string Region { get; set; }
    }
}