using Simulator.Shared.Models.HCs.LinePlanneds;
using Simulator.Shared.Models.HCs.Lines;
using Simulator.Shared.Models.HCs.SKULines;
using Simulator.Shared.NuevaSimlationconQwen.Materials;

namespace Simulator.Shared.NuevaSimlationconQwen.Equipments.Lines
{
    public class ProcessLineFactory
    {

        private readonly SimulationMessageService _messageService;

        public ProcessLineFactory(SimulationMessageService messageService)
        {
          
            _messageService = messageService;
        }

        public void CreateSKUs(ProcessLine line, LinePlannedDTO linePlanned,LineDTO lineDto,List<SKULineDTO> skus,List<IMaterial> materials)
        {
           

            var orderedSkus = linePlanned.PlannedSKUDTOs.OrderBy(x => x.Order).ToList();
            line.ShiftType = linePlanned.ShiftType;
            foreach (var plannedSku in orderedSkus)
            {
                var currentSku = skus.FirstOrDefault(x => x.SKUId == plannedSku.SKUId);
                if (currentSku == null)
                {
                    _messageService.AddWarning($"SKU with ID {plannedSku.SKUId} not found for Line ID {linePlanned.LineId} (\"{linePlanned.LineName}\").", "ProcessLineFactory");
                    continue;
                }

                if (plannedSku.SKU == null)
                {
                    _messageService.AddWarning($"SKU details missing for SKU ID {plannedSku.SKUId} in Line ID {linePlanned.LineId} (\"{linePlanned.LineName}\").", "ProcessLineFactory");
                    continue;
                }

                if (currentSku.SKU!.BackBone== null || !materials.Any(x => x.Id == currentSku.SKU!.BackBone.Id))
                {
                    _messageService.AddWarning($"BackBone material missing for SKU ID {plannedSku.SKUId} in Line ID {linePlanned.LineId} (\"{linePlanned.LineName}\").", "ProcessLineFactory");
                    continue;
                }
               
                var material = materials.First(m => m.Id == currentSku.SKU!.BackBone.Id);
                var sku = new ProcessSKUByLine()
                {
                    Order = plannedSku.Order,
                    Id = plannedSku.Id,
                    SkuName = plannedSku.SkuName,
                    Material = material,
                    PlannedAU = plannedSku.PlannedAU,
                    TimeToReviewAU = new Amount(lineDto.TimeToReviewAU.GetValue(TimeUnits.Minute), TimeUnits.Minute),
                    Weigth_EA = new Amount(plannedSku.SKU.Weigth.GetValue(MassUnits.KiloGram), MassUnits.KiloGram),
                    TotalCases = new Amount(plannedSku.PlannedCases, CaseUnits.Case),
                    EA_Case = new Amount(plannedSku.EA_Case, EACaseUnits.EACase),
                    LineSpeed = new Amount(currentSku.LineSpeed.GetValue(LineVelocityUnits.EA_min), LineVelocityUnits.EA_min),
                    Case_Shift = new Amount(plannedSku.Case_Shift, CaseUnits.Case),
                    ProductCategory = currentSku.ProductCategory,
                    Size = currentSku.SKU.Size,
                    TimeToChangeFormat = plannedSku.TimeToChangeSKU

                };
                line.QueueSKU(sku);
              
            }

          
        }
    }
}
