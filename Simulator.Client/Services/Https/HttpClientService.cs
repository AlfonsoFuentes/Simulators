


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



   

}
