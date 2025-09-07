using MudBlazor;
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
}
