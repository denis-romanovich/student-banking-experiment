using System.ComponentModel.DataAnnotations;

namespace InternetBank.Web.Models
{
    public class Currency
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(3)]
        public string Code { get; set; } = string.Empty;  // USD, EUR, BYN

        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;  // Доллар США, Евро

        public bool IsActive { get; set; } = true;
    }
}