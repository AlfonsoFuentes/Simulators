using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Web.Infrastructure.Services.Currencies
{
    //public interface IRate
    //{
    //    Task<ConversionRate> GetRates(DateTime date);
    //}
    //public class Rates : IRate
    //{
    //    private readonly IHttpClientService _httpClient;
    //    private readonly ILocalStorageService _localStorage;
    //    private const string CurrencyAPI_KEY = "api_key=fxr_live_0717d3622fa4420ae78842bc55544a8b97b9";
    //    private const string URL = $"https://api.fxratesapi.com/historical?{CurrencyAPI_KEY}";
    //    private const string @base = "&base=USD&currencies=EUR,COP&resolution=1d&amount=1&places=3&format=json";

    //    private DateTime CurrentDate { get; set; }
    //    private string monthstring => CurrentDate.Month < 10 ? $"0{CurrentDate.Month}" : $"{CurrentDate.Month}";
    //    private string daystring => CurrentDate.Day < 10 ? $"0{CurrentDate.Day}" : $"{CurrentDate.Day}";
    //    private string yearString => $"{CurrentDate.Year}";
    //    private string datestring => $"&date={yearString}-{monthstring}-{daystring}";
    //    private string URLHistorical => $"{URL}{datestring}{@base}";
    //    public Rates(IHttpClientService httpClient, IConfiguration config)
    //    {
    //        _httpClient = httpClient;
    //    }
    //    public async Task<ConversionRate> GetRates(DateTime date)
    //    {
    //        var today = DateTime.UtcNow;

    //        if (date < today)
    //        {
    //            return await GetFromAPI(date);
    //        }

    //        var cacherate = await _localStorage.ContainKeyAsync("cached_rate");
    //        if (cacherate)
    //        {
    //            var cachedData = await _localStorage.GetItemAsync<CachedRate>("cached_rate");
    //            if (cachedData!.Date.Date == today.Date)
    //            {
    //                return cachedData.Rates;

    //            }

    //            await _localStorage.RemoveItemAsync("cached_rate");



    //        }
    //        var freshRates = await GetFromAPI(today);
    //        var cacheEntry = new CachedRate
    //        {
    //            Date = today,
    //            Rates = freshRates
    //        };

    //        await _localStorage.SetItemAsync("cached_rate", cacheEntry);



    //        return freshRates;
    //    }

    //    public class CachedRate
    //    {
    //        public DateTime Date { get; set; }
    //        public ConversionRate Rates { get; set; } = new();
    //    }

    //    public async Task<ConversionRate> GetFromAPI(DateTime date)
    //    {
    //        var rates = new ConversionRate()
    //        {
    //            COP = 4000,
    //            EUR = 1
    //        };
    //        CurrentDate = date;
    //        try
    //        {
    //            var response = await _httpClient.GetAsync(URLHistorical);
    //            if (response.IsSuccessStatusCode)
    //            {
    //                var result = await response.ToObject<API_Obj>();
    //                if (result.success)
    //                {
    //                    rates.COP = result.rates.COP;
    //                    rates.EUR = result.rates.EUR;
    //                }


    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            string message = ex.Message;
    //        }

    //        return rates;
    //    }
    //}
    //public class API_Obj
    //{

    //    public bool success { get; set; }
    //    public string terms { get; set; } = string.Empty;
    //    public string privacy { get; set; } = string.Empty;
    //    public int timestamp { get; set; }
    //    public DateTime date { get; set; }
    //    public string @base { get; set; } = string.Empty;
    //    public ConversionRate rates { get; set; } = null!;
    //}

    //public class ConversionRate
    //{
    //    public double COP { get; set; }
    //    public double EUR { get; set; }

    //}
    //public static class DependencyContainer
    //{
    //    public static IServiceCollection CurrencyService(
    //        this IServiceCollection services)
    //    {
    //        services.AddScoped<IRate, Rates>();



    //        return services;
    //    }

    //}
}
