using Simulator.Client.Infrastructure.Managers.ClientCRUDServices;
using Simulator.Shared.Enums.HCEnums.Enums;
using Simulator.Shared.Models.HCs.LinePlanneds;
using Web.Infrastructure.Managers.Generic;

namespace Web.Infrastructure.Validators.FinishinLines.LinePlanneds
{

    public class LinePlannedValidator : AbstractValidator<LinePlannedDTO>
    {
        private readonly IClientCRUDService Service;

        public LinePlannedValidator(IClientCRUDService service)
        {
            Service = service;

            RuleFor(x => x.ShiftType).NotEqual(ShiftType.None).WithMessage("Shift Type must be defined!");
            RuleFor(x => x.PlannedSKUDTOs.Count).NotEqual(0).WithMessage("SKU List  must be defined!");

          

        }

        
    }
}
