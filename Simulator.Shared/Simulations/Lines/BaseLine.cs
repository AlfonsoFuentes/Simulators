using Simulator.Shared.Commons.FileResults;
using Simulator.Shared.Models.HCs.EquipmentPlannedDownTimes;
using Simulator.Shared.Models.HCs.LinePlanneds;
using Simulator.Shared.Models.HCs.Lines;
using Simulator.Shared.Models.HCs.SKULines;
using Simulator.Shared.Models.HCs.Washouts;
using Simulator.Shared.Simulations.Lines.States;
using Simulator.Shared.Simulations.Materials;
using Simulator.Shared.Simulations.Pumps;
using Simulator.Shared.Simulations.SimulationResults.Lines;
using Simulator.Shared.Simulations.Tanks;
using Simulator.Shared.Simulations.Tanks.WIPLinetMixers;

namespace Simulator.Shared.Simulations.Lines
{
    public class BaseLine : NewBaseEquipment
    {
        public override Guid Id => LineDTO == null ? Guid.Empty : LineDTO.Id;

        public override string Name => LineDTO == null ? string.Empty : $"{LineDTO.Name}";
        protected LineDTO LineDTO { get; private set; } = null!;
        public ShiftManagement ShiftManagement { get; set; } = null!;
        public ManagePlannedProduction ManagePlannedProduction { get; set; } = null!;
        public List<WashoutSimulation> WashouTimes { get; set; } = null!;
        public void SetLine(LineDTO _LineDTO, List<WashoutSimulation> _WashouTimes)
        {
            WashouTimes = _WashouTimes;
            LineDTO = _LineDTO;
            PlannedDownTimes = LineDTO.PlannedDownTimes;
            ShiftManagement = new(this);
            ManagePlannedProduction = new(this);
            LineState = new LineStateNotScheduled(this);
        }

        public BaseLine()
        {

            EquipmentType = Enums.HCEnums.Enums.ProccesEquipmentType.Line;

           

        }
        public Amount MassOutlet { get; set; } = new(MassUnits.KiloGram);
        public void CalculateOneSecond()
        {
            ManagePlannedProduction.Calculate();
            LabelCurrentMassFlow = ManagePlannedProduction.GetMassFlow;
            LabelCurrentLineSpeed = ManagePlannedProduction.LineSpeed;
            WIPTank.SetOutletFlow(ManagePlannedProduction.GetMassFlow);
            MassOutlet += ManagePlannedProduction.GetMassFlow * OneSecond;
            WIPTank.Calculate(CurrentDate);
        }
        public void NotCalculateOneSecond()
        {
            ManagePlannedProduction.NotCalculate();
            LabelCurrentMassFlow = FlowEmpty;
            LabelCurrentLineSpeed = LineSpeedEmpty;
            WIPTank.SetOutletFlow(FlowEmpty);
            WIPTank.Calculate(CurrentDate);
        }
        public Amount LabelCurrentMassFlow { get; set; } = new Amount(MassFlowUnits.Kg_hr);
        public Amount LabelCurrentLineSpeed { get; set; } = new Amount(LineVelocityUnits.EA_min);
        public List<PlannedSKUSimulation> PlannedSKUSimulations { get; private set; } = new();
        LinePlannedDTO plannedLine = null!;
        public void InitLine(LinePlannedDTO plannedLineDTO, List<SKUSimulation> SkuSimulations, List<SKULineDTO> skulines)
        {
            plannedLine = plannedLineDTO;
            ShiftManagement.SetShiftType(plannedLineDTO.ShiftType);

            foreach (var row in plannedLineDTO.PlannedSKUDTOs)
            {
                PlannedSKUSimulations.Add(new(row, this, SkuSimulations.FirstOrDefault(x => x.Id == row.SKU!.Id)!, skulines));
            }
            Init();


        }
        public bool IsTimeProducingAchieved => ManagePlannedProduction.IsTimeProducingAchieved;
        public bool IsTimeStarvedAUAchieved => ManagePlannedProduction.IsTimeStarvedAUAchieved;
        public bool IsPlannedCasesAchieved => ManagePlannedProduction.IsPlannedCasesAchieved;
        public bool IsProductionPlanAchieved => ManagePlannedProduction.IsProductionPlanAchieved;
        public bool IsNextBackBoneSameAsCurrent => ManagePlannedProduction.IsNextBackBoneSameAsCurrent;
        public bool IsWipTankLoLevel => WIPTank.IsTankLoLevel;

        public bool IsPlannedDowTime => PlannedDownTimes.Count == 0 ? false : PlannedDownTimes.Any(x => x.Between(CurrentDate));

        EquipmentPlannedDownTimeDTO EquipmentPlannedDownTimeFound => PlannedDownTimes.Count == 0 ? null! : PlannedDownTimes.FirstOrDefault(x => x.Between(CurrentDate))!;
        public Amount CurrentPlannedDowntime => EquipmentPlannedDownTimeFound == null! ? new(TimeUnits.Second) : EquipmentPlannedDownTimeFound.SpanTime;
        public string CurrentPlannedDowntimeName => EquipmentPlannedDownTimeFound == null! ? string.Empty : EquipmentPlannedDownTimeFound.Name;

        public bool IsPlannedDowntimeAchieved => CurrentPlannedDowntime == null ? false : CurrenTime >= CurrentPlannedDowntime;
        public Amount TimeToReviewAU => LineDTO.TimeToReviewAU;

        public bool IsNextShiftPlanned => ShiftManagement.IsNextShiftPlanned;
        public bool IsPlannedShift => ShiftManagement.IsPlannedShift;
        public bool IsTimeChangingSKUAchieved => ManagePlannedProduction.IsTimeChangingSKUAchieved(CurrenTime);

        public string LabelLineState => LineState == null ? "" : LineState.StateLabel;
        protected BasePump InletPump => (BasePump)GetInletAttachedEquipment()!;
        public WIPTank WIPTank { get; set; } = null!;

        public LineState LineState { get; set; } = null!;

        public bool LineScheduled => LineState is not LineStateNotScheduled;


        public void SetLineStateState(LineState newLineState)
        {
            LineState = newLineState;
        }
        Amount LineSpeedEmpty { get; set; } = new Amount(LineVelocityUnits.EA_min);
        Amount FlowEmpty { get; set; } = new Amount(MassFlowUnits.Kg_sg);
        public bool IsPlannedSkus => PlannedSKUSimulations.Count > 0;
        public Amount TimeToChangeSKU => ManagePlannedProduction.TimeToChangeSKU;
        public Amount AverageMassFlow => CurrenTimeForMass.Value == 0 ? new(MassFlowUnits.Kg_sg) : MassOutlet / CurrenTimeForMass;
        public Amount CurrentCases => ManagePlannedProduction.CurrentCases;
        public Amount PlannedCases => ManagePlannedProduction.Cases;

        public Amount PendingMass => ManagePlannedProduction.PendingMass;
        public Amount PlannedMass => ManagePlannedProduction.PlannedMass;
        public Amount ProducedMass => ManagePlannedProduction.ProducedMass;
        public Amount CurrenTimeShift { get; set; } = new(1,TimeUnits.Second);
        public Amount CurrenTime { get; set; } = new(1,TimeUnits.Second);
        public Amount CurrenChangeOverTime { get; set; } = new(TimeUnits.Minute);
        public Amount CurrenTimeForMass { get; set; } = new(1, TimeUnits.Second);
        public Amount PendingTimeShift => Shift8Hrs - CurrenTimeShift;
        public Amount PendingMassShift => PendingMass < PendingTimeShift * AverageMassFlow ?
        PendingMass : PendingTimeShift * AverageMassFlow;
        public Amount Shift8Hrs { get; set; } = new(8, TimeUnits.Hour);
        public string CurrentSkuName => ManagePlannedProduction.SkuName;
        public BackBoneSimulation CurrentBackBone => ManagePlannedProduction.BackBoneSimulation;
        public PlannedSKUSimulation CurrentSKU => ManagePlannedProduction.CurrentSKU;
        public Amount NextSKUPlannedMassSKU => NextSKU == null ? new(MassUnits.KiloGram) : NextSKU.PlannedMassSKU;
        public string NextSKUName => NextSKU == null ? "Not SKU Planned" : NextSKU.SkuName;
        public PlannedSKUSimulation NextSKU => ManagePlannedProduction.NextSKU;
        public Amount NextSKUCases => NextSKU == null ? new(CaseUnits.Case) : NextSKU.Cases;
        public DateTime CurrentDate { get; set; }
        public void InitDataForNextShift()
        {
            if (WIPTank != null) WIPTank.InitNextShift();
            CurrenTimeShift = new(TimeUnits.Second);
            if (!IsNextShiftPlanned)
            {
                SetLineStateState(new LineStateShiftNotPlanned(this));
            }


        }
        public bool IsWIPTankAbleToInitNextShifProduction => WIPTank.ChechLevelForShiftChange();

        public void PickSKU()
        {
            if (IsProductionPlanAchieved || ManagePlannedProduction.NextSKU == null)
            {
                return;

            }
            else if (!ShiftManagement.IsPlannedShift)
            {
                WIPTank.NewChangeSkuLine();
                return;
            }
            else if (ShiftManagement.IsNextShiftPlanned)
            {
                WIPTank.NewChangeSkuLine();
                return;
            }

        }

        public override void Init()
        {

            if (!IsPlannedSkus || !ShiftManagement.IsLinePlanned)
            {
                SetLineStateState(new LineStateNotScheduled(this));
            }
            else
            {
                ManagePlannedProduction.Init();
                ChangeSku();
                InitPumpWIP();
                if (!ShiftManagement.IsPlannedShift)
                {
                    SetLineStateState(new LineStateShiftNotPlanned(this));

                }
                else if (!WIPTank.IsTankLoLevel)
                {
                    SetLineStateState(new LineStateRun(this));
                }
                else
                {
                    SetLineStateState(new LineStateWipLoLevel(this));

                }

            }

        }

        void InitPumpWIP()
        {
            if (InletPump.GetInletAttachedEquipment() is WIPTank tank)
            {
                WIPTank = tank;

                WIPTank.SetLine(this);
                WIPTank.CurrentLevel = plannedLine.WIPLevel;
                WIPTank.InitFromLine();

            }

        }
        public void ChangeSku()
        {
            MassOutlet = new(MassUnits.KiloGram);
            CurrenTimeForMass = new(TimeUnits.Minute);
            ManagePlannedProduction.ChangeSku();

        }
        public void WIPChangeOver()
        {
            WIPTank.ChangeOver();
        }
        public override void Calculate(DateTime currentdate)
        {
            ManageTime(currentdate);
            LineState.Calculate();
        }
        public void SetTime(DateTime currentdate)
        {
            CurrentDate = currentdate;
            ShiftManagement.SetTime(currentdate);
        }
        void ManageTime(DateTime currentdate)
        {
            SetTime(currentdate);
            CurrenTimeForMass+= OneSecond;
            CurrenTime+= OneSecond;
            CurrenTimeShift+= OneSecond;

        }
        List<LineResult> Linesresult = new();
        public void StorageDataToSimulation(string state)
        {
            if (ManagePlannedProduction.CurrentSKU == null) return;
            Linesresult.Add(new()
            {
                CurrentDate = CurrentDate,
                Cases = CurrentCases,
                Flow = LabelCurrentMassFlow,
                ProducedMass = ProducedMass,
                PendingMass = PendingMass,
                State = state,
                WIPLevel = WIPTank.CurrentLevel,
                SKU = CurrentSKU,
                TimeInitNextBatch = WIPTank is not WIPInletMixer ? null! : ((WIPInletMixer)WIPTank).TimeToStartNextBatch,




            });
        }
        public BasePump Washoutpump { get; set; } = null!;
        public BasePump GetWashoutPump()
        {
            if (SearchInletWashingEquipment() is BasePump pump)
            {
                Washoutpump = pump;
            }
            return Washoutpump;
        }
        public void ReleaseWashoutPump()
        {

            RemoveProcessInletEquipment(Washoutpump);
            Washoutpump = null!;
        }




    }

}
