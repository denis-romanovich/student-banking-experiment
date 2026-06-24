using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using InternetBank.Web.Services;
using InternetBank.Web.ViewModels;

namespace InternetBank.Web.Controllers;

[Route("[controller]")]
public class AccountsController : Controller
{
    private readonly IAccountService _accountService;
    private readonly ICurrencyService _currencyService;

    public AccountsController(IAccountService accountService, ICurrencyService currencyService)
    {
        _accountService = accountService;
        _currencyService = currencyService;
    }

    // GET: /Accounts
    [HttpGet]
    public async Task<IActionResult> Index(string currency, bool? isActive)
    {
        var userIdString = HttpContext.Session.GetString("UserId");
        if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            return RedirectToPage("/Login");

        var accounts = await _accountService.GetAccountsByUserIdAsync(userId, currency, isActive);
        var currencies = await _currencyService.GetActiveCurrenciesAsync();

        var viewModel = new AccountListViewModel
        {
            Accounts = accounts,
            CurrencyFilter = currency,
            IsActiveFilter = isActive,
            Currencies = new List<SelectListItem>
            {
                new SelectListItem { Text = "Все", Value = "" }
            }
        };
        viewModel.Currencies.AddRange(
            currencies.Select(c => new SelectListItem { Text = c.Code, Value = c.Code })
        );

        return View(viewModel);
    }

    // GET: /Accounts/Open
    [HttpGet("Open")]
    public async Task<IActionResult> Open()
    {
        var userIdString = HttpContext.Session.GetString("UserId");
        if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int _))
            return RedirectToPage("/Login");

        var currencies = await _currencyService.GetActiveCurrenciesAsync();
        var model = new OpenAccountViewModel
        {
            Currencies = currencies
                .Select(c => new SelectListItem { Text = c.Code, Value = c.Code })
                .ToList()
        };
        return View(model);
    }

    // POST: /Accounts/Open
    [HttpPost("Open")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Open(OpenAccountViewModel model)
    {
        if (!ModelState.IsValid)
        {
            // При ошибке повторно загружаем список валют из БД
            var currencies = await _currencyService.GetActiveCurrenciesAsync();
            model.Currencies = currencies
                .Select(c => new SelectListItem { Text = c.Code, Value = c.Code })
                .ToList();
            return View(model);
        }

        var userIdString = HttpContext.Session.GetString("UserId");
        if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            return RedirectToPage("/Login");

        var success = await _accountService.CreateAccountAsync(userId, model.Currency, model.InitialBalance);
        if (!success)
        {
            ModelState.AddModelError("", "Не удалось открыть счёт.");
            // Снова загружаем валюты, если нужно показать форму с ошибкой
            var currencies = await _currencyService.GetActiveCurrenciesAsync();
            model.Currencies = currencies
                .Select(c => new SelectListItem { Text = c.Code, Value = c.Code })
                .ToList();
            return View(model);
        }

        return RedirectToAction(nameof(Index));
    }

    // POST: /Accounts/Close/{id}
    [HttpPost("Close/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Close(int id)
    {
        var userIdString = HttpContext.Session.GetString("UserId");
        if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            return RedirectToPage("/Login");

        var account = await _accountService.GetAccountByIdAsync(id);
        if (account is null || account.UserId != userId)
            return NotFound();

        await _accountService.CloseAccountAsync(id);
        return RedirectToAction(nameof(Index));
    }

    // GET: /Accounts/Details/{id}
    [HttpGet("Details/{id}")]
    public async Task<IActionResult> Details(int id)
    {
        var userIdString = HttpContext.Session.GetString("UserId");
        if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            return RedirectToPage("/Login");

        var account = await _accountService.GetAccountByIdAsync(id);
        if (account is null || account.UserId != userId)
            return NotFound();

        var transactions = await _accountService.GetTransactionsAsync(id);
        var viewModel = new AccountDetailsViewModel
        {
            Account = account,
            Transactions = transactions
        };
        return View(viewModel);
    }
}