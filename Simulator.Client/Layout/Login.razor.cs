using Blazored.FluentValidation;
using Microsoft.AspNetCore.Components;
using Simulator.Shared.Commons.IdentityModels.Requests.Identity;

namespace Simulator.Client.Layout;
public partial class Login
{
    [CascadingParameter]
    public App MainApp { get; set; } = null!;
    private bool Validated { get; set; }
    FluentValidationValidator _fluentValidationValidator = null!;
    async Task ValidateAsync()
    {
        Validated = _fluentValidationValidator == null ? false : await _fluentValidationValidator.ValidateAsync(options => { options.IncludeAllRuleSets(); });
    }
    private TokenRequest _registerUserModel = new();

   
    private async Task SubmitAsync()
    {
        await ValidateAsync();
        if (!Validated) return;
        var result = await _authenticationManager.Login(_registerUserModel);
        if (!result.Succeeded)
        {
            _snackBar.ShowError(result.Messages);

        }
        else
        {
            MainApp.UserId = result.Data.UserId;    
            Navigation.NavigateTo("/");
        }

    }
}
