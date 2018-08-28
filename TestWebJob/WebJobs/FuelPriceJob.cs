using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using TestWebJob.CQRS.Commands;
using TestWebJob.CQRS.Queries;

namespace TestWebJob.WebJobs
{
    public class FuelPriceJob
    {
        private readonly AddPriceRecordCommand _command;
        private readonly FuelPricesQuery _query;
        
        public FuelPriceJob(AddPriceRecordCommand command, FuelPricesQuery query)
        {
            _command = command;
            _query = query;
        }

        [FunctionName("FuelPriceTimeTrigger")]
        public async void Run(TimerInfo timer, TraceWriter log)
        {
            try
            {
                if (timer.IsPastDue)
                    log.Info("Timer is running late");
                log.Info($"Running job at {DateTime.Now}");
                var prices = await _query.ExecuteAsync();
                log.Info($"Prices: {prices}");
                _command.Execute(prices);
                log.Info("Inserted successfully");
            }
            catch (Exception e)
            {
                log.Error($"Error: {e}");
            }
            finally
            {
                log.Info("Exiting Run method");
            }
        }
    }
}