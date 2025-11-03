using Simulator.Shared.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulator.Client.Infrastructure.Managers.ClientCRUDServices
{
    public interface ISnackBarService2
    {
        void ShowError(string message);
        void ShowMessage(IResult result);
        void ShowSuccess(string message);
    }
}
