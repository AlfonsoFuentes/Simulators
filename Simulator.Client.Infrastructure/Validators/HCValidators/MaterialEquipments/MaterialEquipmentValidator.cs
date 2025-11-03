using Simulator.Client.Infrastructure.Managers.ClientCRUDServices;
using Simulator.Shared.Models.HCs.MaterialEquipments;
using Simulator.Shared.Models.HCs.Materials;
using Web.Infrastructure.Managers.Generic;

namespace Web.Infrastructure.Validators.FinishinLines.MaterialEquipments
{

    public class MaterialEquipmentValidator : AbstractValidator<MaterialEquipmentDTO>
    {
        private readonly IClientCRUDService Service;

        public MaterialEquipmentValidator(IClientCRUDService service)
        {
            Service = service;

            RuleFor(x => x.Material).NotNull().WithMessage("Material must be defined!");
            RuleFor(x => x.CapacityValue).NotEqual(0).When(x => x.IsMixer).WithMessage("Capacity must be defined!");
            RuleFor(x => x.Material).MustAsync(ReviewIfNameExist)
                .When(x => x.Material != null && x.ProccesEquipmentId != Guid.Empty)
                .WithMessage(x => $"{x.MaterialM_Number}  already exist in this equipment");

           

        }

        async Task<bool> ReviewIfNameExist(MaterialEquipmentDTO request, MaterialDTO name, CancellationToken cancellationToken)
        {
            request.ValidationKey = MaterialEquipmentDTO.MaterialEquipmentCombination;
            var result = await Service.Validate(request);
            return result.Succeeded;
        }

    }
}
