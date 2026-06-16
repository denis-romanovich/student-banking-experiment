using Microsoft.AspNetCore.Mvc.RazorPages;

namespace InternetBank.Web.Pages;

public class IndexModel : PageModel
{
    public string TotalClients { get; set; } = "500тыс+";
    public string Fee { get; set; } = "0 BYN";
    public List<Testimonial> Testimonials { get; set; } = new();

    public void OnGet()
    {
        Testimonials = new List<Testimonial>
        {
            new Testimonial { Text = "«Открыл счёт за 3 минуты, карту привезли курьером. Очень удобное приложение!»", Author = "Анна" },
            new Testimonial { Text = "«Лучший кешбэк на рынке, поддержка отвечает мгновенно. Советую.»", Author = "Дмитрий" },
            new Testimonial { Text = "«Перевожу деньги за границу без комиссий, всё прозрачно. Спасибо банку.»", Author = "Елена" }
        };
    }
}
public class Testimonial
{
    public string Text { get; set; } = "";
    public string Author { get; set; } = "";
}