using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Backend.Tools
{
    public class HttpErrorData : HttpDataBase
    {
        public HttpStatusCode statusCode { get; set; }
        public required string error { get; set; }

        public static ObjectResult Create(
            HttpStatusCode code = HttpStatusCode.InternalServerError, string error = "Something wrong")
            => new(new HttpErrorData() { statusCode = code, error = error });
         public static ObjectResult CreateAndSign(
             HttpStatusCode code, 
             string error,
             long userId, 
             string sessionCode)
        {
            var result = new HttpErrorData() { statusCode = code, error = error };
            result.Sign(userId, sessionCode);
            return new ObjectResult(result);
        }
    }
}
