using Simulator.Shared.Models.HCs.Washouts;
using Simulator.Shared.Simulations.Materials;

namespace Simulator.Shared.Simulations.Lines
{
    public class ManagePlannedProduction
    {
        BaseLine Line { get; set; } = null!;
        public Amount TimeToChangeSKU { get; set; } = new(TimeUnits.Minute);
        public Queue<PlannedSKUSimulation> QueueSKUs { get; set; } = new();
        List<WashoutSimulation> WashouTimes = null!;
        public PlannedSKUSimulation CurrentSKU { get; private set; } = null!;

        public PlannedSKUSimulation NextSKU => QueueSKUs.Count > 0 ? QueueSKUs.Peek() : null!;
        public ProductBackBoneSimulation BackBoneSimulation => CurrentSKU == null ? null! : CurrentSKU.BackBoneSimulation;
        public bool IsPlannedSKUs => QueueSKUs.Count > 0;
        public Amount CurrenTime { get; set; } = new(1,TimeUnits.Second);
        public Amount OneSecond { get; set; } = new(1, TimeUnits.Second);
        public Amount GetMassFlow => CurrentSKU == null ? new(TimeUnits.Minute) : CurrentSKU.MassFlow;
        public Amount LineSpeed => CurrentSKU == null ? new(TimeUnits.Minute) : CurrentSKU.LineSpeed;
        public bool IsTimeProducingAchieved => getTimeProducing();

        public bool IsTimeStarvedAUAchieved => getTimeStarvedAU();
        public bool IsPlannedCasesAchieved => CurrentSKU == null ? true : CurrentSKU.IsPlannedCasesAchieved;
        public bool IsProductionPlanAchieved => QueueSKUs.Count == 0 && IsPlannedCasesAchieved;
        public bool IsNextBackBoneSameAsCurrent=> NextSKU == null ? false : NextSKU.BackBoneSimulation.Id == CurrentSKU.BackBoneSimulation.Id;
        public Amount AverageMassFlow => CurrentSKU == null ? new(MassFlowUnits.Kg_hr) : CurrentSKU.AverageMassFlow;
        public Amount CurrentCases => CurrentSKU == null ? new(CaseUnits.Case) : CurrentSKU.CurrentCases;
        public Amount Cases => CurrentSKU == null ? new(CaseUnits.Case) : CurrentSKU.Cases;
        public Amount PendingMass => CurrentSKU == null ? new(MassUnits.KiloGram) : CurrentSKU.PendingMass;
        public Amount ProducedMass => CurrentSKU == null ? new(MassUnits.KiloGram) : CurrentSKU.ProducedMass;
        public Amount PlannedMass => CurrentSKU == null ? new(MassUnits.KiloGram) : CurrentSKU.PlannedMassSKU;
        public string SkuName => CurrentSKU == null ? string.Empty : CurrentSKU.SkuName;
        public ManagePlannedProduction(BaseLine line)
        {
            Line = line;
            WashouTimes = line.WashouTimes;

        }
        public void Init2()
        {
            QueueSKUs = ReadPlannedSKUS();
        }
        void CalculateTimChangeSKU()
        {
            var washout = NextSKU == null ? null! :
                WashouTimes.FirstOrDefault(x =>
                x.ProductCategoryCurrent == CurrentSKU.BackBoneSimulation.ProductCategory
            && x.ProductCategoryNext == NextSKU.BackBoneSimulation.ProductCategory);

            TimeToChangeSKU = washout == null ?
                new(TimeUnits.Minute) :
                NextSKU!.BackBoneSimulation.Id == CurrentSKU.BackBoneSimulation.Id ?
                new(TimeUnits.Minute) :
                washout.LineWashoutTime;
            if (NextSKU != null)
            {

                TimeToChangeSKU = NextSKU.TimeToChangeSKU > TimeToChangeSKU ? NextSKU.TimeToChangeSKU : TimeToChangeSKU;
            }
            else
            {
                TimeToChangeSKU = new(TimeUnits.Minute);
            }

        }
        List<PlannedSKUSimulation> PlannedOrdered => Line.PlannedSKUSimulations.OrderBy(x => x.Order).ToList();
        Queue<PlannedSKUSimulation> ReadPlannedSKUS()
        {
            Queue<PlannedSKUSimulation> result = new();

            PlannedOrdered.ForEach(x => result.Enqueue(x));

            return result;
        }

        public void ChangeSku()
        {
            if (QueueSKUs.Count == 0)
            {
                CurrentSKU = null!;

                return;
            }

            CurrentSKU = QueueSKUs.Dequeue();

            Line.SetCurrentMaterialSimulation(CurrentSKU.BackBoneSimulation);
            CalculateTimChangeSKU();
        }

        public void Calculate()
        {
            if (CurrentSKU == null) return ;
           
            CurrenTime+= OneSecond;
            CurrentSKU.Calculate();
        }
        public void NotCalculate()
        {
            CurrenTime += OneSecond;

        }

        bool getTimeProducing()
        {
            if (CurrentSKU == null) return false;
            if (CurrenTime > CurrentSKU.Time_Producing)

            {
                CurrenTime = new(1, TimeUnits.Second);
                CurrentSKU.CalculateTimeStarvedAU();
                return true;
            }
            return false;
        }
        bool getTimeStarvedAU()
        {
            if (CurrentSKU == null) return false;
            if (CurrenTime > CurrentSKU.Time_StarvedByAU)

            {
                CurrenTime = new(1, TimeUnits.Second);
                return true;
            }
            return false;
        }

        public bool IsTimeChangingSKUAchieved(Amount time) => time >= TimeToChangeSKU;



    }

}
