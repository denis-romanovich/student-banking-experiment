using Microsoft.EntityFrameworkCore;
using InternetBank.Web.Data;
using InternetBank.Web.Services;

namespace InternetBank.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

            builder.Services.AddDbContext<BankDbContext>(options =>
                options.UseSqlServer(connectionString));

            // инструмент для выполнения запросов к API Нацбанка
            builder.Services.AddHttpClient();

            // фоновый сервис для автоматического обновления курсов валют
            builder.Services.AddHostedService<CurrencyUpdateService>();

            var app = builder.Build();

            app.MapGet("/", () => "Hello World!");

            app.Run();
        }
    }
}