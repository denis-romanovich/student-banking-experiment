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

            // Добавляем Razor Pages
            builder.Services.AddRazorPages();

            // Регистрируем сервис пользователей (теперь с БД)
            builder.Services.AddScoped<IUserService, DbUserService>();

            // Настройка базы данных
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<BankDbContext>(options =>
                options.UseSqlServer(connectionString));

            // ===== НАСТРОЙКА СЕССИЙ (ОБЯЗАТЕЛЬНО) =====
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
            // =======================================

            var app = builder.Build();

            app.UseStaticFiles();   // статика (css, js)
            app.UseRouting();       // маршрутизация
            app.UseSession();       // <-- включаем сессии
            app.MapRazorPages();    // Razor Pages

            app.Run();
        }
    }
}