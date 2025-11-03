using Simulator.Shared.Commons;
using Simulator.Shared.Commons.IdentityModels.Requests.Identity;
using Simulator.Shared.Commons.IdentityModels.Responses.Identity;


namespace Simulator.Server.Interfaces.Identity
{
    public interface IUserService
    {
        Task<Result<List<UserResponse>>> GetAllAsync();

        Task<int> GetCountAsync();

        Task<IResult<UserResponse>> GetAsync(string userId);

        Task<Shared.Commons.IResult> RegisterAsync(RegisterRequest request, string origin);

        Task<Shared.Commons.IResult> ToggleUserStatusAsync(ToggleUserStatusRequest request);

        Task<IResult<UserRolesResponse>> GetRolesAsync(string id);

        Task<Shared.Commons.IResult> UpdateRolesAsync(UpdateUserRolesRequest request);

        Task<IResult<string>> ConfirmEmailAsync(string userId, string code);

        Task<Shared.Commons.IResult> ForgotPasswordAsync(ForgotPasswordRequest request, string origin);

        Task<Shared.Commons.IResult> ResetPasswordAsync(ResetPasswordRequest request);

        //Task<string> ExportToExcelAsync(string searchString = "");
    }
}