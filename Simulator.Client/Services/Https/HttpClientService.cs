


namespace Simulator.Client.Services.Https
{
    public class HttpClientService : IHttpClientService
    {

        private HttpClient _httpClient;
        private AsyncRetryPolicy<HttpResponseMessage> _retryPolicy;
        //ILogger Logger = null!;
        private NavigationManager _navigationManager;
        private ISnackBarService _snackBar;
        public HttpClientService(IHttpClientFactory httpClientFactory, NavigationManager navigationManager, ISnackBarService snackBar)
        {
            _navigationManager = navigationManager;
            _snackBar = snackBar;
            _httpClient = httpClientFactory.CreateClient("API");
            _retryPolicy = HttpPolicyExtensions.HandleTransientHttpError()
                .WaitAndRetryAsync(retryCount: 1,
                sleepDurationProvider: attempt => TimeSpan.FromSeconds(0.5 * attempt),
                onRetry: (outcome, timeSpan, retryAttempt, context) =>
                {
                    if (outcome.Result == null || outcome.Result?.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        _snackBar.ShowError("Token expired!! must register");
                        _navigationManager.NavigateTo("/logout");
                    }
                });

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
        public async Task<HttpResponseMessage> PostAsJsonAsync<T>(string url, T request, CancellationToken cancellationToken = default)
        {
            HttpResponseMessage result = new();
            try
            {

                result = await _retryPolicy.ExecuteAsync(
                       async () =>
                       {
                           var httpresult = await _httpClient.PostAsJsonAsync(url, request, cancellationToken);
                           return httpresult;
                       });

                result.EnsureSuccessStatusCode();


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                string message = ex.Message;
            }
            return result;

        }


    }



    public class HttpClientService2 : IHttpClientService
    {
        private readonly HttpClient _httpClient;
        private readonly AsyncRetryPolicy<HttpResponseMessage> _retryPolicy;
        private readonly NavigationManager _navigationManager;
        private readonly ISnackBarService _snackBar;
        private readonly ILogger<HttpClientService2> _logger;

        public HttpClientService2(
            IHttpClientFactory httpClientFactory,
            NavigationManager navigationManager,
            ISnackBarService snackBar,
            ILogger<HttpClientService2> logger)
        {
            _navigationManager = navigationManager;
            _snackBar = snackBar;
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient("API");

            _retryPolicy = HttpPolicyExtensions
                .HandleTransientHttpError()
                .WaitAndRetryAsync(
                    retryCount: 1,
                    sleepDurationProvider: attempt => TimeSpan.FromSeconds(0.5 * attempt),
                    onRetry: (outcome, timeSpan, retryAttempt, context) =>
                    {
                        _logger.LogWarning(
                            "Reintentando solicitud (intento {RetryAttempt}) tras error: {ExceptionMessage}",
                            retryAttempt,
                            outcome.Exception?.Message ?? outcome.Result?.StatusCode.ToString() ?? "Desconocido"
                        );
                    });
        }

        private void HandleUnauthorized()
        {
            _logger.LogWarning("Token expirado o no autorizado. Redirigiendo al logout.");
            _snackBar.ShowError("Tu sesión ha expirado. Por favor, vuelve a iniciar sesión.");

            // ✅ En Blazor cliente, NavigateTo es seguro si se llama desde UI (99% de los casos)
            // Si estás en Blazor Server y llamas desde un hilo no-UI, descomenta la siguiente línea:
            // _navigationManager.InvokeAsync(() => _navigationManager.NavigateTo("/logout", forceLoad: true));

            _navigationManager.NavigateTo("/logout", forceLoad: true);

            // Lanzamos excepción para detener el flujo
            throw new UnauthorizedAccessException("Redirigido a logout por token expirado.");
        }

        public async Task<HttpResponseMessage> GetAsync(string url, CancellationToken cancellationToken = default)
        {
            var response = await _retryPolicy.ExecuteAsync(async () =>
                await _httpClient.GetAsync(url, cancellationToken));

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                HandleUnauthorized(); // ← Sí, es void, pero lanza excepción
            }

            response.EnsureSuccessStatusCode();
            return response;
        }

        public async Task<HttpResponseMessage> PostAsJsonAsync<T>(string url, T request, CancellationToken cancellationToken = default)
        {
            var response = await _retryPolicy.ExecuteAsync(async () =>
                await _httpClient.PostAsJsonAsync(url, request, cancellationToken));

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                HandleUnauthorized();
            }

            response.EnsureSuccessStatusCode();
            return response;
        }
    }

}
