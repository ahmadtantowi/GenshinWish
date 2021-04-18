namespace GenshinWish.Models
{
    public class Response<T>
    {
        public int Retcode { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
    }
}