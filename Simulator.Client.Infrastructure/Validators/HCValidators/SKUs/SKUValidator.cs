using FluentValidation;
using Simulator.Client.Infrastructure.ExtensionMethods;
using Simulator.Client.Infrastructure.Managers.ClientCRUDServices;
using Simulator.Shared.Enums.HCEnums.Enums;
using Simulator.Shared.Models.HCs.SKUs;
using Web.Infrastructure.Managers.Generic;

namespace Web.Infrastructure.Validators.FinishinLines.SKUs
{

    public class SKUValidator : AbstractValidator<SKUDTO>
    {
        private readonly IClientCRUDService Service;

        public SKUValidator(IClientCRUDService service)
        {
            Service = service;

            RuleFor(x => x.Name).NotEmpty().WithMessage("Name must be defined!");
            RuleFor(x => x.SkuCode).NotEmpty().WithMessage("Code must be defined!");
            RuleFor(x => x.FocusFactory).NotEqual(FocusFactory.None).WithMessage("Focus Factory must be defined!");
            RuleFor(x => x.ProductCategory).NotEqual(ProductCategory.None).WithMessage("Category must be defined!");
            RuleFor(x => x.BackBone).NotNull().WithMessage("Back Bone must be defined!");
            RuleFor(x => x.SizeValue).NotEqual(0).WithMessage("SKU Size must be defined!");
            RuleFor(x => x.WeigthValue).NotEqual(0).WithMessage("Weight must be defined!");
            RuleFor(x => x.PackageType).NotEqual(PackageType.None).WithMessage("Package type must be defined!");
            RuleFor(x => x.EA_Case).NotEqual(0).WithMessage("EA/case must be defined!");

           

            RuleFor(x => x.Name).MustBeUnique(service, x => x.Name)
           .WithMessage("Name already exists");

            RuleFor(x => x.SkuCode).MustBeUnique(service, x => x.SkuCode)
         .WithMessage("SkuCode already exists");

        }

       
    }
}
