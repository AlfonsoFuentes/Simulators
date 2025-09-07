using Simulator.Client.Services.Extensions;

namespace Simulator.Client.Layout;
public partial class LoginDisplay
{
    [Parameter]
    public string CurrentUserId { get; set; } = string.Empty;


    private string FirstName { get; set; } = string.Empty;
    private string SecondName { get; set; } = string.Empty;
    private string PhoneNumber { get; set; } = string.Empty;
    private string Email { get; set; } = string.Empty;
    private string LetterOfName { get; set; } = string.Empty;
    protected override async Task OnParametersSetAsync()
    {
        await LoadDataAsync();
    }
    private async Task LoadDataAsync()
    {
        var state = await _stateProvider.GetAuthenticationStateAsync();
        var user = state.User;
        if (user == null) return;
        if (user.Identity?.IsAuthenticated == true)
        {
            CurrentUserId = user.GetUserId();
            FirstName = user.GetFirstName();
            SecondName = user.GetLastName();
            if (FirstName.Length > 0)
            {
                LetterOfName = $"{FirstName[0]}{SecondName[0]}";
            }

            Email = user.GetEmail();
            PhoneNumber = user.GetPhoneNumber();

            var currentUserResult = await _userManager.GetAsync(CurrentUserId);
            if (!currentUserResult.Succeeded || currentUserResult.Data == null)
            {
                _snackBar.ShowError(
                    "You are logged out because the user with your Token has been deleted.");
                CurrentUserId = string.Empty;

                FirstName = string.Empty;
                SecondName = string.Empty;
                Email = string.Empty;
                LetterOfName = string.Empty;
                PhoneNumber = string.Empty;
                await _authenticationManager.Logout();
            }
            StateHasChanged();
        }
    }
}
