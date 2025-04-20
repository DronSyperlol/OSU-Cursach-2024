using Backend.Controllers.Auth;
using Backend.Controllers.Cors;
using Backend.Services;
using Database;
using Microsoft.EntityFrameworkCore;

namespace Backend
{
    public class Program
    {
        public static readonly string ApiVer = "v1";
        public static void Main(string[] args)
        {
            new ApplicationContext().Database.Migrate();
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddDbContext<ApplicationContext>();
            builder.Services.AddHostedService<CoreService>();
            builder.Services.AddHostedService<LoggingService>();
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();

            var app = builder.Build();

            app.UsePathBase("/api/" + ApiVer);
            
            app.UseHttpsRedirection();
            app.Use(CorsController.InsertHeaders); // Custom cors
                                                 // Только так у меня получилось протащить заголовки,
                                                 // разрешающие CORS, через прокси keenDNS.
                                                 // При использовании CORS через Services, OPTIONS
                                                 // запросы не проходили из-за чего браузер блокировал
                                                 // ответы.
            app.Use(AuthController.CustomAuthorization);
            app.MapControllers();

            app.Run();
        }
    }
}
