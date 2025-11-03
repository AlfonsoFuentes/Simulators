using MudBlazor;
using Simulator.Client.Infrastructure.Managers.ClientCRUDServices;
using Simulator.Client.Services.Identities.Accounts;
using static MudBlazor.Defaults.Classes;

namespace Simulator.Client.Services.SnackBars
{
    public interface ISnackBarService : IManagetAuth
    {
        void ShowError(string message);
        void ShowError(List<string> message);
        void ShowSuccess(string message);
        void ShowSuccess(List<string> message);
    }
    public class SnackBarService : ISnackBarService
    {
        ISnackbar _snackBar;

        public SnackBarService(ISnackbar _snackBar)
        {
            this._snackBar = _snackBar;
            this._snackBar.Configuration.PositionClass = Defaults.Classes.Position.TopRight;
        }

        public void ShowSuccess(string message)
        {
            _snackBar.Add(message, Severity.Success);
        }

        public void ShowSuccess(List<string> message)
        {
            foreach (var item in message)
            {
                ShowSuccess(item);
            }
        }
        public void ShowError(string message)
        {
            _snackBar.Add(message, Severity.Error);
        }

        public void ShowError(List<string> message)
        {
            foreach (var item in message)
            {
                ShowError(item);
            }
        }
    }
    public class SnackBarService2 : ISnackBarService2
    {
        private readonly ISnackbar _mudSnackbar;

        public SnackBarService2(ISnackbar _snackBar)
        {
            this._mudSnackbar = _snackBar;
            this._mudSnackbar.Configuration.PositionClass = Defaults.Classes.Position.TopRight;
            this._mudSnackbar.Configuration.HideTransitionDuration = 2000;
            this._mudSnackbar.Configuration.NewestOnTop = true;
            this._mudSnackbar.Configuration.SnackbarVariant = Variant.Outlined;
            this._mudSnackbar.Configuration.BackgroundBlurred = true;
        }

        public void ShowMessage(IResult result)
        {
            if (result.Succeeded)
            {
                var message = result.Messages.Count > 0
                    ? string.Join(" ", result.Messages)
                    : "Operation completed successfully.";
                ShowSuccess(message);
            }
            else
            {
                var message = result.Messages.Count > 0
                    ? string.Join("\n", result.Messages)
                    : "An error occurred.";
                ShowError(message);
            }
        }
        public void ShowError(string message)
        {
            _mudSnackbar.Add(message, Severity.Error);
        }
        public void ShowSuccess(string message)
        {
            _mudSnackbar.Add(message, Severity.Success);
        }
    }
}
