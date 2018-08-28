using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using TestWebJob.Models;
using TestWebJob.Utils;

namespace TestWebJob.CQRS.Queries
{
    public class FuelPricesQuery
    {
        private readonly HttpClient _client;
        private readonly string _apiUrl =
            "http://api.eia.gov/series/?api_key=ec92aacd6947350dcb894062a4ad2d08&series_id=PET.EMD_EPD2D_PTE_NUS_DPG.W";

        private readonly int _firstDays;
        
        public FuelPricesQuery() 
        {
            _client = new HttpClient();
            var appSettings = ServiceLocator.GetService<IOptions<AppSettings>>().Value;
            _firstDays = appSettings.Filters.TakenDays;
        }

        public async Task<List<FuelPrice>> ExecuteAsync()
        {
            var req = new HttpRequestMessage(HttpMethod.Get, _apiUrl);
            var resp = await _client.SendAsync(req);
            
            var json = JObject.Parse(await resp.Content.ReadAsStringAsync());
            var series = json.SelectTokens("series");
            var prices = series.FirstOrDefault()
                .Values<FuelPrice>("data");
            var sortedPrices = prices.Take(_firstDays).ToList();
            
            return sortedPrices;
        }
    }
}