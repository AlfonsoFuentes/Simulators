using FluentValidation;
using Simulator.Client.Infrastructure.Managers.ClientCRUDServices;
using Simulator.Shared.Enums.HCEnums.Enums;
using Simulator.Shared.Models.HCs.Tanks;
using Simulator.Shared.Models.HCs.Washouts;
using Web.Infrastructure.Managers.Generic;

namespace Web.Infrastructure.Validators.FinishinLines.Washouts
{

    public class WashoutValidator : AbstractValidator<WashoutDTO>
    {
        private readonly IClientCRUDService Service;

        public WashoutValidator(IClientCRUDService service)
        {
            Service = service;

            RuleFor(x => x.FocusFactory).NotEqual(FocusFactory.None).WithMessage("Focus Factory must be defined!");


            RuleFor(x => x.ProductCategoryCurrent).NotEqual(ProductCategory.None).WithMessage("Product category current must be defined!");
            RuleFor(x => x.ProductCategoryNext).NotEqual(ProductCategory.None).WithMessage("Product category next must be defined!");


            RuleFor(x => x.MixerWashoutValue).NotEqual(0).WithMessage("Mixer Washout time must be defined!");
            RuleFor(x => x.LineWashoutValue).NotEqual(0).WithMessage("Line Washout time must be defined!");

          
            RuleFor(x => x.ProductCategoryNext)
           .MustAsync((dto, _, ct) => ValidateCombination(dto, ct))
           .When(x => x.ProductCategoryCurrent != ProductCategory.None).WithMessage("Combination alredy exist");
        }

        private async Task<bool> ValidateCombination(WashoutDTO dto, CancellationToken ct)
        {
            dto.ValidationKey = WashoutDTO.ProductCategoryCombination;
            var result = await Service.Validate(dto);
            return result.Succeeded;
        }
    }
}
