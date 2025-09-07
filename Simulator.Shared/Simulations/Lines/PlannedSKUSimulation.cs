using Simulator.Shared.Models.HCs.PlannedSKUs;
using Simulator.Shared.Models.HCs.SKULines;
using Simulator.Shared.Simulations.Materials;

namespace Simulator.Shared.Simulations.Lines
{
    public class PlannedSKUSimulation
    {
        PlannedSKUSimulationCalculator PlannedSKUSimulationCalculator = null!;
        PlannedSKUDTO PlannedSKU { get; set; } = null!;
        public Guid PlannedSKUId => PlannedSKU.Id;
        SKUSimulation SKUSimulation { get; set; } = null!;
        public PlannedSKUSimulation(PlannedSKUDTO plannedSKUDTO, BaseLine line, SKUSimulation sKUSimulation, List<SKULineDTO> skulines)
        {
            PlannedSKU = plannedSKUDTO;
            LineTimeToReviewAU = line.TimeToReviewAU;
            PlannedSKUSimulationCalculator = new(this);
            var skuline = skulines.FirstOrDefault(x => x.SKUId == plannedSKUDTO.SKUId && x.LineId == line.Id);
            if (skuline != null)
            {
                PlannedSKU.Case_Shift = skuline.Case_Shift;
            }
            CalculateTimeStarvedAU();
            SKUSimulation = sKUSimulation;
        }
        public int Order => PlannedSKU.Order;
        Amount TimeToReviewAU { get; set; } = new(TimeUnits.Minute);
        Amount LineTimeToReviewAU { get; set; } = new(TimeUnits.Minute);

        public ProductBackBoneSimulation BackBoneSimulation => SKUSimulation.BackBoneSimulation;

        public Amount MassFlow => PlannedSKU.MassFlowSKU;
        public Amount AverageMassFlow => PlannedSKU.MassFlowSKUAverage;
        public Amount Weigth_EA => PlannedSKU.Weigth_EA;
        public Amount Cases => new(PlannedSKU.PlannedCases, CaseUnits.Case);

        public Amount EA_Case => new(PlannedSKU.EA_Case, EACaseUnits.EACase);
        public string SkuName => PlannedSKU.SkuName;
        public Amount LineSpeed => PlannedSKU.LineSpeed;
        public Amount Case_Shift => new(PlannedSKU.Case_Shift, CaseUnits.Case);

        public Amount EA => new(Cases.GetValue(CaseUnits.Case) * EA_Case.GetValue(EACaseUnits.EACase), EAUnits.EA);
        public Amount Shifts => Cases / Case_Shift;
        public Amount BottlesByShift => Case_Shift * EA_Case;
        public Amount ProductMassByShift => BottlesByShift * Weigth_EA;

        public Amount PlannedMassSKU => new(EA.GetValue(EAUnits.EA) * Weigth_EA.GetValue(MassUnits.KiloGram), MassUnits.KiloGram);

        public double PlannedAU => PlannedSKU.PlannedAU;
        public Amount Time_Producing => TimeToReviewAU * PlannedAU / 100;
        public Amount Time_StarvedByAU => TimeToReviewAU - Time_Producing;
        public Amount TimeToChangeSKU => PlannedSKU.TimeToChangeSKU;


        private Random _random = new Random();
        public void CalculateTimeStarvedAU()
        {
            int seconds = Convert.ToInt32(LineTimeToReviewAU.GetValue(TimeUnits.Second));

            var time = _random.Next(seconds);
            TimeToReviewAU = new(time, TimeUnits.Second);

        }
        public void Calculate()
        {
            PlannedSKUSimulationCalculator.Calculate();
        }
        public bool IsPlannedCasesAchieved => PlannedSKUSimulationCalculator.Cases >= Cases;


        public Amount CurrentCases => PlannedSKUSimulationCalculator.Cases;
        public Amount PendingMass => PlannedSKUSimulationCalculator.PendingMass;
        public Amount ProducedMass => PlannedSKUSimulationCalculator.ProducedMass;
    }

}
