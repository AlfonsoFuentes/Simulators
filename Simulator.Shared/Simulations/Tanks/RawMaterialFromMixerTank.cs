namespace Simulator.Shared.Simulations.Tanks
{
    //public class RawMaterialFromMixerTank : BaseTank
    //{
    //    public List<NewBaseEquipment> ManufacturingEquipments { get; set; } = new();
    //    BackBoneSimulation CurrentBackBone => (BackBoneSimulation)CurrentMaterialSimulation;
    //    public RawMaterialFromMixerTank(TankDTO tankDTO) : base(tankDTO)
    //    {
    //        TankDTO = tankDTO;

    //        CalculateForOutlet = CalculateFromOutlet;
    //        CalculateForInlet = CalculateFromMixers;


    //    }
    //    protected void CalculateFromOutlet()
    //    {
    //        CurrentTime += OneSecond;
    //        CurrentLevel -= Outlet;
    //        TotalMassOutlet += Outlet;
    //        if (CurrentTime.GetValue(TimeUnits.Minute) > 10)
    //            AverageOutlet = TotalMassOutlet / CurrentTime;
    //        Outlet.SetValue(0, MassUnits.KiloGram);

    //    }


    //    public override void Init()
    //    {

    //        CurrentLevel = TankDTO.InitialLevel;

    //        SetMaterialsOutlet();



    //    }
    //    public ManageMassWIPTank ManageMassWIPTank { get; set; } = new();
    //    public int BatchsProduced => ManageMassWIPTank.Batchs;
    //    public Amount ProducingMass => ManageMassWIPTank.Producing;
    //    public bool GetEquipmentOcupiedBy => ConnectedOutletEquipments.Any(x => x.ConnectedOutletEquipments.Count > 0) || ConnectedInletEquipments.Any(x => x.ConnectedInletEquipments.Count > 0);

    //    protected void CalculateFromMixers()
    //    {


    //        CurrentLevel += Inlet;
    //        ManageMassWIPTank.Producing -= Inlet;
    //        Inlet.SetValue(0, MassUnits.KiloGram);



    //    }

    //    public override void Calculate(DateTime currentdate)
    //    {
    //        base.Calculate(currentdate);
    //        CheckMassNeededCurrentBackBone();
    //    }

    //    public override bool GetTankAvailableforTransferMixer(BaseMixer mixer)
    //    {
    //        if (InletOcupiedBy != null) return false;
    //        if (mixer.CurrentBackBoneSimulation.Id != CurrentBackBone.Id) return false;


    //        var transfertime = CurrentBackBone.BatchData[mixer].TransferTime;
    //        var outletduringTransfering = AverageOutlet * transfertime;
    //        var futurelevel = CurrentLevel - outletduringTransfering + mixer.CurrentLevel;


    //        return futurelevel <= Capacity;
    //    }
    //    void CheckMassNeededCurrentBackBone()
    //    {
    //        if (IsCurrentBackBoneFromMixerAvailable)
    //        {
    //            var MixerSelected = CurrentBackBone.GetMixerAvailableForWIP(this);
    //            if (IsAbleToStartBatchCurrentBackBone(MixerSelected))
    //            {
    //                StartBatchCurrentBackBone(MixerSelected);
    //            }
    //        }
    //    }
    //    public int Batchs => ManageMassWIPTank.Batchs;
    //    public bool IsCurrentBackBoneFromMixerAvailable => CurrentBackBone.IsMixerAvailableForWIPTanks(this);
    //    void StartBatchCurrentBackBone(BaseMixer mixer)
    //    {

    //        ManageMassWIPTank.Producing += mixer.Capacity;

    //        AddProcessInletEquipment(mixer.OutletPump);
    //        mixer.InitBatch(CurrentBackBone, CurrentDate);
    //        mixer.ProducingTo = this;
    //        mixer = null!;
    //        ManageMassWIPTank.Batchs++;
    //    }
    //    bool IsWIPTankHasCapacityToReceiveMixerWhenFinishBatch(
    //        BaseMixer MixerSelected, BackBoneStepSimulationCalculation BatchData,
    //        Amount washouttime, Amount Producing)
    //    {
    //        var BatchTime = washouttime + BatchData.BatchCycleTime;

    //        var outletMassDuringBatch = BatchTime * AverageOutlet;

    //        var WIPFinaLevelAfterBatch = (CurrentLevel + Producing) + MixerSelected.Capacity - outletMassDuringBatch;

    //        return WIPFinaLevelAfterBatch <= Capacity;
    //    }
    //    bool IsNeedToStartBatchToAvoidEmptyWIP(BackBoneStepSimulationCalculation BatchData,
    //        Amount washouttime, Amount Producing)
    //    {
    //        if (CurrentLevel.Value == 0) return true;
    //        var BatchTime = washouttime + BatchData.BatchCycleTime;

    //        var WIPFinaLevelAfterBatch = (CurrentLevel + Producing);
    //        var OutletFlow = AverageOutlet;
    //        var TimeToFinishCurrentMass = OutletFlow.Value == 0 ? new(TimeUnits.Minute) : WIPFinaLevelAfterBatch / OutletFlow;


    //        return TimeToFinishCurrentMass <= BatchTime;
    //    }
    //    bool IsBatchNeedIfOtherBatchIsRunning(Amount TransferTime)
    //    {
    //        if (ManufacturingEquipments.Count == 0) return true;

    //        var mixers = ManufacturingEquipments.Select(x => x as BaseMixer).ToList();

    //        var MixerAlmostFinishBatch = mixers.MinBy(x => x!.CurrentTime.Value);
    //        if (MixerAlmostFinishBatch == null) return false;



    //        return MixerAlmostFinishBatch.CurrentTime >= TransferTime;
    //    }

    //    bool IsAbleToStartBatchCurrentBackBone(BaseMixer MixerSelected)
    //    {
    //        if (MixerSelected == null) return false;

    //        var BatchData = CurrentBackBone.BatchData[MixerSelected];
    //        var washouttime = MixerSelected.GetNextWashoutTime(CurrentBackBone);

    //        //Si el tanque tiene capacidad
    //        if (!IsWIPTankHasCapacityToReceiveMixerWhenFinishBatch(MixerSelected, BatchData,
    //            washouttime, ManageMassWIPTank.Producing)) return false;

    //        //Si por el nivel actual necesita arrancar batch
    //        if (!IsNeedToStartBatchToAvoidEmptyWIP(BatchData,
    //            washouttime, ManageMassWIPTank.Producing)) return false;

    //        //Si hay algun batch iniciado que no se cruce la transferencia entre los dos
    //        if (!IsBatchNeedIfOtherBatchIsRunning(BatchData.TransferTime)) return false;

    //        //Si va aterminar el turno que no arranque el batch

    //        return true;
    //    }
    //}
}