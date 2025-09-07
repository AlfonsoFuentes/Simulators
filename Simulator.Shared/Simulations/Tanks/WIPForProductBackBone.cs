using Simulator.Shared.Models.HCs.Tanks;
using Simulator.Shared.Simulations.Materials;
using Simulator.Shared.Simulations.Mixers;
using Simulator.Shared.Simulations.Tanks.WIPLinetMixers;

namespace Simulator.Shared.Simulations.Tanks
{
    public class WIPForProductBackBone : WIPTank
    {
        public string SendingProductName => SendingProduct == null ? string.Empty : SendingProduct.Name;
        public WIPInletMixer SendingProduct { get; set; } = null!;
        public ManageMassWIPTank ManageMassWIPTank { get; set; } = new();

        BackBoneSimulation CurrentBackBone => (BackBoneSimulation)CurrentMaterialSimulation;

        public override bool IsTankLoLevel => CurrentLevel <= TankDTO.MinLevel;
        Amount ZeroFlow = new(MassFlowUnits.Kg_min);
        public WIPForProductBackBone(TankDTO tankDTO) : base(tankDTO)
        {

            TankDTO = tankDTO;

            CalculateForInlet = CalculateFromMixers;

            CalculateForOutlet = CalculateFromOutlet;
            

        }

        void CheckOutLetTransfer()
        {
            if (SendingProduct != null)
            {
                if (ManageMassWIPTank.NeedToDelivered == SendingProduct.MassCurrentSku.Received)
                {
                    SetOutletFlow(ZeroFlow);
                    SendingProduct.SetInletFlow(ZeroFlow);
                    OutletPump.RemoveProcessOutletEquipment(SendingProduct);

                    SendingProduct = null!;
                }
                else
                {
                    var reach = ManageMassWIPTank.Delivered <= OutletPump.Flow * OneSecond;

                    var mass = ManageMassWIPTank.Delivered < OutletPump.Flow * OneSecond ? ManageMassWIPTank.Delivered : OutletPump.Flow * OneSecond;

                    SetOutletFlow(mass / OneSecond);
                    SendingProduct.SetInletFlow(mass / OneSecond);
                }


            }
        }
        public override void Calculate(DateTime currentdate)
        {
            base.Calculate(currentdate);
            CheckMassNeededCurrentBackBone();
            CheckOutLetTransfer();
        }
        protected void CalculateFromOutlet()
        {

            CurrentLevel -= Outlet;
            TotalMassOutlet += Outlet;
            TotalMassOutletShift += Outlet;
            if (SendingProduct != null)
                ManageMassWIPTank.Delivered -= (ManageMassWIPTank.Delivered - Outlet).Value < 0 ? ManageMassWIPTank.Delivered : Outlet;
            AverageOutlet = CurrentTime.GetValue(TimeUnits.Minute) > 10 ? TotalMassOutlet / CurrentTime : new(MassFlowUnits.Kg_min);

            Outlet.SetValue(0, MassUnits.KiloGram);

        }
        public int BatchsProduced => ManageMassWIPTank.Batchs;
        public Amount ProducingMass => ManageMassWIPTank.Producing;

       
        public override void Init()
        {
            CurrentLevel = TankDTO.InitialLevel;
     



        }
        public bool GetEquipmentOcupiedBy =>
            ConnectedOutletEquipments.Any(x => x.ConnectedOutletEquipments.Count > 0)
            || ConnectedInletEquipments.Any(x => x.ConnectedInletEquipments.Count > 0) || IsEquipmentHasQueue;

        protected void CalculateFromMixers()
        {


            CurrentLevel += Inlet;
            ManageMassWIPTank.Producing -= Inlet;
            Inlet.SetValue(0, MassUnits.KiloGram);



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

        void StartBatchCurrentBackBone(BaseMixer mixer)
        {
            //de pronto hay que ponerlo despues de initbatch
            ManageMassWIPTank.Producing += mixer.CurrentMixerBackBoneCapacity;

            AddProcessInletEquipment(mixer.OutletPump);
            mixer.InitBatch(CurrentBackBone, CurrentDate);
            mixer.ProducingTo = this;
            mixer = null!;
            ManageMassWIPTank.Batchs++;
        }

        public int Batchs => ManageMassWIPTank.Batchs;
        public bool IsCurrentBackBoneFromMixerAvailable => CurrentBackBone.IsMixerAvailableForWIPTanks(this);

        bool IsWIPTankHasCapacityToReceiveMixerWhenFinishBatch(
            BaseMixer MixerSelected, BackBoneStepSimulationCalculation BatchData,
            Amount washouttime, Amount Producing)
        {
            var BatchTime = washouttime + BatchData.BatchCycleTime;

            var outletMassDuringBatch = BatchTime * AverageOutlet;

            var WIPFinaLevelAfterBatch = CurrentLevel + Producing + MixerSelected.CurrentMixerBackBoneCapacity - outletMassDuringBatch;

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

