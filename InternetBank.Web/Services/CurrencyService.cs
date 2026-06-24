using Microsoft.EntityFrameworkCore;
using InternetBank.Web.Data;
using InternetBank.Web.Models;

namespace InternetBank.Web.Services
{
    public class CurrencyService : ICurrencyService
    {
        private readonly BankDbContext _db;

        public CurrencyService(BankDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Currency>> GetActiveCurrenciesAsync()
        {
            return await _db.Currencies
                .Where(c => c.IsActive)
                .OrderBy(c => c.Code)
                .ToListAsync();
        }
    }
}