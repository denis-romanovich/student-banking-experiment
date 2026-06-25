using Microsoft.AspNetCore.Mvc.Rendering;
using InternetBank.Web.Models;

namespace InternetBank.Web.ViewModels;

public class AccountListViewModel
{
    public IEnumerable<Account> Accounts { get; set; } = new List<Account>();
    public string? CurrencyFilter { get; set; }
    public bool? IsActiveFilter { get; set; }
    public List<SelectListItem> Currencies { get; set; } = new();
}