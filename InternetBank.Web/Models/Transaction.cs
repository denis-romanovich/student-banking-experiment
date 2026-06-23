using System;

namespace InternetBank.Web.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Account Account { get; set; } = null!;
    }
}