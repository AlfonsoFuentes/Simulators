using FluentValidation;
using Simulator.Client.Infrastructure.ExtensionMethods;
using Simulator.Client.Infrastructure.Managers.ClientCRUDServices;
using Simulator.Shared.Enums.HCEnums.Enums;
using Simulator.Shared.Models.HCs.Lines;
using UnitSystem;
using Web.Infrastructure.Managers.Generic;

namespace Web.Infrastructure.Validators.FinishinLines.Lines
{

    public class LineValidator : AbstractValidator<LineDTO>
    {
        private readonly IClientCRUDService Service;

        public LineValidator(IClientCRUDService service)
        {
            Service = service;

            RuleFor(x => x.Name).NotEmpty().WithMessage("Name must be defined!");
           
            RuleFor(x => x.PackageType).NotEqual(PackageType.None).WithMessage("Package Type must be defined!");
        

            RuleFor(x => x.TimeToReviewAUValue).NotEqual(0).WithMessage("Time to Review AU must be defined!");

            RuleFor(x => x.Name).MustBeUnique(service, x => x.Name)
        .WithMessage(x => $"{x.Name} already exists");



        }

      
       
    }
}
