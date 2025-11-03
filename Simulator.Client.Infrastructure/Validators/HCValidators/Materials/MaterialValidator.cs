using Simulator.Client.Infrastructure.ExtensionMethods;
using Simulator.Client.Infrastructure.Managers.ClientCRUDServices;
using Simulator.Shared.Enums.HCEnums.Enums;
using Simulator.Shared.Models.HCs.Materials;
using Web.Infrastructure.Managers.Generic;

namespace Web.Infrastructure.Validators.FinishinLines.Materials
{

    public class MaterialValidator : AbstractValidator<MaterialDTO>
    {
        private readonly IClientCRUDService Service;

        public MaterialValidator(IClientCRUDService service)
        {
            Service = service;

            RuleFor(x => x.SAPName).NotEmpty().WithMessage("SAPName must be defined!");
            RuleFor(x => x.M_Number).NotEmpty().WithMessage("M_Number must be defined!");
            RuleFor(x => x.CommonName).NotEmpty().WithMessage("Common Name must be defined!");

            RuleFor(x => x.MaterialType).NotEqual(MaterialType.None).WithMessage("Material Type must be defined!");
            RuleFor(x => x.FocusFactory).NotEqual(FocusFactory.None).WithMessage("Focus Factory must be defined!");

            RuleFor(x => x.PhysicalState).NotEqual(MaterialPhysicState.None).WithMessage("Physical State must be defined!");

            RuleFor(x => x.SAPName).MustBeUnique(service, x => x.SAPName)
            .WithMessage("SAPName already exists");

            RuleFor(x => x.M_Number).MustBeUnique(service, x => x.M_Number)
                .WithMessage("M_Number already exists");

            RuleFor(x => x.CommonName).MustBeUnique(service, x => x.CommonName)
                .WithMessage("CommonName already exists");


            RuleFor(x => x.ProductCategory)
                .NotEqual(ProductCategory.None)
                .When(x => x.MaterialType == MaterialType.RawMaterialBackBone || x.MaterialType == MaterialType.ProductBackBone)
                .WithMessage("Product Category must be defined!");

            RuleFor(x => x.BackBoneSteps.Count)
                .NotEqual(0)
                .When(x => x.MaterialType == MaterialType.RawMaterialBackBone || x.MaterialType == MaterialType.ProductBackBone)
                .WithMessage("Back Bone Steps must be defined!");


        }
     
        
    }
}
