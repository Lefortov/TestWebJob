using Microsoft.EntityFrameworkCore;

namespace TestWebJob.Models
{
    public class WebJobContext : DbContext
    {
        public DbSet<FuelPrice> FuelPrices { get; set; }
    }
}