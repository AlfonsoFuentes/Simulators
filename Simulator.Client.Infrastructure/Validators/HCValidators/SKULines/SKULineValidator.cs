using Simulator.Client.Infrastructure.Managers.ClientCRUDServices;
using Simulator.Shared.Enums.HCEnums.Enums;
using Simulator.Shared.Models.HCs.SKULines;
using Simulator.Shared.Models.HCs.SKUs;
using Simulator.Shared.Models.HCs.Washouts;
using Web.Infrastructure.Managers.Generic;

namespace Web.Infrastructure.Validators.FinishinLines.SKULines
{

    public class SKULineValidator : AbstractValidator<SKULineDTO>
    {
        private readonly IClientCRUDService Service;

        public SKULineValidator(IClientCRUDService service)
        {
            Service = service;

            RuleFor(x => x.SKU).NotNull().WithMessage("SKU must be defined!");
            RuleFor(x => x.Case_Shift).NotEqual(0).WithMessage("Cases / shift must be defined!");

            RuleFor(x => x.LineSpeedValue).NotEqual(0).WithMessage("SKU Line speed must be defined!");





            RuleFor(x => x.SKU)
          .MustAsync((dto, _, ct) => ValidateCombination(dto, ct))
          .When(x => x.LineId != Guid.Empty && x.SKU != null).WithMessage(x => $"{x.SKUName} already exist");
        }

       
        private async Task<bool> ValidateCombination(SKULineDTO dto, CancellationToken ct)
        {
            dto.ValidationKey = SKULineDTO.ValidationSKUId;
            var result = await Service.Validate(dto);
            return result.Succeeded;
        }
    }
}
