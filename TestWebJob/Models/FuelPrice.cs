using System;
using System.ComponentModel.DataAnnotations;

namespace TestWebJob.Models
{
    public class FuelPrice
    {
        public DateTime Date { get; set; }
        public decimal Price { get; set; }
    }
}