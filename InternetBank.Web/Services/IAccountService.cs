using InternetBank.Web.Models;

namespace InternetBank.Web.Services;

public interface IAccountService
{
    Task<IEnumerable<Account>> GetAccountsByUserIdAsync(int userId, string? currency = null, bool? isActive = null);
    Task<Account?> GetAccountByIdAsync(int accountId);
    Task<bool> CreateAccountAsync(int userId, string currency, decimal initialBalance = 0);
    Task<bool> CloseAccountAsync(int accountId);
    Task<IEnumerable<Transaction>> GetTransactionsAsync(int accountId, int count = 20);
}