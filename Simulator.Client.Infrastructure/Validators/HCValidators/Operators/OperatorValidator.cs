using Simulator.Shared.Models.HCs.Operators;
using Web.Infrastructure.Managers.Generic;

namespace Web.Infrastructure.Validators.FinishinLines.Operators
{

    public class OperatorValidator : AbstractValidator<OperatorDTO>
    {
        private readonly IGenericService Service;

        public OperatorValidator(IGenericService service)
        {
            Service = service;

            RuleFor(x => x.Name).NotEmpty().WithMessage("Name must be defined!");

            RuleFor(x => x.Name).MustAsync(ReviewIfNameExist)
                .When(x => !string.IsNullOrEmpty(x.Name))
                .WithMessage(x => $"{x.Name} already exist");

          

        }

        async Task<bool> ReviewIfNameExist(OperatorDTO request, string name, CancellationToken cancellationToken)
        {
            ValidateOperatorNameRequest validate = new()
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
