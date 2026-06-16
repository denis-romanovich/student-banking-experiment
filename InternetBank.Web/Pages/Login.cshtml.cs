using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using InternetBank.Web.Services;
using System.ComponentModel.DataAnnotations;

namespace InternetBank.Web.Pages;

public class LoginModel : PageModel
{
    private readonly IUserService _userService;

    public LoginModel(IUserService userService)
    {
        _userService = userService;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public string? ErrorMessage { get; set; }

    public class InputModel
    {
        [Required(ErrorMessage = "Введите email")]
        [EmailAddress(ErrorMessage = "Неверный формат email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Введите пароль")]
        public string Password { get; set; } = string.Empty;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        var isValid = await _userService.ValidateCredentialsAsync(Input.Email, Input.Password);
        if (!isValid)
        {
            ErrorMessage = "Неверный email или пароль";
            return Page();
        }

        var user = await _userService.GetUserByEmailAsync(Input.Email);
        if (user is null)
        {
            ErrorMessage = "Пользователь не найден";
            return Page();
        }

        // Сохраняем данные в сессию
        HttpContext.Session.SetString("UserId", user.Id.ToString());
        HttpContext.Session.SetString("UserEmail", user.Email);
        HttpContext.Session.SetString("UserFirstName", user.FirstName);

        // Редирект на главную
        return RedirectToPage("/Index");
    }
}