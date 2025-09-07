using FluentValidation;
using Simulator.Shared.Enums.HCEnums.Enums;
using Simulator.Shared.Models.HCs.Tanks;
using Web.Infrastructure.Managers.Generic;

namespace Web.Infrastructure.Validators.FinishinLines.Tanks
{

    public class TankValidator : AbstractValidator<TankDTO>
    {
        private readonly IGenericService Service;

        public TankValidator(IGenericService service)
        {
            Service = service;

            RuleFor(x => x.Name).NotEmpty().WithMessage("Name must be defined!");
            RuleFor(x => x.CapacityValue).NotEqual(0).WithMessage("Capacity must be defined!");
            RuleFor(x => x.MaxLevelValue).NotEqual(0).WithMessage("Max Level must be defined!");
            RuleFor(x => x.LoLoLevelValue).NotEqual(0).WithMessage("Lo-Lo Level must be defined!");
            RuleFor(x => x.MinLevelValue).NotEqual(0).WithMessage("Min Level must be defined!");
            RuleFor(x => x.FluidStorage).NotEqual(FluidToStorage.None).WithMessage("Fluid to Storage must be defined!");

            RuleFor(x => x.TankCalculationType).NotEqual(TankCalculationType.None).WithMessage("Tank Calculation type must be defined!");
            RuleFor(x => x.Name).MustAsync(ReviewIfNameExist)
                .When(x => !string.IsNullOrEmpty(x.Name))
                .WithMessage(x => $"{x.Name} already exist");

     

        }

        async Task<bool> ReviewIfNameExist(TankDTO request, string name, CancellationToken cancellationToken)
        {
            ValidateTankNameRequest validate = new()
            {
                Name = name,

                MainProcessId=request.MainProcessId,
                Id = request.Id

            };
            var result = await Service.Validate(validate);
            return !result;
        }
       
    }
}
