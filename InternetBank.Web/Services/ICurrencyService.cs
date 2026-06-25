using InternetBank.Web.Models;

namespace InternetBank.Web.Services
{
    public interface ICurrencyService
    {
        Task<IEnumerable<Currency>> GetActiveCurrenciesAsync();
    }
}