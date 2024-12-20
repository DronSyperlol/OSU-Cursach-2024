using Backend.Controllers.Auth;
using Backend.Controllers.Cors;
using Database;
using Microsoft.EntityFrameworkCore;

namespace Core
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
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();

            var app = builder.Build();

            app.UsePathBase("/api/" + ApiVer);
            
            app.UseHttpsRedirection();
            app.Use(CorsResolver.InsertHeaders); // Custom cors
                                                 // ������ ��� � ���� ���������� ��������� ���������,
                                                 // ����������� CORS, ����� ������ keenDNS.
                                                 // ��� ������������� CORS ����� Services, OPTIONS
                                                 // ������� �� ��������� ��-�� ���� ������� ����������
                                                 // ������.
            app.Use(Auth.CustomAuthorization);
            app.MapControllers();

            app.Run();
        }
    }
}
