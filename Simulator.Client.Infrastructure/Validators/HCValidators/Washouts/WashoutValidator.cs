using FluentValidation;
using Simulator.Shared.Enums.HCEnums.Enums;
using Simulator.Shared.Models.HCs.Tanks;
using Simulator.Shared.Models.HCs.Washouts;
using Web.Infrastructure.Managers.Generic;

namespace Web.Infrastructure.Validators.FinishinLines.Washouts
{

    public class WashoutValidator : AbstractValidator<WashoutDTO>
    {
        private readonly IGenericService Service;

        public WashoutValidator(IGenericService service)
        {
            Service = service;



            RuleFor(x => x.ProductCategoryCurrent).NotEqual(ProductCategory.None).WithMessage("Product category current must be defined!");
            RuleFor(x => x.ProductCategoryNext).NotEqual(ProductCategory.None).WithMessage("Product category next must be defined!");


            RuleFor(x => x.MixerWashoutValue).NotEqual(0).WithMessage("Mixer Washout time must be defined!");
            RuleFor(x => x.LineWashoutValue).NotEqual(0).WithMessage("Line Washout time must be defined!");


            RuleFor(x => x.ProductCategoryCurrent).MustAsync(ReviewIfNameExist)
              .When(x => x.ProductCategoryCurrent != ProductCategory.None && x.ProductCategoryNext != ProductCategory.None)
              .WithMessage(x => $"Washout from: {x.ProductCategoryCurrent} to {x.ProductCategoryNext} already exist");
        }

        async Task<bool> ReviewIfNameExist(WashoutDTO request, ProductCategory name, CancellationToken cancellationToken)
        {
            ValidateWashoutRequest validate = new()
            {
                Current = request.ProductCategoryCurrent,
                Next = request.ProductCategoryNext,
                Id = request.Id

            };
            var result = await Service.Validate(validate);
            return !result;
        }
    }
}
