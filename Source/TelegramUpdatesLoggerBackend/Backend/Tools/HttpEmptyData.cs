namespace Backend.Tools
{
    public class HttpEmptyData : HttpDataBase
    {
        public HttpEmptyData() { }
        public HttpEmptyData(long userId, string sessionCode) 
        {
            Sign(userId, sessionCode);
        }
    }
}
