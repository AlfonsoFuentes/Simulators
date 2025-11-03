using Simulator.Client.Infrastructure.ExtensionMethods;
using Simulator.Client.Infrastructure.Managers.ClientCRUDServices;
using Simulator.Shared.Models.HCs.StreamJoiners;
using Web.Infrastructure.Managers.Generic;

namespace Simulator.Client.Infrastructure.Validators.HCValidators.StreamJoiners
{
    public class StreamJoinerValidator : AbstractValidator<StreamJoinerDTO>
    {
        private readonly IClientCRUDService Service;

        public StreamJoinerValidator(IClientCRUDService service)
        {
            Service = service;

            RuleFor(x => x.Name).NotEmpty().WithMessage("Name must be defined!");



            RuleFor(x => x.Name).MustBeUnique(service, x => x.Name)
        .WithMessage(x => $"{x.Name} already exists");



        }

        

    }
}
