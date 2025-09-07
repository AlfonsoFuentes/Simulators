using Simulator.Shared.Commons;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Simulator.Client.Infrastructure.ExtensionMethods;

public static class ResultExtensions
{
    public static readonly JsonSerializerOptions DefaultOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        ReferenceHandler = ReferenceHandler.Preserve,
        DefaultBufferSize = 128 // Ajusta según tus necesidades
    };
    

    public static async Task<IResult<T>> ToResult<T>(this HttpResponseMessage response)
    {
        var responseAsString = await response.Content.ReadAsStringAsync();

        try
        {
            var responseObject = JsonSerializer.Deserialize<Result<T>>(responseAsString, DefaultOptions);
            return responseObject!;
        }
        catch (Exception ex)
        {
            string exm = ex.Message;
        }
        return Result<T>.Fail("Something went wrong in last operation");
    }

    public static async Task<IResult> ToResult(this HttpResponseMessage response)
    {
        var responseAsString = await response.Content.ReadAsStringAsync();
        try
        {
            var responseObject = JsonSerializer.Deserialize<Result>(responseAsString,DefaultOptions);
            return responseObject!;
        }
        catch (Exception ex)
        {
            string exm = ex.Message;
        }
        return Result.Fail("Something went wrong in last operation");

    }


    public static async Task<T> ToObject<T>(this HttpResponseMessage response)
    {
        var responseAsString = await response.Content.ReadAsStringAsync();
        try
        {
            var responseObject = JsonSerializer.Deserialize<T>(responseAsString, DefaultOptions);
            return responseObject!;
        }
        catch (Exception ex)
        {
            string exm = ex.Message;
        }
        return default!;
    }
}



