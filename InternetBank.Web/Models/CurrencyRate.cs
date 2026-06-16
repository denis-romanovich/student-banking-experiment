using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace InternetBank.Web.Models
{
    public class CurrencyRate
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(3)]
        [JsonPropertyName("Cur_Abbreviation")]
        public string CurrencyCode { get; set; }

        [Required]
        [JsonPropertyName("Cur_Scale")]
        public int Scale { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 4)")]
        [JsonPropertyName("Cur_OfficialRate")]
        public decimal Rate { get; set; }

        [Required]
        [JsonPropertyName("Date")]
        public DateTime UpdatedAt { get; set; }
    }
}