using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using InternetBank.Web.Services;
using System.ComponentModel.DataAnnotations;

namespace InternetBank.Web.Pages;

public class RegisterModel : PageModel
{
    private readonly IUserService _userService;

    public RegisterModel(IUserService userService)
    {
        _userService = userService;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public string? ErrorMessage { get; set; }

    public class InputModel
    {
        [Required(ErrorMessage = "Введите имя")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Введите фамилию")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Введите email")]
        [EmailAddress(ErrorMessage = "Неверный формат email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Введите пароль")]
        [MinLength(6, ErrorMessage = "Пароль должен содержать минимум 6 символов")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Подтвердите пароль")]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        var existing = await _userService.GetUserByEmailAsync(Input.Email);
        if (existing is not null)
        {
            ErrorMessage = "Пользователь с таким email уже существует";
            return Page();
        }

        var success = await _userService.CreateUserAsync(Input.Email, Input.Password, Input.FirstName, Input.LastName);
        if (!success)
        {
            ErrorMessage = "Не удалось создать пользователя. Попробуйте ещё раз.";
            return Page();
        }

        return RedirectToPage("/Login");
    }
}