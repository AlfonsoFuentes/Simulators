using Simulator.Shared.Models.HCs.SimulationPlanneds;
using Web.Infrastructure.Managers.Generic;

namespace Web.Infrastructure.Validators.FinishinLines.SimulationPlanneds
{

    public class SimulationPlannedValidator : AbstractValidator<SimulationPlannedDTO>
    {
        private readonly IGenericService Service;

        public SimulationPlannedValidator(IGenericService service)
        {
            Service = service;

            RuleFor(x => x.Name).NotEmpty().WithMessage("Name must be defined!");
            RuleFor(x => x.InitDate).NotNull().WithMessage("Init date must be defined!");
            RuleFor(x => x.InitSpam).NotNull().WithMessage("Init Hour must be defined!");
            RuleFor(x => x.Hours).NotEqual(0).WithMessage("Hours must be defined!");
         
            RuleFor(x => x.PlannedLines.Count).NotEqual(0).WithMessage("Lines must be defined!");
            RuleFor(x => x.Name).MustAsync(ReviewIfNameExist)
                .When(x => !string.IsNullOrEmpty(x.Name))
                .WithMessage(x => $"{x.Name} already exist");



        }

        async Task<bool> ReviewIfNameExist(SimulationPlannedDTO request, string name, CancellationToken cancellationToken)
        {
            ValidateSimulationPlannedNameRequest validate = new()
            {
                Name = name,


                Id = request.Id

            };
            var result = await Service.Validate(validate);
            return !result;
        }

    }
}
