using Microsoft.AspNetCore.Components;
using Polly;
using Polly.Extensions.Http;
using Polly.Retry;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Net;
using System.Net.Http.Json;

namespace Web.Infrastructure.Services.Client;
public interface IHttpClientService
{
    Task<HttpResponseMessage> GetAsync(string url, CancellationToken cancellationToken = default);
    Task<HttpResponseMessage> PostAsJsonAsync<T>(string url, T request, CancellationToken cancellationToken = default);


}

