using Simulator.Shared.Enums.HCEnums.Enums;
using Simulator.Shared.Models.HCs.Materials;
using Web.Infrastructure.Managers.Generic;

namespace Web.Infrastructure.Validators.FinishinLines.Materials
{

    public class MaterialValidator : AbstractValidator<MaterialDTO>
    {
        private readonly IGenericService Service;

        public MaterialValidator(IGenericService service)
        {
            Service = service;

            RuleFor(x => x.SAPName).NotEmpty().WithMessage("SAPName must be defined!");
            RuleFor(x => x.M_Number).NotEmpty().WithMessage("M_Number must be defined!");
            RuleFor(x => x.CommonName).NotEmpty().WithMessage("Common Name must be defined!");

            RuleFor(x => x.MaterialType).NotEqual(MaterialType.None).WithMessage("Material Type must be defined!");
            RuleFor(x => x.FocusFactory).NotEqual(FocusFactory.None).WithMessage("Focus Factory must be defined!");

            RuleFor(x => x.PhysicalState).NotEqual(MaterialPhysicState.None).WithMessage("Physical State must be defined!");
            RuleFor(x => x.ProductCategory)
                .NotEqual(ProductCategory.None)
                .When(x => x.MaterialType == MaterialType.RawMaterialBackBone || x.MaterialType == MaterialType.ProductBackBone)
                .WithMessage("Product Category must be defined!");

            RuleFor(x => x.BackBoneSteps.Count)
                .NotEqual(0)
                .When(x => x.MaterialType == MaterialType.RawMaterialBackBone || x.MaterialType == MaterialType.ProductBackBone)
                .WithMessage("Back Bone Steps must be defined!");


            RuleFor(x => x.SAPName).MustAsync(ReviewIfNameExist)
                .When(x => !string.IsNullOrEmpty(x.SAPName))
                .WithMessage(x => $"{x.SAPName} already exist");

            RuleFor(x => x.M_Number).MustAsync(ReviewIfMNumberExist)
               .When(x => !string.IsNullOrEmpty(x.M_Number))
               .WithMessage(x => $"{x.M_Number} already exist");

            RuleFor(x => x.CommonName).MustAsync(ReviewIfCommonNameExist)
              .When(x => !string.IsNullOrEmpty(x.CommonName))
              .WithMessage(x => $"{x.CommonName} already exist");

        }

        async Task<bool> ReviewIfNameExist(MaterialDTO request, string name, CancellationToken cancellationToken)
        {
            ValidateMaterialNameRequest validate = new()
            {
                SapName = name,
                FocusFactory = request.FocusFactory,

                Id = request.Id

            };
            var result = await Service.Validate(validate);
            return !result;
        }
        async Task<bool> ReviewIfMNumberExist(MaterialDTO request, string name, CancellationToken cancellationToken)
        {
            ValidateMaterialMNumberRequest validate = new()
            {
                MNumber = name,

                FocusFactory = request.FocusFactory,
                Id = request.Id

            };
            var result = await Service.Validate(validate);
            return !result;
        }
        async Task<bool> ReviewIfCommonNameExist(MaterialDTO request, string name, CancellationToken cancellationToken)
        {
            ValidateMaterialCommonNameRequest validate = new()
            {
                CommonName = name,
                FocusFactory = request.FocusFactory,

                Id = request.Id

            };
            var result = await Service.Validate(validate);
            return !result;
        }
    }
}
