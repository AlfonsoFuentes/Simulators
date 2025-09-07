using Blazored.LocalStorage;
using Polly;
using Polly.Extensions.Http;
using Polly.Retry;
using System.Net;
using System.Net.Http.Json;

namespace Simulator.Client.Services.CurrencyServices
{

    public interface INewCurrency
    {
        Task<ConversionRate> GetRates(DateTime date);
    }
    public class NewCurrency : INewCurrency
    {
        private HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;
        private const string CurrencyAPI_KEY = "api_key=fxr_live_0717d3622fa4420ae78842bc55544a8b97b9";
        private const string URL = $"https://api.fxratesapi.com/historical?{CurrencyAPI_KEY}";
        private const string @base = "&base=USD&currencies=EUR,COP&resolution=1d&amount=1&places=3&format=json";
        //private readonly ILogger<NewCurrency> _logger;
        private DateTime CurrentDate { get; set; }
        private string monthstring => CurrentDate.Month < 10 ? $"0{CurrentDate.Month}" : $"{CurrentDate.Month}";
        private string daystring => CurrentDate.Day < 10 ? $"0{CurrentDate.Day}" : $"{CurrentDate.Day}";
        private string yearString => $"{CurrentDate.Year}";
        private string datestring => $"&date={yearString}-{monthstring}-{daystring}";
        private string URLHistorical => $"{URL}{datestring}{@base}";
        private AsyncRetryPolicy<HttpResponseMessage> _retryPolicy;
        public NewCurrency(IHttpClientFactory httpClientFactory, ILocalStorageService _localStorage/*, ILogger<NewCurrency> logger*/)
        {
            _httpClient = httpClientFactory.CreateClient("API2");
            _retryPolicy = HttpPolicyExtensions.HandleTransientHttpError()
                .WaitAndRetryAsync(retryCount: 1,
                sleepDurationProvider: attempt => TimeSpan.FromSeconds(0.5 * attempt),
                onRetry: (outcome, timeSpan, retryAttempt, context) =>
                {
                    if (outcome.Result == null || outcome.Result?.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        
                    }
                });
            this._localStorage = _localStorage;
            //_logger = logger;
        }
        public async Task<ConversionRate> GetRates(DateTime date)
        {
            var cachedKey = $"cached_rate-{date:yyyy-MM-dd}";

            try
            {
                //_logger.LogInformation("Buscando tasas en caché para la fecha {Date}", date);

                var hasCache = await _localStorage.ContainKeyAsync(cachedKey);
                if (hasCache)
                {
                    var cachedData = await _localStorage.GetItemAsync<CachedRate>(cachedKey);
                    if (cachedData!.Date.Date == date.Date)
                    {
                        //_logger.LogInformation("Tasas encontradas en caché para la fecha {Date}", date);
                        return cachedData.Rates;
                    }

                    //_logger.LogWarning("Caché encontrado pero expirado para la fecha {Date}. Eliminando...", date);
                    await _localStorage.RemoveItemAsync(cachedKey);
                }

                //_logger.LogInformation("No se encontraron tasas en caché. Obteniendo desde la API...");

                var freshRates = await GetFromAPI(date);
                var cacheEntry = new CachedRate
                {
                    Date = date,
                    Rates = freshRates
                };

                await _localStorage.SetItemAsync(cachedKey, cacheEntry);
                //_logger.LogInformation("Tasas almacenadas en caché para la fecha {Date}", date);

                return freshRates;
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Error al obtener las tasas para la fecha {Date}", date);
                string message = ex.Message;
                //throw;
            }
            return new ConversionRate
            {
                COP = 4000,
                EUR = 1
            };
        }
       

        public class CachedRate
        {
            public DateTime Date { get; set; }
            public ConversionRate Rates { get; set; } = new();
        }
        public async Task<ConversionRate> GetFromAPI(DateTime date)
        {
            CurrentDate = date;

            try
            {
                //_logger.LogInformation("Consultando tasas desde API para la fecha {Date} en URL: {Url}", date, URLHistorical);

                var response = await GetAsync(URLHistorical);

                if (!response.IsSuccessStatusCode)
                {
                    //_logger.LogWarning("La API respondió con código {StatusCode} para la fecha {Date}", response.StatusCode, date);
                    return new ConversionRate { COP = 4000, EUR = 1 };
                }

                var result = await response.ToObject<API_Obj>();

                if (result == null || !result.success)
                {
                    //_logger.LogWarning("Respuesta inválida o no exitosa de la API para la fecha {Date}", date);
                    return new ConversionRate { COP = 4000, EUR = 1 };
                }

                //_logger.LogInformation("Tasas obtenidas exitosamente desde API para la fecha {Date}: COP={COP}, EUR={EUR}",
                    //date, result.rates.COP, result.rates.EUR);

                return new ConversionRate
                {
                    COP = result.rates.COP,
                    EUR = result.rates.EUR
                };
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                //_logger.LogError(ex, "Excepción al llamar a la API para la fecha {Date}", date);
                return new ConversionRate { COP = 4000, EUR = 1 }; // Fallback
            }
        }
        public async Task<HttpResponseMessage> GetAsync(string url, CancellationToken cancellationToken = default)
        {
            var HttpResponse = await _retryPolicy.ExecuteAsync(
                async () =>
                {
                    var httpReponse = await _httpClient.GetAsync(url, cancellationToken);
                    return httpReponse;
                });
            HttpResponse.EnsureSuccessStatusCode();
            return HttpResponse;

        }
    }
    public class API_Obj
    {

        public bool success { get; set; }
        public string terms { get; set; } = string.Empty;
        public string privacy { get; set; } = string.Empty;
        public int timestamp { get; set; }
        public DateTime date { get; set; }
        public string @base { get; set; } = string.Empty;
        public ConversionRate rates { get; set; } = null!;
    }

    public class ConversionRate
    {
        public double COP { get; set; }
        public double EUR { get; set; }

    }
    public static class DependencyContainer
    {
        public static IServiceCollection CurrencyService(
            this IServiceCollection services)
        {
            services.AddScoped<INewCurrency, NewCurrency>();



            return services;
        }

    }
}
