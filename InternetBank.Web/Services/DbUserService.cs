using InternetBank.Web.Models;
using InternetBank.Web.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace InternetBank.Web.Services;

public class DbUserService : IUserService
{
    private readonly BankDbContext _db;
    private readonly PasswordHasher<User> _hasher = new();

    public DbUserService(BankDbContext db)
    {
        _db = db;
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<bool> CreateUserAsync(string email, string password, string firstName, string lastName)
    {
        // Проверяем, нет ли уже пользователя с таким email
        if (await _db.Users.AnyAsync(u => u.Email == email))
            return false;

        var user = new User
        {
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            PasswordHash = _hasher.HashPassword(null, password),
            CreatedAt = DateTime.UtcNow
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ValidateCredentialsAsync(string email, string password)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user is null)
            return false;

        var result = _hasher.VerifyHashedPassword(null, user.PasswordHash, password);
        return result == PasswordVerificationResult.Success;
    }
}