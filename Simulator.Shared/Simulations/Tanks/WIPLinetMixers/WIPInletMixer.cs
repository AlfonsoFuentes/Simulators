using Simulator.Shared.Models.HCs.Tanks;
using Simulator.Shared.Simulations.Lines;
using Simulator.Shared.Simulations.Materials;
using Simulator.Shared.Simulations.Mixers;
using Simulator.Shared.Simulations.Mixers.States.MixerCalculations.MixerStates;
using Simulator.Shared.Simulations.Pumps;

namespace Simulator.Shared.Simulations.Tanks.WIPLinetMixers
{
    public class WIPInletMixer : WIPTank
    {
        public ManageMassWIPTank MassCurrentSku { get; set; } = new();
        public ManageMassWIPTank MassNextSku { get; set; } = new();

        Amount Time4Hrs => new Amount(4, TimeUnits.Hour);
        Amount MasstoTransfer => Time4Hrs * CurrentPlannedSKU.AverageMassFlow;

        public bool IsMassNeedFromWIPBackBone => CurrentLevel <= MasstoTransfer;
        public WIPInletMixer(TankDTO tankDTO) : base(tankDTO)
        {
            CurrentLevel = tankDTO.InitialLevel;
            CalculateForOutlet = CalculateFromOutlet;
            CalculateForInlet = CalculateFromMixers;
        }



        public override void Calculate(DateTime currentdate)
        {
            base.Calculate(currentdate);

            CheckMassNeeded();


        }
        protected void CalculateFromOutlet()
        {

            CurrentLevel -= Outlet;
            TotalMassOutlet += Outlet;
            TotalMassOutletShift += Outlet;
            MassCurrentSku.Delivered -= Outlet;
            AverageOutlet = CurrentTime.GetValue(TimeUnits.Minute) > 10 ? TotalMassOutlet / CurrentTime : CurrentPlannedSKU.AverageMassFlow;

            Outlet.SetValue(0, MassUnits.KiloGram);

        }
        protected void CalculateFromMixers()
        {


            CurrentLevel += Inlet;
            MassCurrentSku.Producing -= Inlet;
            MassCurrentSku.Received += Inlet;
            Inlet.SetValue(0, MassUnits.KiloGram);


        }
        public override bool GetTankAvailableforTransferMixer(BaseMixer mixer)
        {
            if (InletOcupiedBy != null) return false;
            if (mixer.CurrentBackBoneSimulation.Id != CurrentPlannedSKU.BackBoneSimulation.Id) return false;


            var transfertime = CurrentPlannedSKU.BackBoneSimulation.BatchDataMixer[mixer].TransferTime;
            var outletduringTransfering = CurrentPlannedSKU.AverageMassFlow * transfertime;
            var futurelevel = CurrentLevel - outletduringTransfering + mixer.CurrentLevel;


            return futurelevel <= Capacity;
        }
        public override void InitFromLine()
        {
            CurrentDate = Line.CurrentDate;
            CurrentPlannedSKU = Line.CurrentSKU;
            NextPlannedSKU = Line.NextSKU;
            SetCurrentMaterialSimulation(CurrentPlannedSKU.BackBoneSimulation);

            MassCurrentSku.Needed = Line.PlannedMass + TankDTO.LoLoLevel * 1.1 - CurrentLevel;
            MassCurrentSku.Delivered = Line.PlannedMass;

            if (NextPlannedSKU != null)
            {
                MassNextSku.Needed = NextPlannedSKU.PlannedMassSKU + TankDTO.LoLoLevel * 1.1;
                MassNextSku.Delivered = NextPlannedSKU.PlannedMassSKU;
            }

        }
        void CheckMassNeededCurrentBackBone()
        {
            if (IsCurrentBackBoneFromTank)
            {
                if (IsMassNeedFromWIPBackBone)
                {
                    var TankSelected = CurrentPlannedSKU.BackBoneSimulation.GetTankAvailable(this);
                    if (TankSelected != null)
                    {
                        StartTank(TankSelected);
                    }
                }

            }
            else if (IsCurrentBackBoneFromMixerAvailable)
            {
                var MixerSelected = CurrentPlannedSKU.BackBoneSimulation.GetMixerAvailableForWIP(this);
                if (IsAbleToStartBatchCurrentBackBone(MixerSelected))
                {
                    StartBatchCurrentBackBone(MixerSelected);
                }
            }
        }


        BaseMixer GetMixer()
        {

            if (CurrentPlannedSKU == null) return null!;
            if (GetEquipmentAtInlet(CurrentPlannedSKU.BackBoneSimulation) is BasePump pump)
            {
                if (pump.GetEquipmentAtInlet(CurrentPlannedSKU.BackBoneSimulation) is BaseMixer mixer)
                {
                    return mixer;
                }

            }


            return null!;
        }
        void CheckMassNeededNextBackBone()
        {
            if (!IsNextBackBoneFromTank && IsNextBackBoneFromMixerAvailable)
            {

                var MixerSelected = NextPlannedSKU.BackBoneSimulation.GetMixerAvailableForWIP(this);
                if (IsAbleToStartBatchNextBackBone(MixerSelected))
                {
                    StartBatchNextBackBone(MixerSelected);
                }
            }
        }
        void CheckMassNeeded()
        {
            if (IsMassNedeedCurrentBackBone)
            {
                CheckMassNeededCurrentBackBone();
            }
            else if (IsMassNedeedNextBackBone)
            {
                CheckMassNeededNextBackBone();
            }

        }
        void StartBatchCurrentBackBone(BaseMixer mixer)
        {
            Amount Capacity = new Amount(0, MassUnits.KiloGram);
            MassCurrentSku.Producing += Capacity;
            MassCurrentSku.Needed -= Capacity;
            AddProcessInletEquipment(mixer.OutletPump);
            mixer.ProducingTo = this;
            mixer.InitBatch(CurrentPlannedSKU.BackBoneSimulation, CurrentDate);

            mixer = null!;
            MassCurrentSku.Batchs++;
        }
        public override void SetInitFromMixer(BaseMixer mixer)
        {
            var addingmas = mixer.MixerState is MixerStateBatching ? mixer.CurrentMixerBackBoneCapacity : mixer.CurrentLevel;
            MassCurrentSku.Producing += addingmas;
            MassCurrentSku.Needed -= addingmas;
            AddProcessInletEquipment(mixer.OutletPump);




            MassCurrentSku.Batchs++;
        }
        void StartBatchNextBackBone(BaseMixer mixer)
        {
            MassNextSku.Producing += mixer.CurrentMixerBackBoneCapacity;
            MassNextSku.Needed -= mixer.CurrentMixerBackBoneCapacity;
            AddProcessInletEquipment(mixer.OutletPump);
            mixer.InitBatch(NextPlannedSKU.BackBoneSimulation, CurrentDate);
            mixer.ProducingTo = this;
            mixer = null!;
            MassNextSku.Batchs++;
        }
        void StartTank(WIPForProductBackBone tank)
        {
            var masstoTransfer = MassCurrentSku.Needed < MasstoTransfer ? MassCurrentSku.Needed : MasstoTransfer;

            tank.ManageMassWIPTank.Delivered = masstoTransfer;
            tank.ManageMassWIPTank.NeedToDelivered = masstoTransfer;
            AddProcessInletEquipment(tank.OutletPump);
            MassCurrentSku.Producing += masstoTransfer;
            MassCurrentSku.Needed -= masstoTransfer;
            MassCurrentSku.Received = new(MassUnits.KiloGram);
            tank.SendingProduct = this;

        }


        public int Batchs => MassCurrentSku.Batchs + MassNextSku.Batchs;
        public bool IsMassNedeedCurrentBackBone => MassCurrentSku.Needed.Value > 0;
        public bool IsMassNedeedNextBackBone => MassNextSku.Needed.Value > 0;
        public bool IsCurrentBackBoneFromTank => CurrentPlannedSKU.BackBoneSimulation == null ? false :
            CurrentPlannedSKU.BackBoneSimulation.Tanks.Any(x => x.ConnectedOutletEquipments.Any(x => x.ConnectedOutletEquipments.Any(x => x.Id == Id)));

        public bool IsNextBackBoneFromTank => NextPlannedSKU.BackBoneSimulation == null ? false :
           NextPlannedSKU.BackBoneSimulation.Tanks.Any(x => x.ConnectedOutletEquipments.Any(x => x.ConnectedOutletEquipments.Any(x => x.Id == Id)));

        public bool IsCurrentBackBoneFromMixerAvailable => IsCurrentBackBoneFromTank ? false : CurrentPlannedSKU.BackBoneSimulation.IsMixerAvailableForWIPTanks(this);
        public bool IsNextBackBoneFromMixerAvailable => IsNextBackBoneFromTank ? false : NextPlannedSKU.BackBoneSimulation.IsMixerAvailableForWIPTanks(this);


        bool IsWIPTankHasCapacityToReceiveMixerWhenFinishBatch(
            BaseMixer MixerSelected, BackBoneStepSimulationCalculation BatchData, PlannedSKUSimulation Plann,
            Amount washouttime, Amount Producing)
        {

            var BatchTime = washouttime + BatchData.BatchCycleTime;
            var capacity=BatchData.Capacity;
            var outletMassDuringBatch = BatchTime * Plann.AverageMassFlow;//Aqui se sumar meter el tiempo de transferencia

            var WIPFinaLevelAfterBatch = CurrentLevel + Producing + capacity - outletMassDuringBatch;

            return WIPFinaLevelAfterBatch <= Capacity;
        }
        Amount Time10Minutes=new(10,TimeUnits.Minute);
        bool IsNeedToStartBatchToAvoidEmptyWIP(BackBoneStepSimulationCalculation BatchData, PlannedSKUSimulation Plann,
            Amount washouttime, Amount Producing, Amount TimeToChangeSKU = null!)
        {
            if (CurrentLevel.Value == 0) return true;
            var BatchTime = washouttime + BatchData.BatchCycleTime + Time10Minutes; // aumenta 10 min para evitar otras perdidas de tiempo no conocidas

            var WIPFinaLevelAfterBatch = CurrentLevel + Producing;
            var OutletFlow = Plann.AverageMassFlow;
            var TimeToFinishCurrentMass = WIPFinaLevelAfterBatch / OutletFlow;
            if (TimeToChangeSKU != null) TimeToFinishCurrentMass += TimeToChangeSKU;


            return TimeToFinishCurrentMass <= BatchTime;
        }
        public Amount TimeToStartNextBatch => GetTimeToStartNextBatch();
        Amount GetTimeToStartNextBatch()
        {
            BaseMixer MixerCurrentBackBone = GetMixer();
            if (CurrentLevel.Value == 0 || MixerCurrentBackBone == null) return new(TimeUnits.Minute);
            if (MassCurrentSku.Needed.Value <= 0) return new(TimeUnits.Minute);
            var BatchData = CurrentPlannedSKU.BackBoneSimulation.BatchDataMixer[MixerCurrentBackBone];
            var washouttime = MixerCurrentBackBone.GetNextWashoutTime(CurrentPlannedSKU.BackBoneSimulation);
            var BatchTime = washouttime + BatchData.BatchCycleTime;

            var WIPFinaLevelAfterBatch = CurrentLevel + MassCurrentSku.Producing;
            var TimeToFinishCurrentMass = WIPFinaLevelAfterBatch / CurrentPlannedSKU.AverageMassFlow;
            if (Line.TimeToChangeSKU != null) TimeToFinishCurrentMass += Line.TimeToChangeSKU;

            TimeToFinishCurrentMass -= BatchTime;
            return new(TimeToFinishCurrentMass.GetValue(TimeUnits.Minute), TimeUnits.Minute);
        }
        public string TimeToStartNextBatchString => TimeToStartNextBatch.ToString("G6");

        bool IsPendingTimeShiftEnoughToReceiveBatch(BackBoneStepSimulationCalculation BatchData, Amount washouttime)
        {
            if (Line.IsNextShiftPlanned) return true;

            var BatchTime = washouttime + BatchData.BatchCycleTime + BatchData.TransferTime;


            return Line.PendingTimeShift > BatchTime;
        }
        bool IsBatchNeedIfOtherBatchIsRunning(Amount TransferTime)
        {
            if (ManufacturingEquipments.Count == 0) return true;

            var mixers = ManufacturingEquipments.Select(x => x as BaseMixer).ToList();

            var MixerAlmostFinishBatch = mixers.MinBy(x => x!.CurrentTime.Value);
            if (MixerAlmostFinishBatch == null) return false;



            return MixerAlmostFinishBatch.CurrentTime >= TransferTime;
        }

        bool IsAbleToStartBatchNextBackBone(BaseMixer MixerSelected)
        {
            if (MixerSelected == null) return false;

            var BatchData = NextPlannedSKU.BackBoneSimulation.BatchDataMixer[MixerSelected];
            var washouttime = MixerSelected.GetNextWashoutTime(NextPlannedSKU.BackBoneSimulation);

            if (!IsWIPTankHasCapacityToReceiveMixerWhenFinishBatch(MixerSelected, BatchData,
                CurrentPlannedSKU, washouttime, MassCurrentSku.Producing + MassNextSku.Producing)) return false;
            if (!IsNeedToStartBatchToAvoidEmptyWIP(BatchData,
                CurrentPlannedSKU, washouttime, MassCurrentSku.Producing + MassNextSku.Producing, Line.TimeToChangeSKU)) return false;
            if (!IsBatchNeedIfOtherBatchIsRunning(BatchData.TransferTime)) return false;
            if (!IsPendingTimeShiftEnoughToReceiveBatch(BatchData, washouttime)) return false;
            return true;
        }

        bool IsAbleToStartBatchCurrentBackBone(BaseMixer MixerSelected)
        {
            if (MixerSelected == null) return false;

            var BatchData = CurrentPlannedSKU.BackBoneSimulation.BatchDataMixer[MixerSelected];
            var washouttime = MixerSelected.GetNextWashoutTime(CurrentPlannedSKU.BackBoneSimulation);

            //Si el tanque tiene capacidad
            if (!IsWIPTankHasCapacityToReceiveMixerWhenFinishBatch(MixerSelected, BatchData,
                CurrentPlannedSKU, washouttime, MassCurrentSku.Producing)) return false;

            //Si por el nivel actual necesita arrancar batch
            if (!IsNeedToStartBatchToAvoidEmptyWIP(BatchData,
                CurrentPlannedSKU, washouttime, MassCurrentSku.Producing)) return false;

            //Si hay algun batch iniciado que no se cruce la transferencia entre los dos
            if (!IsBatchNeedIfOtherBatchIsRunning(BatchData.TransferTime)) return false;

            //Si va aterminar el turno que no arranque el batch
            if (!IsPendingTimeShiftEnoughToReceiveBatch(BatchData, washouttime)) return false;
            return true;
        }


        public override void NewChangeSkuLine()
        {

        }

        public override void ChangeOver()
        {
            if (CurrentPlannedSKU.BackBoneSimulation.Id == NextPlannedSKU.BackBoneSimulation.Id)
            {
                MassNextSku.Batchs += MassCurrentSku.Batchs;
                MassNextSku.Producing += MassCurrentSku.Producing;
                MassNextSku.Needed += MassCurrentSku.Needed;
            }
            CurrentPlannedSKU = Line.CurrentSKU;
            SetCurrentMaterialSimulation(CurrentPlannedSKU.BackBoneSimulation);

            MassCurrentSku.Batchs = MassNextSku.Batchs;
            MassCurrentSku.Producing = MassNextSku.Producing;
            MassCurrentSku.Needed = MassNextSku.Needed;
            MassCurrentSku.Delivered = MassNextSku.Delivered;

            NextPlannedSKU = Line.NextSKU;
            MassNextSku = new();
            if (NextPlannedSKU != null)
            {
                MassNextSku.Needed = NextPlannedSKU.PlannedMassSKU;
                MassNextSku.Delivered = NextPlannedSKU.PlannedMassSKU;
            }

        }
        public override bool ChechLevelForShiftChange()
        {


            if (IsMassNedeedCurrentBackBone) return CheckLevelForBackBone(CurrentPlannedSKU);
            if (IsMassNedeedNextBackBone) return CheckLevelForBackBone(NextPlannedSKU, Line.TimeToChangeSKU);

            return false;

        }


        bool IsPendingLineTimeLessThanBatchTime(PlannedSKUSimulation sKUSimulation,
            BackBoneStepSimulationCalculation BatchData, Amount Producing, Amount washouttime)
        {
            var TotalTime = washouttime + BatchData.BatchCycleTime;

            var MasPendingToDeliver = CurrentLevel + Producing;
            var flowLine = sKUSimulation.AverageMassFlow;
            var timePendingToDelivereMass = MasPendingToDeliver / flowLine;

            var TimeBatchNeeded = TotalTime - timePendingToDelivereMass;


            return Line.PendingTimeShift <= TimeBatchNeeded;
        }

        bool CheckLevelForBackBone(PlannedSKUSimulation sKUSimulation, Amount TimeToChangeSKU = null!)
        {
            var MixerSelected = sKUSimulation.BackBoneSimulation.GetMixerAvailableForWIP(this);
            if (MixerSelected == null) return false;
            var BatchData = CurrentPlannedSKU.BackBoneSimulation.BatchDataMixer[MixerSelected];
            var washouttime = MixerSelected.GetNextWashoutTime(CurrentPlannedSKU.BackBoneSimulation);

            //Si el tanque tiene capacidad
            if (!IsWIPTankHasCapacityToReceiveMixerWhenFinishBatch(MixerSelected, BatchData,
                CurrentPlannedSKU, washouttime, MassCurrentSku.Producing)) return false;

            //Si por el nivel actual necesita arrancar batch
            if (!IsNeedToStartBatchToAvoidEmptyWIP(BatchData,
                CurrentPlannedSKU, washouttime, MassCurrentSku.Producing, TimeToChangeSKU)) return false;

            //Si el tiempo de batch alcanzapara iniciar
            if (!IsPendingLineTimeLessThanBatchTime(sKUSimulation, BatchData, MassCurrentSku.Producing, washouttime)) return false;
            return true;
        }

    }


}
