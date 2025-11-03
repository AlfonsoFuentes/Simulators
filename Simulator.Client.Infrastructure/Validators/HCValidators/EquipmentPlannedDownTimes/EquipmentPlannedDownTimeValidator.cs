using Simulator.Client.Infrastructure.Managers.ClientCRUDServices;
using Simulator.Shared.Models.HCs.EquipmentPlannedDownTimes;
using Web.Infrastructure.Managers.Generic;

namespace Web.Infrastructure.Validators.FinishinLines.EquipmentPlannedDownTimes
{

    public class EquipmentPlannedDownTimeValidator : AbstractValidator<EquipmentPlannedDownTimeDTO>
    {
        private readonly IClientCRUDService Service;

        public EquipmentPlannedDownTimeValidator(IClientCRUDService service)
        {
            Service = service;

            RuleFor(x => x.StartTime).NotNull().WithMessage("Start time must be defined!");
            RuleFor(x => x.EndTime).NotNull().WithMessage("End time must be defined!");

            RuleFor(x => x.EndTime).GreaterThanOrEqualTo(x=>x.StartTime).When(x => x.EndTime != null && x.StartTime != null).WithMessage("End time must be greater than start time!");

        }


    }
}
