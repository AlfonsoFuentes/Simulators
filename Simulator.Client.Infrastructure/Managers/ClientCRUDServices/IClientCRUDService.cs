using Microsoft.Extensions.Logging;
using Simulator.Client.Infrastructure.ExtensionMethods;
using Simulator.Shared.Commons;
using Simulator.Shared.Intefaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.Infrastructure.Services.Client;

namespace Simulator.Client.Infrastructure.Managers.ClientCRUDServices
{
    public interface IClientCRUDService
    {
        Task<IResult> Save<T>(T dto) where T : IDto; // o IDto
        Task<IResult> Delete<T>(T dto) where T : IDto;
        Task<IResult<List<T>>> GetAll<T>(T filter) where T : IDto;
        Task<IResult<T>> GetById<T>(T dto) where T : IDto;
        Task<IResult> Validate<T>(T dto) where T : IDto, IValidationRequest;
        Task<IResult> OrderDown<T>(T dto) where T : IDto;
        Task<IResult> OrderUp<T>(T dto) where T : IDto;
        Task<IResult> DeleteGroup<T>(List<T> dtos) where T : IDto;
    }

    public class ClientCRUDService : IClientCRUDService
    {
        private readonly IHttpClientService _http;
        private readonly ISnackBarService2 _snackBar;
        private readonly ILogger<ClientCRUDService> _logger; // 👈 nuevo
        public ClientCRUDService(IHttpClientService http, ISnackBarService2 snackBar, ILogger<ClientCRUDService> logger)
        {
            _http = http;
            _snackBar = snackBar;
            _logger = logger;
        }



        public async Task<IResult> Save<T>(T dto) where T : IDto
        {
            var entityName = typeof(T).Name;
            var endpoint = $"api/save/{entityName}";

            _logger.LogInformation("Starting Save operation for {EntityName} with Id: {Id}", entityName, dto.Id);

            try
            {
                var response = await _http.PostAsJsonAsync(endpoint, dto);
                var result = await response.ToResult();

                if (result.Succeeded)
                {
                    _logger.LogInformation("Successfully saved {EntityName} with Id: {Id}", entityName, dto.Id);
                }
                else
                {
                    _logger.LogWarning("Save failed for {EntityName} with Id: {Id}. Errors: {Errors}",
                        entityName, dto.Id, string.Join("; ", result.Messages));
                }

                _snackBar.ShowMessage(result);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception during Save for {EntityName} with Id: {Id}", entityName, dto.Id);
                var errorResult = Result.Fail("A technical error occurred. Please try again.");
                _snackBar.ShowMessage(errorResult);
                return errorResult;
            }
        }

        public async Task<IResult> Delete<T>(T dto) where T : IDto
        {
            var entityName = typeof(T).Name;
            var endpoint = $"api/delete/{entityName}";

            _logger.LogInformation("Starting Delete operation for {EntityName} with Id: {Id}", entityName, dto.Id);

            try
            {
                var response = await _http.PostAsJsonAsync(endpoint, dto);
                var result = await response.ToResult();

                if (result.Succeeded)
                {
                    _logger.LogInformation("Successfully deleted {EntityName} with Id: {Id}", entityName, dto.Id);
                }
                else
                {
                    _logger.LogWarning("Delete failed for {EntityName} with Id: {Id}. Errors: {Errors}",
                        entityName, dto.Id, string.Join("; ", result.Messages));
                }

                _snackBar.ShowMessage(result);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception during Delete for {EntityName} with Id: {Id}", entityName, dto.Id);
                var errorResult = Result.Fail("A technical error occurred. Please try again.");
                _snackBar.ShowMessage(errorResult);
                return errorResult;
            }
        }

        public async Task<IResult<List<T>>> GetAll<T>(T dto) where T : IDto
        {
            var entityName = typeof(T).Name;
            var endpoint = $"api/getall/{entityName}";

            try
            {
                var response = await _http.PostAsJsonAsync(endpoint, dto);
                var result = await response.ToResult<List<T>>();

                if (!result.Succeeded)
                {
                    _logger.LogWarning("GetAll failed for {EntityName}. Errors: {Errors}",
                        entityName, string.Join("; ", result.Messages));
                    _snackBar.ShowMessage(result);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception during GetAll for {EntityName}", entityName);
                var errorResult = Result<List<T>>.Fail("A technical error occurred. Please try again.");
                _snackBar.ShowMessage(errorResult);
                return errorResult;
            }
        }

        public async Task<IResult<T>> GetById<T>(T dto) where T : IDto
        {
            var entityName = typeof(T).Name;
            var endpoint = $"api/getbyid/{entityName}";
            var response = await _http.PostAsJsonAsync(endpoint, dto);
            var result = await response.ToResult<T>();
            if (!result.Succeeded)
                _snackBar.ShowMessage(result);
            return result;
        }
        public async Task<IResult> DeleteGroup<T>(List<T> dtos) where T : IDto
        {
            var entityName = typeof(T).Name;
            var endpoint = $"api/deletegroup/{entityName}";

            _logger.LogInformation("Starting DeleteGroup operation for {EntityName} with {Count} items", entityName, dtos?.Count);

            try
            {
                var response = await _http.PostAsJsonAsync(endpoint, dtos);
                var result = await response.ToResult();

                if (result.Succeeded)
                {
                    _logger.LogInformation("Successfully deleted {Count} {EntityName}(s)", dtos?.Count, entityName);
                }
                else
                {
                    _logger.LogWarning("DeleteGroup failed for {EntityName}. Errors: {Errors}",
                        entityName, string.Join("; ", result.Messages));
                }

                _snackBar.ShowMessage(result);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception during DeleteGroup for {EntityName}", entityName);
                var errorResult = Result.Fail("A technical error occurred. Please try again.");
                _snackBar.ShowMessage(errorResult);
                return errorResult;
            }
        }
        public async Task<IResult> OrderUp<T>(T dto) where T : IDto
        {
            var entityName = typeof(T).Name;
            var endpoint = $"api/orderup/{entityName}";

            _logger.LogInformation("Starting OrderUp operation for {EntityName} with Id: {Id}", entityName, dto.Id);

            try
            {
                var response = await _http.PostAsJsonAsync(endpoint, dto);
                var result = await response.ToResult();

                if (result.Succeeded)
                {
                    _logger.LogInformation("Successfully moved {EntityName} up with Id: {Id}", entityName, dto.Id);
                }
                else
                {
                    _logger.LogWarning("OrderUp failed for {EntityName} with Id: {Id}. Errors: {Errors}",
                        entityName, dto.Id, string.Join("; ", result.Messages));
                }

                _snackBar.ShowMessage(result);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception during OrderUp for {EntityName} with Id: {Id}", entityName, dto.Id);
                var errorResult = Result.Fail("A technical error occurred. Please try again.");
                _snackBar.ShowMessage(errorResult);
                return errorResult;
            }
        }

        // 🔹 OrderDown
        public async Task<IResult> OrderDown<T>(T dto) where T : IDto
        {
            var entityName = typeof(T).Name;
            var endpoint = $"api/orderdown/{entityName}";

            _logger.LogInformation("Starting OrderDown operation for {EntityName} with Id: {Id}", entityName, dto.Id);

            try
            {
                var response = await _http.PostAsJsonAsync(endpoint, dto);
                var result = await response.ToResult();

                if (result.Succeeded)
                {
                    _logger.LogInformation("Successfully moved {EntityName} down with Id: {Id}", entityName, dto.Id);
                }
                else
                {
                    _logger.LogWarning("OrderDown failed for {EntityName} with Id: {Id}. Errors: {Errors}",
                        entityName, dto.Id, string.Join("; ", result.Messages));
                }

                _snackBar.ShowMessage(result);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception during OrderDown for {EntityName} with Id: {Id}", entityName, dto.Id);
                var errorResult = Result.Fail("A technical error occurred. Please try again.");
                _snackBar.ShowMessage(errorResult);
                return errorResult;
            }
        }
        public async Task<IResult> Validate<T>(T dto) where T : IDto, IValidationRequest
        {
            var entityName = typeof(T).Name;
            var endpoint = $"api/validate/{entityName}";

            try
            {
                var response = await _http.PostAsJsonAsync(endpoint, dto);
                var result = await response.ToResult();
                return result; // No mostrar en SnackBar (es para validación en formularios)
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception during Validate for {EntityName}", entityName);
                return Result.Fail("Validation failed due to a technical error.");
            }
        }
    }
}
