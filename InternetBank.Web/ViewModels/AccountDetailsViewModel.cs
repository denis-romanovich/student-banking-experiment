using InternetBank.Web.Models;

namespace InternetBank.Web.ViewModels;

public class AccountDetailsViewModel
{
    public Account Account { get; set; } = null!;
    public IEnumerable<Transaction> Transactions { get; set; } = new List<Transaction>();
}