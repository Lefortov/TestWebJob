using System.Collections.Generic;
using System.Linq;
using TestWebJob.Models;

namespace TestWebJob.CQRS.Commands
{
    public class AddPriceRecordCommand
    {
        private readonly WebJobContext _dbContext;

        public AddPriceRecordCommand(WebJobContext dbContext)
        {
            _dbContext = dbContext;
        }
        
        public void Execute(List<FuelPrice> prices)
        {
            foreach (var price in prices)
            {
                if (_dbContext.FuelPrices.Count(a => a.Date.Equals(price.Date)) == 0)
                {
                    _dbContext.FuelPrices.Add(price);
                }
            }

            _dbContext.SaveChanges();
        }
    }
}