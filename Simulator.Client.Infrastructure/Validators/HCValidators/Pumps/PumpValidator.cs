using FluentValidation;
using Simulator.Client.Infrastructure.ExtensionMethods;
using Simulator.Client.Infrastructure.Managers.ClientCRUDServices;
using Simulator.Shared.Models.HCs.Pumps;
using Web.Infrastructure.Managers.Generic;

namespace Web.Infrastructure.Validators.FinishinLines.Pumps
{

    public class PumpValidator : AbstractValidator<PumpDTO>
    {
        private readonly IClientCRUDService Service;

        public PumpValidator(IClientCRUDService service)
        {
            Service = service;

            RuleFor(x => x.Name).NotEmpty().WithMessage("Name must be defined!");


            RuleFor(x => x.Name).MustBeUnique(service, x => x.Name)
          .WithMessage(x => $"{x.Name} already exists");

            RuleFor(x => x.FlowValue).NotEqual(0).WithMessage("Flow must be defined!");

        }

      
        
    }
}
