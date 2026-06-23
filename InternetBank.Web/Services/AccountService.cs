using Microsoft.EntityFrameworkCore;
using InternetBank.Web.Data;
using InternetBank.Web.Models;

namespace InternetBank.Web.Services;

public class AccountService : IAccountService
{
    private readonly BankDbContext _db;

    public AccountService(BankDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<Account>> GetAccountsByUserIdAsync(int userId, string? currency = null, bool? isActive = null)
    {
        var query = _db.Accounts.Where(a => a.UserId == userId);
        if (!string.IsNullOrEmpty(currency))
            query = query.Where(a => a.Currency == currency);
        if (isActive.HasValue)
            query = query.Where(a => a.IsActive == isActive.Value);
        return await query.OrderByDescending(a => a.CreatedAt).ToListAsync();
    }

    public async Task<Account?> GetAccountByIdAsync(int accountId)
    {
        return await _db.Accounts
            .Include(a => a.Transactions)
            .FirstOrDefaultAsync(a => a.Id == accountId);
    }

    public async Task<bool> CreateAccountAsync(int userId, string currency, decimal initialBalance = 0)
    {
        var user = await _db.Users.FindAsync(userId);
        if (user is null) return false;

        var account = new Account
        {
            UserId = userId,
            AccountNumber = GenerateAccountNumber(),
            Currency = currency,
            Balance = initialBalance,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        _db.Accounts.Add(account);
        await _db.SaveChangesAsync();

        if (initialBalance > 0)
        {
            var transaction = new Transaction
            {
                AccountId = account.Id,
                Amount = initialBalance,
                Description = "Начальный взнос",
                CreatedAt = DateTime.UtcNow
            };
            _db.Transactions.Add(transaction);
            await _db.SaveChangesAsync();
        }
        return true;
    }

    public async Task<bool> CloseAccountAsync(int accountId)
    {
        var account = await _db.Accounts.FindAsync(accountId);
        if (account is null) return false;
        if (!account.IsActive) return false;

        account.IsActive = false;
        account.ClosedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<Transaction>> GetTransactionsAsync(int accountId, int count = 20)
    {
        return await _db.Transactions
            .Where(t => t.AccountId == accountId)
            .OrderByDescending(t => t.CreatedAt)
            .Take(count)
            .ToListAsync();
    }

    private string GenerateAccountNumber()
    {
        var random = new Random();
        return "BYN" + random.Next(100000, 999999).ToString();
    }
}