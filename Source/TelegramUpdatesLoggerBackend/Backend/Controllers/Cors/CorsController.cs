using Microsoft.AspNetCore.Mvc;

// Custom cors
// Только так у меня получилось протащить заголовки,
// разрешающие CORS, через прокси keenDNS.
// При использовании CORS через Services, OPTIONS
// запросы не проходили из-за чего браузер блокировал
// ответы.

namespace Backend.Controllers.Cors
{
    [Route("{section}")]
    [ApiController]
    public class CorsController : ControllerBase
    {
        [HttpOptions("{methodName}")]
        public void HandleOptions(string section, string methodName)
        {
            Response.StatusCode = 200;
            Response.Headers.TryAdd("Access-Control-Allow-Methods", "POST");
            Response.Headers.TryAdd("Access-Control-Allow-Headers", "Content-type, userId");
            Response.Headers.TryAdd("Access-Control-Allow-Origin", "*");
            Response.Headers.TryAdd("Access-Control-Max-Age", "5");
        }

        public static async Task InsertHeaders(HttpContext context, Func<Task> next) 
        {
            context.Response.Headers.TryAdd("Access-Control-Allow-Headers", "Content-type, userId");
            context.Response.Headers.TryAdd("Access-Control-Allow-Origin", "*");
            await next.Invoke();
        }
    }
}
