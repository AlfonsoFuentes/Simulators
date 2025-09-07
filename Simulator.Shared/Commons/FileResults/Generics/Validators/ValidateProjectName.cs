using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulator.Shared.Commons.FileResults.Generics.Validators
{
    public record ValidateName<T>(T Data) where T : class;
}
