using Config;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers.Static
{
    [Route("static")]
    public class StaticController : Controller
    {
        [HttpGet("{filename}")]
        public async Task Index(string filename)
        {
            if (System.IO.File.Exists(ProgramConfig.TelegramApiAuth.DownloadsDir + filename))
            {
                await Response.SendFileAsync(ProgramConfig.TelegramApiAuth.DownloadsDir + filename);
            }
            else NotFound();
        }
    }
}
