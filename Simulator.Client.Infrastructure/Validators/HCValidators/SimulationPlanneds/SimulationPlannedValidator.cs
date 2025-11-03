using Simulator.Client.Infrastructure.ExtensionMethods;
using Simulator.Client.Infrastructure.Managers.ClientCRUDServices;
using Simulator.Shared.Models.HCs.SimulationPlanneds;
using Web.Infrastructure.Managers.Generic;

namespace Web.Infrastructure.Validators.FinishinLines.SimulationPlanneds
{

    public class SimulationPlannedValidator : AbstractValidator<SimulationPlannedDTO>
    {
        private readonly IClientCRUDService Service;

        public SimulationPlannedValidator(IClientCRUDService service)
        {
            Service = service;

            RuleFor(x => x.Name).NotEmpty().WithMessage("Name must be defined!");
            RuleFor(x => x.InitDate).NotNull().WithMessage("Init date must be defined!");
            RuleFor(x => x.InitSpam).NotNull().WithMessage("Init Hour must be defined!");
            RuleFor(x => x.Hours).NotEqual(0).WithMessage("Hours must be defined!");

            RuleFor(x => x.PlannedLines.Count).NotEqual(0).WithMessage("Lines must be defined!");
            RuleFor(x => x.Name).MustBeUnique(service, x => x.Name)
      .WithMessage(x => $"{x.Name} already exists");
        }

       

    }
}
