using Simulator.Client.Infrastructure.Managers.ClientCRUDServices;
using Simulator.Shared.Enums.HCEnums.Enums;
using Simulator.Shared.Models.HCs.MixerPlanneds;
using Web.Infrastructure.Managers.Generic;

namespace Web.Infrastructure.Validators.FinishinLines.MixerPlanneds
{

    public class MixerPlannedValidator : AbstractValidator<MixerPlannedDTO>
    {
        private readonly IClientCRUDService Service;

        public MixerPlannedValidator(IClientCRUDService service)
        {
            Service = service;

            RuleFor(x => x.MixerDTO).NotNull().WithMessage("Mixer must be defined!");
            RuleFor(x => x.BackBone).NotNull().WithMessage("Backbone must be defined!");

            RuleFor(x => x.CurrentMixerState).NotEqual(CurrentMixerState.None).WithMessage("Current MixerState Type must be defined!");
            RuleFor(x => x.BackBoneSteps.Count).NotEqual(0).When(x => x.CurrentMixerState == CurrentMixerState.Batching).WithMessage("Back Bone Steps must be defined!");

            RuleFor(x => x.ProducingTo).NotNull().WithMessage("Producing to must be defined!");
            RuleFor(x => x.CapacityValue).NotEqual(0).WithMessage("Capacity must be defined!");
            RuleFor(x => x.MixerLevelValue).NotEqual(0).WithMessage("Initial mixer level must be defined!");

        }


    }
}
