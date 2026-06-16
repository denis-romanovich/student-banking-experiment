using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using InternetBank.Web.Data;
using InternetBank.Web.Models;

namespace InternetBank.Web.Services
{
    public class CurrencyUpdateService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<CurrencyUpdateService> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private const string ApiUrl = "https://api.nbrb.by/exrates/rates?periodicity=0";

      
        private readonly string[] _targetCurrencies = { "USD", "EUR", "RUB" };

        public CurrencyUpdateService(
            IServiceProvider serviceProvider,
            ILogger<CurrencyUpdateService> logger,
            IHttpClientFactory httpClientFactory)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Фоновый сервис обновления курсов валют запущен.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation("Запуск скачивания курсов валют с НБРБ...");
                    await UpdateRatesAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Произошла ошибка при обновлении курсов валют.");
                }

          
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }

            _logger.LogInformation("Фоновый сервис обновления курсов валют останавливается.");
        }

        private async Task UpdateRatesAsync()
        {
            using var httpClient = _httpClientFactory.CreateClient();

            
            var allRates = await httpClient.GetFromJsonAsync<List<CurrencyRate>>(ApiUrl);

            if (allRates == null || !allRates.Any())
            {
                _logger.LogWarning("От API НБРБ получен пустой ответ.");
                return;
            }

            // Фильтр массива, оставляет только Доллар, Евро и Российский рубль
            var filteredRates = allRates
                .Where(r => _targetCurrencies.Contains(r.CurrencyCode))
                .ToList();

            // фоновый сервис является Singleton, мы не можем внедрить BankDbContext напрямую через конструктор
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<BankDbContext>();

            foreach (var newRate in filteredRates)
            {
          
                var existingRate = dbContext.CurrencyRates
                    .FirstOrDefault(r => r.CurrencyCode == newRate.CurrencyCode);

                if (existingRate != null)
                {
                    
                    existingRate.Rate = newRate.Rate;
                    existingRate.Scale = newRate.Scale;
                    existingRate.UpdatedAt = newRate.UpdatedAt;
                }
                else
                {
                    
                    dbContext.CurrencyRates.Add(newRate);
                }
            }

         
            await dbContext.SaveChangesAsync();
            _logger.LogInformation("Курсы валют успешно сохранены в базу данных.");
        }
    }
}