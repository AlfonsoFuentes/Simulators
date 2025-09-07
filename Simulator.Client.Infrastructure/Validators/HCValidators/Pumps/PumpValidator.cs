using Simulator.Shared.Models.HCs.Pumps;
using Web.Infrastructure.Managers.Generic;

namespace Web.Infrastructure.Validators.FinishinLines.Pumps
{

    public class PumpValidator : AbstractValidator<PumpDTO>
    {
        private readonly IGenericService Service;

        public PumpValidator(IGenericService service)
        {
            Service = service;

            RuleFor(x => x.Name).NotEmpty().WithMessage("Name must be defined!");
            


            RuleFor(x => x.Name).MustAsync(ReviewIfNameExist)
                .When(x => !string.IsNullOrEmpty(x.Name))
                .WithMessage(x => $"{x.Name} already exist");

            RuleFor(x => x.FlowValue).NotEqual(0).WithMessage("Flow must be defined!");

        }

        async Task<bool> ReviewIfNameExist(PumpDTO request, string name, CancellationToken cancellationToken)
        {
            ValidatePumpNameRequest validate = new()
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
