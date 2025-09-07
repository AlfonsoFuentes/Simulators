using Simulator.Shared.Models.HCs.ContinuousSystems;
using UnitSystem;
using Web.Infrastructure.Managers.Generic;

namespace Web.Infrastructure.Validators.FinishinLines.ContinuousSystems
{

    public class ContinuousSystemValidator : AbstractValidator<ContinuousSystemDTO>
    {
        private readonly IGenericService Service;

        public ContinuousSystemValidator(IGenericService service)
        {
            Service = service;

            RuleFor(x => x.Name).NotEmpty().WithMessage("Name must be defined!");
           

            RuleFor(x => x.FlowValue).NotEqual(0).WithMessage("Mass flow must be defined!");


            RuleFor(x => x.Name).MustAsync(ReviewIfNameExist)
                .When(x => !string.IsNullOrEmpty(x.Name))
                .WithMessage(x => $"{x.Name} already exist");

           

        }

        async Task<bool> ReviewIfNameExist(ContinuousSystemDTO request, string name, CancellationToken cancellationToken)
        {
            ValidateContinuousSystemNameRequest validate = new()
            {
                Name = name,

                MainProcessId = request.MainProcessId,
                Id = request.Id

            };
            var result = await Service.Validate(validate);
            return !result;
        }
       
    }
}
