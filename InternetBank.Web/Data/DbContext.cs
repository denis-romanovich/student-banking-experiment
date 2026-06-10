using Microsoft.EntityFrameworkCore;
using InternetBank.Web.Models;

namespace InternetBank.Web.Data
{
    public class BankDbContext : DbContext
    {
        public BankDbContext(DbContextOptions<BankDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
    }
}