using FluentValidation;
using Simulator.Shared.Enums.HCEnums.Enums;
using Simulator.Shared.Models.HCs.Lines;
using UnitSystem;
using Web.Infrastructure.Managers.Generic;

namespace Web.Infrastructure.Validators.FinishinLines.Lines
{

    public class LineValidator : AbstractValidator<LineDTO>
    {
        private readonly IGenericService Service;

        public LineValidator(IGenericService service)
        {
            Service = service;

            RuleFor(x => x.Name).NotEmpty().WithMessage("Name must be defined!");
           
            RuleFor(x => x.PackageType).NotEqual(PackageType.None).WithMessage("Package Type must be defined!");
        

            RuleFor(x => x.TimeToReviewAUValue).NotEqual(0).WithMessage("Time to Review AU must be defined!");

            RuleFor(x => x.Name).MustAsync(ReviewIfNameExist)
                .When(x => !string.IsNullOrEmpty(x.Name))
                .WithMessage(x => $"{x.Name} already exist");

           

        }

        async Task<bool> ReviewIfNameExist(LineDTO request, string name, CancellationToken cancellationToken)
        {
            ValidateLineNameRequest validate = new()
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
