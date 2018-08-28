using System;
using System.Collections.Generic;
using System.Linq;
using Castle.Components.DictionaryAdapter;
using Microsoft.EntityFrameworkCore;
using Moq;
using TestWebJob.CQRS.Commands;
using TestWebJob.CQRS.Queries;
using TestWebJob.Models;
using Xunit;

namespace TestWebJob.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void Get_Prices_Successfully()
        {
            var query = new FuelPricesQuery();
            var res = query.ExecuteAsync().Result;
            Assert.NotEmpty(res);
        }

        public void Insert_Prices_Successfully()
        {
            try
            {
                IQueryable<FuelPrice> fuelPrices = new List<FuelPrice>
                {
                    new FuelPrice
                    {
                        Date = DateTime.Now.AddDays(-1),
                        Price = 5
                    },
                    new FuelPrice
                    {
                        Date = DateTime.Now.AddDays(-2),
                        Price = 3
                    },
                }.AsQueryable();
                var mockSet = new Mock<DbSet<FuelPrice>>();
                mockSet.As<IQueryable<FuelPrice>>().Setup(m => m.Provider).Returns(fuelPrices.Provider);
                mockSet.As<IQueryable<FuelPrice>>().Setup(m => m.Expression).Returns(fuelPrices.Expression);
                mockSet.As<IQueryable<FuelPrice>>().Setup(m => m.ElementType).Returns(fuelPrices.ElementType);
                mockSet.As<IQueryable<FuelPrice>>().Setup(m => m.GetEnumerator()).Returns(fuelPrices.GetEnumerator);
            
                var mockContext = new Mock<WebJobContext>();
                mockContext.Setup(c => c.FuelPrices).Returns(mockSet.Object);
            
                var command = new AddPriceRecordCommand(mockContext.Object);
                command.Execute(new EditableList<FuelPrice>()
                {
                    new FuelPrice()
                    {
                        Date = DateTime.Now,
                        Price = 1
                    }
                });
            }
            catch (Exception e)
            {
                Assert.Null(e);
            }
        }
    }
}