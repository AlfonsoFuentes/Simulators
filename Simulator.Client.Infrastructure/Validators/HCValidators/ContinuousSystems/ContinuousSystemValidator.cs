using Simulator.Client.Infrastructure.ExtensionMethods;
using Simulator.Client.Infrastructure.Managers.ClientCRUDServices;
using Simulator.Shared.Models.HCs.ContinuousSystems;
using UnitSystem;
using Web.Infrastructure.Managers.Generic;

namespace Web.Infrastructure.Validators.FinishinLines.ContinuousSystems
{

    public class ContinuousSystemValidator : AbstractValidator<ContinuousSystemDTO>
    {
        private readonly IClientCRUDService Service;

        public ContinuousSystemValidator(IClientCRUDService service)
        {
            Service = service;

            RuleFor(x => x.Name).NotEmpty().WithMessage("Name must be defined!");
           

            RuleFor(x => x.FlowValue).NotEqual(0).WithMessage("Mass flow must be defined!");


            RuleFor(x => x.Name).MustBeUnique(service, x => x.Name)
        .WithMessage(x => $"{x.Name} already exists");



        }

       
       
    }
}
