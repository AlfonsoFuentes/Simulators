using Simulator.Shared.Models.HCs.SKULines;
using Simulator.Shared.Models.HCs.SKUs;
using Web.Infrastructure.Managers.Generic;

namespace Web.Infrastructure.Validators.FinishinLines.SKULines
{

    public class SKULineValidator : AbstractValidator<SKULineDTO>
    {
        private readonly IGenericService Service;

        public SKULineValidator(IGenericService service)
        {
            Service = service;

            RuleFor(x => x.SKU).NotNull().WithMessage("SKU must be defined!");
            RuleFor(x => x.Case_Shift).NotEqual(0).WithMessage("Cases / shift must be defined!");

            RuleFor(x => x.LineSpeedValue).NotEqual(0).WithMessage("SKU Line speed must be defined!");



            RuleFor(x => x.SKU).MustAsync(ReviewIfNameExist)
                .When(x => x.LineId != Guid.Empty && x.SKU != null)
                .WithMessage(x => $"{x.SKUName} already exist");

           

        }

        async Task<bool> ReviewIfNameExist(SKULineDTO request, SKUDTO? name, CancellationToken cancellationToken)
        {
            ValidateSKULineNameRequest validate = new()
            {
               SKUId = request.SKUId,
               LineId = request.LineId,


                Id = request.Id

            };
            var result = await Service.Validate(validate);
            return !result;
        }
       
    }
}
