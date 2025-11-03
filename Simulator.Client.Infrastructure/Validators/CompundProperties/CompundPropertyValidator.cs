using Simulator.Shared.Models.CompoundProperties;
using Simulator.Shared.NewModels.Compounds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulator.Client.Infrastructure.Validators.CompundProperties
{
    public class CompoundPropertyValidator : AbstractValidator<CompoundPropertyDTO>
    {
        public CompoundPropertyValidator()
        {

            RuleFor(x => x.Name).NotNull().WithMessage("Name must be defined!");

        }
    }
    public class NewCompoundPropertyValidator : AbstractValidator<NewCompoundPropertyDTO>
    {
        public NewCompoundPropertyValidator()
        {

            RuleFor(x => x.Name).NotNull().WithMessage("Name must be defined!");
            RuleFor(x => x).NotNull();
        }
    }
}
