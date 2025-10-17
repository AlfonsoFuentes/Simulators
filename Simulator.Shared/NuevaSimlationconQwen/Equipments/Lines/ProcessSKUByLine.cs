using Simulator.Shared.Enums.HCEnums.Enums;
using Simulator.Shared.NuevaSimlationconQwen.Materials;
using Simulator.Shared.NuevaSimlationconQwen.Reports;
using System;
using static Simulator.Shared.StaticClasses.StaticClass;

namespace Simulator.Shared.NuevaSimlationconQwen.Equipments.Lines
{
    public class ProcessSKUByLine
    {

        public ProcessSKUByLine()
        {

        }
        public int Order { get; set; }
        public Guid Id { get; set; }
        public string SkuName { get; set; } = string.Empty;
        public IMaterial Material { get; set; } = null!;
        public Amount TimeToReviewAU { get; set; } = new(TimeUnits.Minute);


        public Amount Weigth_EA { get; set; } = new Amount(MassUnits.KiloGram);
        public Amount TotalCases { get; set; } = new(CaseUnits.Case);
        public Amount EA_Case { get; set; } = new(EACaseUnits.EACase);
        public Amount LineSpeed { get; set; } = new(LineVelocityUnits.EA_min);
        public Amount Case_Shift { get; set; } = new(CaseUnits.Case);
        public ProductCategory ProductCategory { get; set; } = ProductCategory.None;
        public Amount Size { get; set; } = new(VolumeUnits.MilliLiter);
        public double PlannedAU { get; set; }

        public Amount TotalPlannedEA => new(TotalCases.GetValue(CaseUnits.Case) * EA_Case.GetValue(EACaseUnits.EACase), EAUnits.EA);

        public double Shifts => Case_Shift.Value == 0 ? 0 : TotalCases.GetValue(CaseUnits.Case) / Case_Shift.GetValue(CaseUnits.Case);
        public Amount TimePlanned => Shifts == 0 ? new Amount(0, TimeUnits.Hour) : new Amount(Shifts * 8, TimeUnits.Hour);
        public Amount EAsByShift => Case_Shift * EA_Case;
        public Amount ProductMassByShift => EAsByShift * Weigth_EA;
        public Amount MaxMassFlow => new Amount(LineSpeed.GetValue(LineVelocityUnits.EA_min) * Weigth_EA.GetValue(MassUnits.KiloGram), MassFlowUnits.Kg_min);
        public Amount TotalPlannedMass => new Amount(TotalPlannedEA.GetValue(EAUnits.EA) * Weigth_EA.GetValue(MassUnits.KiloGram), MassUnits.KiloGram);
        public Amount TimeToChangeFormat { get; set; } = new Amount(0, TimeUnits.Minute);

    }
    public class ProductionSKURun
    {
        public double EfficiencyTimePercentage => Math.Round(
           TimePlanned.GetValue(TimeUnits.Minute) > 0
               ? (TimeConsumed.GetValue(TimeUnits.Minute) / TimePlanned.GetValue(TimeUnits.Minute)) * 100
               : 0, 1);

        //Propiedades que vienen del SKU
        private ProcessSKUByLine? _plannedSKU = null!;
        private readonly IRandomGenerator _randomGenerator;
        private Amount TimeToReviewAU => _plannedSKU?.TimeToReviewAU ?? new Amount(0, TimeUnits.Minute);
        public Amount PlannedCases => _plannedSKU?.TotalCases ?? new Amount(0, CaseUnits.Case);
        private Amount Case_Shift => _plannedSKU?.Case_Shift ?? new Amount(0, CaseUnits.Case);
        private Amount EA_Case => _plannedSKU?.EA_Case ?? new Amount(0, EAUnits.EA);
        public Amount LineSpeed => _plannedSKU?.LineSpeed ?? new Amount(0, LineVelocityUnits.EA_min);
        public Amount Weigth_EA => _plannedSKU?.Weigth_EA ?? new Amount(0, MassUnits.KiloGram);
        public double PlannedAU => _plannedSKU?.PlannedAU ?? 0;
        public Amount TotalPlannedEA => _plannedSKU?.TotalPlannedEA ?? new Amount(0, EAUnits.EA);
        public double Shifts => _plannedSKU?.Shifts ?? 0;
        public Amount EAsByShift => _plannedSKU?.EAsByShift ?? new Amount(0, EAUnits.EA);
        public Amount TimePlanned => new Amount(Shifts * 8, TimeUnits.Hour);
        public Amount TimeConsumed => CurrentRunningTime;
        public Amount ProductMassByShift => _plannedSKU?.ProductMassByShift ?? new Amount(0, MassUnits.KiloGram);
        public Amount MaxMassFlow => _plannedSKU?.MaxMassFlow ?? new Amount(0, MassFlowUnits.Kg_min);
        public Amount TotalPlannedMass => _plannedSKU?.TotalPlannedMass ?? new Amount(0, MassUnits.KiloGram);
        //Calculadas durante la simulacion

        public Amount AverageMassFlow => CurrentRunningTime.Value == 0 ? new(0, MassFlowUnits.Kg_sg) :
            new(ProducedMass.GetValue(MassUnits.KiloGram) / CurrentRunningTime.GetValue(TimeUnits.Second)
                , MassFlowUnits.Kg_sg);


        public Amount ZeroFlow { get; private set; } = new Amount(0, MassFlowUnits.Kg_min);
        public Amount CurrentFlow { get; private set; } = new Amount(0, MassFlowUnits.Kg_min);
        public Amount ProducedMass { get; private set; } = new Amount(0, MassUnits.KiloGram);
        public Amount ProducedCases { get; private set; } = new(0, CaseUnits.Case);
        public Amount ProducedEA { get; private set; } = new(0, EAUnits.EA);
        public Amount CurrentRunningTime { get; set; } = new Amount(0, TimeUnits.Second);//Contador de tiempo total
        Amount CurrentStarvedTime { get; set; } = new Amount(0, TimeUnits.Second);//Contador de tiempo parado por Au    
        Amount CurrentProducingTime { get; set; } = new Amount(0, TimeUnits.Second);//Contador de tiempo produciendo
        public Amount OneSecond => new Amount(1, TimeUnits.Second);
        // Propiedades calculadas — siempre actualizadas
        public Amount RemainingCases => PlannedCases - ProducedCases;
        public Amount RemainingMass => TotalPlannedMass - ProducedMass;

     
        public bool IsCompleted => RemainingMass <= Amount.Zero(MassUnits.KiloGram);
        public Amount CalculatedTimeToReviewAU { get; private set; } = new Amount(0, TimeUnits.Minute);
        public bool IsRunningAU { get; private set; }

        public ProductionSKURun(ProcessSKUByLine? _plannedSKU)
        {
            this._plannedSKU = _plannedSKU;

            _randomGenerator = new RandomGenerator();
            CalculateTimeStarvedAU();

        }


        public void Produce()
        {
            IsRunningAU = false;
            CurrentRunningTime += OneSecond;
            CurrentProducingTime += OneSecond;
            var currenMass = MaxMassFlow * OneSecond;
            var currentEA = LineSpeed * OneSecond;
            var currentCase = currentEA / EA_Case;
            CurrentFlow = MaxMassFlow;

            ProducedMass += currenMass;
            ProducedEA += LineSpeed * OneSecond;
            ProducedCases += currentCase;

            // Actualizar cajas

        }

        public void ProcessDuringAU()
        {
            CurrentRunningTime += OneSecond;
            CurrentFlow = ZeroFlow;
            IsRunningAU = true;
            CurrentStarvedTime += OneSecond;

        }
        Amount Time_Producing => CalculatedTimeToReviewAU * PlannedAU / 100;
        Amount Time_StarvedByAU => CalculatedTimeToReviewAU - Time_Producing;
        public Amount Pending_Time_Producing => Time_Producing - CurrentProducingTime;
        public Amount Pending_Time_StarvedByAU => Time_StarvedByAU - CurrentStarvedTime;

        public void CalculateTimeStarvedAU()
        {
            int seconds = Convert.ToInt32(TimeToReviewAU.GetValue(TimeUnits.Second));

            var time = _randomGenerator.Next(seconds);
            CalculatedTimeToReviewAU = new(time, TimeUnits.Second);
            CurrentProducingTime = new Amount(0, TimeUnits.Minute);
            CurrentStarvedTime = new(0, TimeUnits.Second);
        }

        public Amount EstimatedTimeRemaining
        {
            get
            {
                if (AverageMassFlow.Value == 0) return new(0, TimeUnits.Second);
                var time = RemainingMass.GetValue(MassUnits.KiloGram) / AverageMassFlow.GetValue(MassFlowUnits.Kg_min);

                Amount remainingMinutes = new(time, TimeUnits.Minute);
                return remainingMinutes;
            }
        }
    }
}
