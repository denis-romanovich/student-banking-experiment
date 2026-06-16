using InternetBank.Web.Models;

namespace InternetBank.Web.Services;

public interface IUserService
{
    Task<User?> GetUserByEmailAsync(string email);
    Task<bool> CreateUserAsync(string email, string password, string firstName, string lastName);
    Task<bool> ValidateCredentialsAsync(string email, string password);
}