using Microsoft.EntityFrameworkCore;

namespace TestWebJob.Models
{
    public class WebJobContext : DbContext
    {
        public WebJobContext(DbContextOptions<WebJobContext> options)
            : base(options)
        { }
        
        public DbSet<FuelPrice> FuelPrices { get; set; }
    }
}