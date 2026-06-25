using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace InternetBank.Web.ViewModels;

public class OpenAccountViewModel
{
    [Required(ErrorMessage = "Выберите валюту")]
    public string Currency { get; set; } = "BYN";

    [Range(0, double.MaxValue, ErrorMessage = "Сумма должна быть неотрицательной")]
    [Display(Name = "Начальный баланс")]
    public decimal InitialBalance { get; set; } = 0;

    public List<SelectListItem> Currencies { get; set; } = new();
}