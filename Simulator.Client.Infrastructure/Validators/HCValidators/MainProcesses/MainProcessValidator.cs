using Simulator.Shared.Models.HCs.MainProcesss;
using Web.Infrastructure.Managers.Generic;

namespace Web.Infrastructure.Validators.FinishinLines.MainProcesss
{

    public class MainProcessValidator : AbstractValidator<MainProcessDTO>
    {
        private readonly IGenericService Service;

        public MainProcessValidator(IGenericService service)
        {
            Service = service;

            RuleFor(x => x.Name).NotEmpty().WithMessage("Name must be defined!");
           

            RuleFor(x => x.Name).MustAsync(ReviewIfNameExist)
                .When(x => !string.IsNullOrEmpty(x.Name))
                .WithMessage(x => $"{x.Name} already exist");

          

        }

        async Task<bool> ReviewIfNameExist(MainProcessDTO request, string name, CancellationToken cancellationToken)
        {
            ValidateMainProcessNameRequest validate = new()
            {
                Name = name,


                Id = request.Id

            };
            var result = await Service.Validate(validate);
            return !result;
        }
       
    }
}
