using Simulator.Shared.Models.HCs.BackBoneSteps;
using UnitSystem;
using Web.Infrastructure.Managers.Generic;

namespace Simulator.Client.Infrastructure.Validators.HCValidators.BackBones
{

    public class BackBoneStepValidator : AbstractValidator<BackBoneStepDTO>
    {
        private readonly IGenericService Service;

        public BackBoneStepValidator(IGenericService service)
        {
            Service = service;

            RuleFor(x => x.BackBoneStepType).NotEqual(Shared.Enums.HCEnums.Enums.BackBoneStepType.None).WithMessage("Type must be defined!");
            RuleFor(x => x.Percentage).NotEqual(0).When(x => x.BackBoneStepType == Shared.Enums.HCEnums.Enums.BackBoneStepType.Add).WithMessage("Percentage must be defined!");
            RuleFor(x => x.StepRawMaterial).NotNull().When(x => x.BackBoneStepType == Shared.Enums.HCEnums.Enums.BackBoneStepType.Add).WithMessage("Raw Material must be defined!");
            RuleFor(x => x.Time).NotEqual(new UnitSystem.Amount(0, TimeUnits.Minute)).When(x => x.BackBoneStepType == Shared.Enums.HCEnums.Enums.BackBoneStepType.Mixing).WithMessage("Time must be defined!");


            RuleFor(x => x.Time).NotEqual(new UnitSystem.Amount(0, TimeUnits.Minute)).When(x => x.BackBoneStepType == Shared.Enums.HCEnums.Enums.BackBoneStepType.Analisys).WithMessage("Time must be defined!");


            RuleFor(x => x.Time).NotEqual(new UnitSystem.Amount(0, TimeUnits.Minute)).When(x => x.BackBoneStepType == Shared.Enums.HCEnums.Enums.BackBoneStepType.Adjust).WithMessage("Time must be defined!");


            RuleFor(x => x.Time).NotEqual(new UnitSystem.Amount(0, TimeUnits.Minute)).When(x => x.BackBoneStepType == Shared.Enums.HCEnums.Enums.BackBoneStepType.Connect_Mixer_WIP).WithMessage("Time must be defined!");

            RuleFor(x => x.StepRawMaterial).NotNull().When(x => x.BackBoneStepType == Shared.Enums.HCEnums.Enums.BackBoneStepType.Washout).WithMessage("Washout material must be defined!");





        }

        async Task<bool> ReviewIfNameExist(BackBoneStepDTO request, string name, CancellationToken cancellationToken)
        {
            ValidateBackBoneStepNameRequest validate = new()
            {
                Name = name,


                Id = request.Id

            };
            var result = await Service.Validate(validate);
            return !result;
        }
    }
}
