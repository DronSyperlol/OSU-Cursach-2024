namespace Core
{
    public class Program
    {
        public static readonly string ApiVer = "v1";
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            const string CORS_POLICY_NAME = "CorsPolicy";

            builder.Services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders =
                    Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedFor |
                    Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto;
            });
            builder.Services.AddCors(options =>
            {
                options.AddPolicy(name: CORS_POLICY_NAME,
                                  policy =>
                                  {
                                      //policy.WithOrigins("https://dronsyperlol.github.io/", "http://localhost:3000/", "http://localhost:3001/");
                                      //policy.WithHeaders("Content-type");
                                      //policy.WithMethods("POST, OPTIONS");
                                      policy.AllowAnyHeader();
                                      policy.AllowAnyOrigin();
                                      policy.AllowAnyMethod();
                                  });
            });
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();

            var app = builder.Build();

            app.UseForwardedHeaders();
            app.UseCors(CORS_POLICY_NAME);
            app.UsePathBase("/api/" + ApiVer);
            app.MapControllers();

            app.Run();
        }
    }
}
