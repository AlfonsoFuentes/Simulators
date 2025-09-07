using Simulator.Shared.Models.HCs.Tanks;
using Simulator.Shared.Simulations.Materials;
using Simulator.Shared.Simulations.Mixers;
using Simulator.Shared.Simulations.Mixers.States.MixerCalculations.MixerStates;
using Simulator.Shared.Simulations.Tanks.WIPLinetMixers;

namespace Simulator.Shared.Simulations.Tanks
{
    public class BackBoneRawMaterialTank : WIPTank
    {

        BackBoneSimulation CurrentBackBone => (BackBoneSimulation)CurrentMaterialSimulation;
        public bool GetEquipmentOcupiedBy => ConnectedOutletEquipments.Any(x => x.ConnectedOutletEquipments.Count > 0) || ConnectedInletEquipments.Any(x => x.ConnectedInletEquipments.Count > 0);

        public BackBoneRawMaterialTank(TankDTO tankDTO) : base(tankDTO)
        {
            TankDTO = tankDTO;

            CalculateForOutlet = CalculateFromOutlet;
            CalculateForInlet = CalculateFromMixers;


        }
        protected void CalculateFromOutlet()
        {
            CurrentTime += OneSecond;
            CurrentLevel -= Outlet;
            TotalMassOutlet += Outlet;
            if (CurrentTime.GetValue(TimeUnits.Second) > 10)
                AverageOutlet = TotalMassOutlet / CurrentTime;
            Outlet.SetValue(0, MassUnits.KiloGram);

        }


        public override void Init()
        {

            CurrentLevel = TankDTO.InitialLevel;





        }
        public ManageMassWIPTank ManageMassWIPTank { get; set; } = new();
        public int BatchsProduced => ManageMassWIPTank.Batchs;
        public Amount ProducingMass => ManageMassWIPTank.Producing;
        protected void CalculateFromMixers()
        {


            CurrentLevel += Inlet;
            ManageMassWIPTank.Producing -= Inlet;
            Inlet.SetValue(0, MassUnits.KiloGram);



        }

        public override void Calculate(DateTime currentdate)
        {
            base.Calculate(currentdate);
            CheckMassNeededCurrentBackBone();
        }

        public override bool GetTankAvailableforTransferMixer(BaseMixer mixer)
        {
            if (InletOcupiedBy != null) return false;
            if (mixer.CurrentBackBoneSimulation.Id != CurrentBackBone.Id) return false;


            var transfertime = CurrentBackBone.BatchDataMixer[mixer].TransferTime;
            var outletduringTransfering = AverageOutlet * transfertime;
            var futurelevel = CurrentLevel - outletduringTransfering + mixer.CurrentLevel;


            return futurelevel <= Capacity;
        }
        void CheckMassNeededCurrentBackBone()
        {
            if (IsCurrentBackBoneFromMixerAvailable)
            {
                var MixerSelected = CurrentBackBone.GetMixerAvailableForWIP(this);
                if (IsAbleToStartBatchCurrentBackBone(MixerSelected))
                {
                    StartBatchCurrentBackBone(MixerSelected);
                }
            }
        }
        public int Batchs => ManageMassWIPTank.Batchs;
        public bool IsCurrentBackBoneFromMixerAvailable => CurrentBackBone.IsMixerAvailableForWIPTanks(this);
        void StartBatchCurrentBackBone(BaseMixer mixer)
        {
            if (CurrentBackBone.BatchDataMixer.ContainsKey(mixer))
            {
                ManageMassWIPTank.Producing += CurrentBackBone.BatchDataMixer[mixer].Capacity;

                AddProcessInletEquipment(mixer.OutletPump);
                mixer.ProducingTo = this;
                mixer.InitBatch(CurrentBackBone, CurrentDate);

                mixer = null!;
                ManageMassWIPTank.Batchs++;
            }


        }
        bool IsWIPTankHasCapacityToReceiveMixerWhenFinishBatch(
            BaseMixer MixerSelected, BackBoneStepSimulationCalculation BatchData,
            Amount washouttime, Amount Producing)
        {
            var TransferTime = BatchData.TransferTime;
            var BatchTime = washouttime + BatchData.BatchCycleTime + TransferTime;
            var MixerCapacity = BatchData.Capacity;

            var outletMassDuringBatch = BatchTime * AverageOutlet;

            var WIPFinaLevelAfterBatch = CurrentLevel + Producing + MixerCapacity - outletMassDuringBatch;

            return WIPFinaLevelAfterBatch <= Capacity;
        }
        bool IsNeedToStartBatchToAvoidEmptyWIP(BackBoneStepSimulationCalculation BatchData,
            Amount washouttime, Amount Producing)
        {
            if (CurrentLevel.Value == 0) return true;
            var BatchTime = washouttime + BatchData.BatchCycleTime;

            var WIPFinaLevelAfterBatch = CurrentLevel + Producing;
            var OutletFlow = AverageOutlet;
            var TimeToFinishCurrentMass = WIPFinaLevelAfterBatch / OutletFlow;


            return TimeToFinishCurrentMass <= BatchTime;
        }
        bool IsBatchNeedIfOtherBatchIsRunning(Amount TransferTime)
        {
            if (ManufacturingEquipments.Count == 0) return true;

            var mixers = ManufacturingEquipments.Select(x => x as BaseMixer).ToList();

            var MixerAlmostFinishBatch = mixers.MinBy(x => x!.CurrentTime.Value);
            if (MixerAlmostFinishBatch == null) return false;



            return MixerAlmostFinishBatch.CurrentTime >= TransferTime;
        }

        bool IsAbleToStartBatchCurrentBackBone(BaseMixer MixerSelected)
        {
            if (MixerSelected == null) return false;

            var BatchData = CurrentBackBone.BatchDataMixer[MixerSelected];
            var washouttime = MixerSelected.GetNextWashoutTime(CurrentBackBone);

            //Si el tanque tiene capacidad
            if (!IsWIPTankHasCapacityToReceiveMixerWhenFinishBatch(MixerSelected, BatchData,
                washouttime, ManageMassWIPTank.Producing)) return false;

            //Si por el nivel actual necesita arrancar batch
            if (!IsNeedToStartBatchToAvoidEmptyWIP(BatchData,
                washouttime, ManageMassWIPTank.Producing)) return false;

            //Si hay algun batch iniciado que no se cruce la transferencia entre los dos
            if (!IsBatchNeedIfOtherBatchIsRunning(BatchData.TransferTime)) return false;

            //Si va aterminar el turno que no arranque el batch

            return true;
        }
    }


}
