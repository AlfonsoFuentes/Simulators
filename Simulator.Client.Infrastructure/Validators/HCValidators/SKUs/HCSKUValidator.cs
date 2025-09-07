using FluentValidation;
using Simulator.Shared.Enums.HCEnums.Enums;
using Simulator.Shared.Models.HCs.SKUs;
using Web.Infrastructure.Managers.Generic;

namespace Web.Infrastructure.Validators.FinishinLines.SKUs
{

    public class HCSKUValidator : AbstractValidator<SKUDTO>
    {
        private readonly IGenericService Service;

        public HCSKUValidator(IGenericService service)
        {
            Service = service;

            RuleFor(x => x.Name).NotEmpty().WithMessage("Name must be defined!");
            RuleFor(x => x.SkuCode).NotEmpty().WithMessage("Code must be defined!");

            RuleFor(x => x.ProductCategory).NotEqual(ProductCategory.None).WithMessage("Category must be defined!");
            RuleFor(x => x.BackBone).NotNull().WithMessage("Back Bone must be defined!");
            RuleFor(x => x.SizeValue).NotEqual(0).WithMessage("SKU Size must be defined!");
            RuleFor(x => x.WeigthValue).NotEqual(0).WithMessage("Weight must be defined!");
            RuleFor(x => x.PackageType).NotEqual(PackageType.None).WithMessage("Package type must be defined!");
            RuleFor(x => x.EA_Case).NotEqual(0).WithMessage("EA/case must be defined!");

            RuleFor(x => x.SkuCode).MustAsync(ReviewIfSkucodeExist)
                .When(x => !string.IsNullOrEmpty(x.SkuCode))
                .WithMessage(x => $"{x.SkuCode} already exist");

            RuleFor(x => x.Name).MustAsync(ReviewIfNameExist)
               .When(x => !string.IsNullOrEmpty(x.Name))
               .WithMessage(x => $"{x.Name} already exist");

        }

        async Task<bool> ReviewIfSkucodeExist(SKUDTO request, string name, CancellationToken cancellationToken)
        {
            ValidateSKUCodeRequest validate = new()
            {
                SkuCode = request.SkuCode,


                Id = request.Id

            };
            var result = await Service.Validate(validate);
            return !result;
        }
        async Task<bool> ReviewIfNameExist(SKUDTO request, string name, CancellationToken cancellationToken)
        {
            ValidateSKUNameRequest validate = new()
            {
                Name = name,


                Id = request.Id

            };
            var result = await Service.Validate(validate);
            return !result;
        }
    }
}
