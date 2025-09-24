using Simulator.Shared.Models.HCs.PreferedMixers;
using Web.Infrastructure.Managers.Generic;

namespace Web.Infrastructure.Validators.FinishinLines.PreferedMixers
{

    public class PreferedMixerValidator : AbstractValidator<PreferedMixerDTO>
    {
        private readonly IGenericService Service;

        public PreferedMixerValidator(IGenericService service)
        {
            Service = service;

          
            RuleFor(x => x.Mixer).NotNull().WithMessage("Mixer must be defined!");
          
           
        }

       
    }
}
