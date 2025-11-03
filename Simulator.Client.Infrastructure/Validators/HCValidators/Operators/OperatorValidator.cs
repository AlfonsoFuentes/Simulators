using Simulator.Client.Infrastructure.ExtensionMethods;
using Simulator.Client.Infrastructure.Managers.ClientCRUDServices;
using Simulator.Shared.Models.HCs.Operators;
using Web.Infrastructure.Managers.Generic;

namespace Web.Infrastructure.Validators.FinishinLines.Operators
{

    public class OperatorValidator : AbstractValidator<OperatorDTO>
    {
        private readonly IClientCRUDService Service;

        public OperatorValidator(IClientCRUDService service)
        {
            Service = service;

            RuleFor(x => x.Name).NotEmpty().WithMessage("Name must be defined!");

            RuleFor(x => x.Name).MustBeUnique(service, x => x.Name)
        .WithMessage(x => $"{x.Name} already exists");



        }

       
       
    }
}
