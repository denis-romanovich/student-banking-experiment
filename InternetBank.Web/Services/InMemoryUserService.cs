using System.Collections.Concurrent;
using InternetBank.Web.Models;
using Microsoft.AspNetCore.Identity;

namespace InternetBank.Web.Services;

public class InMemoryUserService : IUserService
{
    private static readonly ConcurrentDictionary<int, User> _users = new();
    private static int _nextId = 1;

    public Task<User?> GetUserByEmailAsync(string email)
    {
        var user = _users.Values.FirstOrDefault(u => u.Email == email);
        return Task.FromResult(user);
    }

    public Task<bool> CreateUserAsync(string email, string password, string firstName, string lastName)
    {
        if (_users.Values.Any(u => u.Email == email))
            return Task.FromResult(false);

        var hasher = new PasswordHasher<User>();
        var user = new User
        {
            Id = Interlocked.Increment(ref _nextId),
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            PasswordHash = hasher.HashPassword(null, password),
            CreatedAt = DateTime.UtcNow
        };
        _users.TryAdd(user.Id, user);
        return Task.FromResult(true);
    }

    public Task<bool> ValidateCredentialsAsync(string email, string password)
    {
        var user = _users.Values.FirstOrDefault(u => u.Email == email);
        if (user is null)
            return Task.FromResult(false);

        var hasher = new PasswordHasher<User>();
        var result = hasher.VerifyHashedPassword(null, user.PasswordHash, password);
        return Task.FromResult(result == PasswordVerificationResult.Success);
    }
}