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

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();

            var app = builder.Build();

            app.UseAuthorization();
            app.UseHttpsRedirection();

            app.UsePathBase("/api/" + ApiVer);
            app.MapControllers();

            app.Use(CorsResolver.InsertHeaders); // Custom cors
                                                 // ������ ��� � ���� ���������� ��������� ���������,
                                                 // ����������� CORS, ����� ������ keenDNS.
                                                 // ��� ������������� CORS ����� Services, OPTIONS
                                                 // ������� �� ��������� ��-�� ���� ������� ����������
                                                 // ������.
            app.Run();
        }
    }
}
