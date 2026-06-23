using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using InternetBank.Web.Services;
using InternetBank.Web.ViewModels;

namespace InternetBank.Web.Controllers;

[Route("[controller]")]
public class AccountsController : Controller
{
    private readonly IAccountService _accountService;

    public AccountsController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    // GET: /Accounts
    [HttpGet]
    public async Task<IActionResult> Index(string currency, bool? isActive)
    {
        var userIdString = HttpContext.Session.GetString("UserId");
        if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            return RedirectToPage("/Login");

        var accounts = await _accountService.GetAccountsByUserIdAsync(userId, currency, isActive);

        var viewModel = new AccountListViewModel
        {
            Accounts = accounts,
            CurrencyFilter = currency,
            IsActiveFilter = isActive,
            Currencies = new List<SelectListItem>
            {
                new SelectListItem { Text = "Все", Value = "" },
                new SelectListItem { Text = "BYN", Value = "BYN" },
                new SelectListItem { Text = "USD", Value = "USD" },
                new SelectListItem { Text = "EUR", Value = "EUR" },
                new SelectListItem { Text = "RUB", Value = "RUB" }
            }
        };
        return View(viewModel);
    }

    // GET: /Accounts/Open
    [HttpGet("Open")]
    public IActionResult Open()
    {
        var userIdString = HttpContext.Session.GetString("UserId");
        if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int _))
            return RedirectToPage("/Login");

        var model = new OpenAccountViewModel
        {
            Currencies = new List<SelectListItem>
            {
                new SelectListItem { Text = "BYN", Value = "BYN" },
                new SelectListItem { Text = "USD", Value = "USD" },
                new SelectListItem { Text = "EUR", Value = "EUR" },
                new SelectListItem { Text = "RUB", Value = "RUB" }
            }
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
            model.Currencies = new List<SelectListItem>
            {
                new SelectListItem { Text = "BYN", Value = "BYN" },
                new SelectListItem { Text = "USD", Value = "USD" },
                new SelectListItem { Text = "EUR", Value = "EUR" },
                new SelectListItem { Text = "RUB", Value = "RUB" }
            };
            return View(model);
        }

        var userIdString = HttpContext.Session.GetString("UserId");
        if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            return RedirectToPage("/Login");

        var success = await _accountService.CreateAccountAsync(userId, model.Currency, model.InitialBalance);
        if (!success)
        {
            ModelState.AddModelError("", "Не удалось открыть счёт.");
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