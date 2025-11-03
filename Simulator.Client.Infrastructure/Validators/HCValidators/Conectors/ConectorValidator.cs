using FluentValidation;
using Simulator.Client.Infrastructure.Managers.ClientCRUDServices;
using Simulator.Shared.Enums.HCEnums.Enums;
using Simulator.Shared.Models.HCs.Conectors;
using Simulator.Shared.Models.HCs.Washouts;
using Web.Infrastructure.Managers.Generic;

namespace Web.Infrastructure.Validators.FinishinLines.Conectors
{

    public class InletConectorValidator : AbstractValidator<InletConnectorDTO>
    {
        private readonly IClientCRUDService Service;

        public InletConectorValidator(IClientCRUDService service)
        {
            Service = service;

            RuleFor(x => x.Froms.Count).NotEqual(0).WithMessage("From equipment must be defined!");


            RuleFor(x => x.From)
        .MustAsync((dto, _, ct) => ValidateCombination(dto, ct))
        .When(x => x.From != null && x.To != null).WithMessage(x => $"Connection -> To: {x.To!.Name} already exist");


        }


        private async Task<bool> ValidateCombination(InletConnectorDTO dto, CancellationToken ct)
        {
            dto.ValidationKey = InletConnectorDTO.ConnectorReview;
            var result = await Service.Validate(dto);
            return result.Succeeded;
        }

    }
    public class OutletConectorValidator : AbstractValidator<OutletConnectorDTO>
    {
        private readonly IClientCRUDService Service;

        public OutletConectorValidator(IClientCRUDService service)
        {
            Service = service;


            RuleFor(x => x.Tos.Count).NotEqual(0).WithMessage("To equipment must be defined!");

            RuleFor(x => x.To)
        .MustAsync((dto, _, ct) => ValidateCombination(dto, ct))
        .When(x => x.From != null && x.To != null).WithMessage(x => $"Connection -> from: {x.From!.Name} already exist");

          


        }

        private async Task<bool> ValidateCombination(OutletConnectorDTO dto, CancellationToken ct)
        {
            dto.ValidationKey = OutletConnectorDTO.ConnectorReview;
            var result = await Service.Validate(dto);
            return result.Succeeded;
        }


    }
}
