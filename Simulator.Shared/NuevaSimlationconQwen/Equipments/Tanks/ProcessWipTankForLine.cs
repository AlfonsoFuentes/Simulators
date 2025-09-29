using Simulator.Shared.Enums.HCEnums.Enums;
using Simulator.Shared.NuevaSimlationconQwen.Equipments.Lines;
using Simulator.Shared.NuevaSimlationconQwen.Equipments.Mixers;
using Simulator.Shared.NuevaSimlationconQwen.Equipments.Pumps;
using Simulator.Shared.NuevaSimlationconQwen.Equipments.Skids;
using Simulator.Shared.NuevaSimlationconQwen.ManufacturingOrders;
using Simulator.Shared.NuevaSimlationconQwen.Materials;
using Simulator.Shared.NuevaSimlationconQwen.Reports;
using Simulator.Shared.Simulations.Tanks;

namespace Simulator.Shared.NuevaSimlationconQwen.Equipments.Tanks
{
    public class ProcessWipTankForLine : ProcessBaseTank, ILiveReportable
    {

        public IVesselManufactureOrder NextOrder { get; set; } = null!;

        public IVesselManufactureOrder CurrentOrder { get; set; } = null!;
        public TankCalculationType TankCalculationType { get; set; } = TankCalculationType.None;

        public List<ProcessMixer> InletMixers => InletEquipments.SelectMany(x => x.InletEquipments.OfType<ProcessMixer>().ToList()).ToList();
        public List<ProcessContinuousSystem> InletSKIDS => InletEquipments.OfType<ProcessContinuousSystem>().ToList().ToList();
        List<ManufaturingEquipment> ManufactureAttached => [.. InletSKIDS, .. InletMixers];

        public ProcessPump? WIPTankPump => OutletPumps.FirstOrDefault();

        public override void ValidateOutletInitialState(DateTime currentdate)
        {
            CurrentLevel = InitialLevel;
            OutletState = new ProcessWipTankOutletInitializeTankState(this);



        }

        public Queue<TransferFromMixertoWIPOrder> TransfersOrdersFromMixers { get; set; } = new Queue<TransferFromMixertoWIPOrder>();
        private TransferFromMixertoWIPOrder? CurrentTransferFromMixer { get; set; }

        public void ReceiveTransferRequestFromMixer(TransferFromMixertoWIPOrder order)
        {
            TransfersOrdersFromMixers.Enqueue(order);
        }


        public void SetCurrentMassTransfered()
        {
            if (CurrentTransferFromMixer != null)
            {
                Amount massTransfered = CurrentTransferFromMixer.TransferFlow * OneSecond;
                if (CurrentTransferFromMixer.PendingToReceive <= massTransfered)
                {
                    massTransfered = CurrentTransferFromMixer.PendingToReceive;
                }
                CurrentLevel += massTransfered;
                CurrentTransferFromMixer.MassReceived += massTransfered;
                CurrentTransferFromMixer.SourceMixer.ReceiveReportOfMassDelivered(massTransfered);
            }
        }

        public bool ReviewIfTransferCanInit()
        {
            if (TransfersOrdersFromMixers.Count == 0) return false;

            CurrentTransferFromMixer = TransfersOrdersFromMixers.Dequeue();
            CurrentTransferFromMixer.SourceMixer.ReceiveTransferOrderFromWIPToInit(CurrentTransferFromMixer);
            return true;
        }
        public void ReceiveFromMixer()
        {
            if (CurrentTransferFromMixer != null && CurrentOrder != null)
            {
                var masstoreceive = CurrentTransferFromMixer.TransferFlow * OneSecond;
                if (CurrentTransferFromMixer.PendingToReceive < masstoreceive)
                {
                    masstoreceive = CurrentTransferFromMixer.PendingToReceive;
                }
                CurrentLevel += masstoreceive;

                CurrentTransferFromMixer.MassReceived += masstoreceive;
                CurrentTransferFromMixer.SourceMixer.ReceiveReportOfMassDelivered(masstoreceive);
            }
        }
        public bool IsTransferFinalized()
        {
            if (CurrentTransferFromMixer != null)
            {
                if (CurrentTransferFromMixer.PendingToReceive <= ZeroMass)
                {
                    return true;
                }
            }
            return false;
        }
        public void ReportStarvedTransferToMixer()
        {
            if (CurrentTransferFromMixer != null)
            {
                CurrentTransferFromMixer.SourceMixer.ReceiveTransferStarvedFromWIP();
            }
        }
        public void ReportReinitTransferToMixer()
        {
            if (CurrentTransferFromMixer != null)
            {
                CurrentTransferFromMixer.SourceMixer.ReceiveTransferReInitStarvedFromWIP();
            }
        }
        public bool IsReportFinishTransferToMixer()
        {
            if (CurrentTransferFromMixer != null)
            {
                CurrentTransferFromMixer.SourceMixer.ReceiveTransferFinalizedFromWIP();
                CurrentTransferFromMixer = null;
            }
            return true;
        }
        public bool ReviewIfTransferCanReinit()
        {
            if (CurrentTransferFromMixer != null)
            {
                if (Capacity - CurrentLevel >= CurrentTransferFromMixer.PendingToReceive)
                {
                    ReportReinitTransferToMixer();
                    return true;
                }

                return false;
            }
            return false;
        }
        public override void CalculateOutletLevel()
        {
            var mass = OutletFlows * OneSecond;

            CurrentLevel -= mass;
            if (CurrentOrder != null)
            {
                CurrentOrder.AddMassDelivered(mass);
                CurrentOrder.AddRunTime();

            }
        }

        public void ReceiveInitFromLineProductionOrder(FromLineToWipProductionOrder order)
        {
            var manufactures = ManufactureAttached.FirstOrDefault(x => x.EquipmentMaterials.Any(x => x.Material.Id == order.Material.Id));
            if (manufactures != null)
            {
                if (manufactures is ProcessContinuousSystem skid)
                {
                    if (CurrentOrder == null)
                    {
                        CurrentOrder = new WIPInletSKIDManufacturingOrder(this, order);


                        OutletManufactureOrderIsReceived = true;
                        order.ReceiveWipCanHandleMaterial(this);
                        CurrentOrder.AddMassProduced(CurrentLevel);
                    }
                    else
                    {
                        NextOrder = new WIPInletSKIDManufacturingOrder(this, order);

                        order.ReceiveWipCanHandleMaterial(this);
                    }
                }
                else
                {
                    if (CurrentOrder == null)
                    {
                        CurrentOrder = new WIPInletMixerManufacturingOrder(this, order);


                        OutletManufactureOrderIsReceived = true;
                        order.ReceiveWipCanHandleMaterial(this);
                        CurrentOrder.AddMassProduced(CurrentLevel);
                    }
                    else
                    {
                        NextOrder = new WIPInletMixerManufacturingOrder(this, order);

                        order.ReceiveWipCanHandleMaterial(this);
                    }
                }

            }


        }

        bool OutletManufactureOrderIsReceived = false;
        bool InletManufactureOrderIsReceived = false;
        public bool IsNewOrderReceived()
        {
            if (OutletManufactureOrderIsReceived)
            {
                IsInletStateSelected();
                OutletManufactureOrderIsReceived = false;
                return true;
            }

            return false;
        }
        public bool IsCurrentOrderRealesed()
        {
            if (NextOrder != null)
            {
                CurrentOrder = NextOrder;
                NextOrder = null!;
                OutletManufactureOrderIsReceived = true;
                return true;
            }
            CurrentOrder = null!;

            return true;
        }
        public bool IsTankHigherThenHiLevelForMixer()
        {
            if (CurrentLevel >= HiLevel)
            {
                ReportStarvedTransferToMixer();
                return true;
            }
            return false;
        }
        public bool IsTankHigherThenHiLevel()
        {
            if (CurrentLevel >= HiLevel)
            {
                StopSkid();
                return true;
            }
            return false;
        }
        public bool IsTankIsLowerThanLowLevel()
        {
            if (CurrentOrder == null) return false;

            if (CurrentLevel <= LoLevel)
            {
                StartSkid();
                return true;
            }
            return false;
        }
        public bool IsTankLowerThanLowLowLevel()
        {
            if (CurrentOrder == null) return false;

            if (CurrentLevel <= LoLolevel)
            {
                StartSkid();
                return true;
            }
            return false;
        }
        public bool IsTankHigherThanLowLevel()
        {
            if (CurrentLevel >= LoLevel)
            {
                return true;
            }
            return false;
        }

        public void StartSkid()
        {
            InletSKIDS.ForEach(x => x.Produce());

        }
        public void StopSkid()
        {
            InletSKIDS.ForEach(x => x.Stop());

        }
        public void ReceiveProductFromSKID(Amount flow)
        {
            var mass = flow * OneSecond;
            CurrentLevel += mass;
            if (CurrentOrder != null)
            {
                CurrentOrder.AddMassProduced(mass);
            }
        }
        public bool IsInletStateSelected()
        {
            if (CurrentOrder != null)
            {
                var manufactures = ManufactureAttached.FirstOrDefault(x => x.EquipmentMaterials.Any(x => x.Material.Id == CurrentOrder.Material.Id));
                if (manufactures != null && manufactures is ProcessContinuousSystem skid)
                {
                    InletSKIDS.ForEach(x => x.ReceiveManufactureOrderFromWIP(CurrentOrder));
                    InletState = new TankInletIniateSKIDState(this);
                    InletManufactureOrderIsReceived = true;
                    return true;
                }
                else
                {
                    InletState = new TankInletIniateMixerState(this);
                    InletManufactureOrderIsReceived = true;
                    return true;
                }

            }

            return false;
        }
        public bool IsMixerWipToProducedCompleted()
        {
            if (CurrentOrder != null)
            {
                if (TransfersOrdersFromMixers.Count > 0 || CurrentTransferFromMixer != null || CurrentOrder.ManufactureOrdersFromMixers.Count > 0)
                {
                    return false;
                }
                if (CurrentOrder.MassPendingToProduce <= ZeroMass)
                {
                    CurrentOrder.SendToLineCurrentOrderIsProduced();

                    return true;
                }

            }


            return false;
        }
        public bool IsSKIDWIPProducedCompleted()
        {

            if (CurrentOrder.MassPendingToProduce <= ZeroMass)
            {
                if (!CurrentOrder.IsSendToLineCurrentOrderIsProduced)
                    CurrentOrder.SendToLineCurrentOrderIsProduced();

                return true;
            }



            return false;
        }

        public bool IsMassDeliveredCompleted()
        {
            if (CurrentOrder != null)
            {
                if (CurrentOrder.MassPendingToDeliver <= ZeroMass)
                {
                    return true;
                }
            }
            return false;
        }
        bool FinshingCurrentOrderReceived = false;
        public void ReceiveFinishedorder()
        {
            FinshingCurrentOrderReceived = true;
        }
        public bool IsFinshingOrderReceived()
        {
            if (FinshingCurrentOrderReceived)
            {
                FinshingCurrentOrderReceived = false;
                return true;
            }
            return false;
        }

        public bool IsOuletAvailable()
        {
            if (OutletState is ProcessWipTankOutletAvailableState)
            {
                return true;
            }
            return false;
        }
        public bool IsNewOrderReceivedToStartOrder()
        {
            if (InletManufactureOrderIsReceived)
            {
                InletManufactureOrderIsReceived = false;
                return true;
            }
            return false;
        }
        public bool IsInletSKIDFinalizedOrder()
        {
            InletSKIDS.ForEach(x => x.ReceiveTotalStop());

            return true;
        }

        public bool IsMaterialNeeded()
        {
            if (CurrentOrder is null) return false;

            if (CurrentOrder.MassPendingToProduce <= ZeroMass) return false;


            var plan = GetTimeToProduceProduct(CurrentOrder.Line, CurrentOrder.Material);
            if (plan.SelectedMixer is null || plan.SelectedRecipe is null)
                return false;

            if (CurrentOrder.TotalMassStoragedOrProducing.Value == 0)
            {
                return TryToStartNewOrder(CurrentOrder, plan.SelectedMixer, plan.SelectedRecipe); ;
            }
            var futurelevel = CurrentOrder.TotalMassStoragedOrProducing + plan.SelectedRecipe.BatchSize;
            if (CurrentOrder.TimeToEmptyMassInProcess.Value > 0)
            {
                var massoutletduringBatch = plan.TotalBatchTime * 0.85 * CurrentOrder.AverageOutletFlow;
                futurelevel -= massoutletduringBatch;

            }
            if (CurrentTransferFromMixer != null)
            {
                futurelevel += CurrentTransferFromMixer.PendingToReceive;
            }
            if (futurelevel <= Capacity)
            {
                return TryToStartNewOrder(CurrentOrder, plan.SelectedMixer, plan.SelectedRecipe);
            }

            return false;

        }
        public bool TryToStartNewOrder(IVesselManufactureOrder order, ManufaturingEquipment mixer, IEquipmentMaterial recipe)
        {
            if (order is null) return false;
            var lastMixer = order.LastInOrder;
            if (lastMixer is null)
            {
                StartNewOrder(order, mixer);
                return true;
            }
            if (lastMixer.Mixer.CurrentManufactureOrder.CurrentBatchTime > recipe.TransferTime)
            {
                StartNewOrder(order, mixer);
                return true;
            }
            return false;
        }
        public void StartNewOrder(IVesselManufactureOrder order, ManufaturingEquipment mixer)
        {


            mixer.ReceiveManufactureOrderFromWIP(order);
        }

        (Amount TotalBatchTime, Amount BatchTime, ManufaturingEquipment SelectedMixer, IEquipmentMaterial SelectedRecipe)
            GetTimeToProduceProduct(ProcessLine Line, IMaterial Material)
        {
            var selectedMixerMaterial = SelectCandidateMixers(Line, Material);
            if (selectedMixerMaterial.MixerCandidate is null)
                return (new Amount(0, TimeUnits.Minute), new Amount(0, TimeUnits.Minute), null!, null!);

            //Added ten minutes by Unknow delays
            Amount TenMinutes = new Amount(10, TimeUnits.Minute);

            var washoutTime = GetWashoutTime(selectedMixerMaterial.MixerCandidate, Material);
            var batchTime = selectedMixerMaterial.Recipe.BatchCycleTime;
            var totalBatchtTime = batchTime + washoutTime;
            var transferTime = selectedMixerMaterial.Recipe.TransferTime;
            var totalTime = washoutTime + transferTime + batchTime;

            return (totalTime, totalBatchtTime, selectedMixerMaterial.MixerCandidate, selectedMixerMaterial.Recipe);
        }
        (ManufaturingEquipment MixerCandidate, IEquipmentMaterial Recipe) SelectCandidateMixers(ProcessLine Line, IMaterial Material)
        {
            if (Material is null) return (null!, null!);
            IEquipmentMaterial materialFromMixer = null!;
            // 1. Preferidos libres
            if (Line.PreferredManufacturer.Any())
            {
                var mixer = Line.PreferredManufacturer
                    .FirstOrDefault(x => x.EquipmentMaterials.Any(m => m.Material.Id == Material.Id) && x.CurrentManufactureOrder == null);
                if (mixer != null)
                {
                    materialFromMixer = SelectMaterialFromMixer(mixer, Material);
                    return (mixer, materialFromMixer);
                }
            }
            var mixers = Line.InletPumps.SelectMany(x => x.InletWipTanks.SelectMany(x => x.InletMixers)).ToList();
            // 2. Todos los mezcladores que producen el material
            var allMixersThatProduceMaterial = mixers
                .Where(x => x.EquipmentMaterials.Any(m => m.Material.Id == Material.Id))
                .ToList();

            if (allMixersThatProduceMaterial.Count == 0) return (null!, null!);

            // 3. Si alguno está libre → devolver el primero

            var freeMixers = allMixersThatProduceMaterial.Where(x => x.CurrentManufactureOrder == null).ToList();
            var freeMixer = freeMixers.RandomElement(); // ← ¡Así de simple!
            if (freeMixer != null)
            {
                materialFromMixer = SelectMaterialFromMixer(freeMixer, Material);
                return (freeMixer, materialFromMixer);
            }

            // 4. Todos ocupados → buscar el primero que pueda encolar (batchTime > transferTime)
            var orderedMixers = allMixersThatProduceMaterial
                .OrderBy(x => x.CurrentManufactureOrder.CurrentBatchTime.GetValue(TimeUnits.Minute))
                .ToList();

            foreach (var candidate in orderedMixers)
            {
                materialFromMixer = SelectMaterialFromMixer(candidate, Material);
                // Asegúrate de que materialFromMixer no sea null
                if (materialFromMixer != null &&
                    candidate.CurrentManufactureOrder.CurrentBatchTime > materialFromMixer.TransferTime)
                {

                    return (candidate, materialFromMixer); // ¡Encontramos uno que puede encolar!
                }
            }
            var FirstMixer = orderedMixers.FirstOrDefault();
            if (FirstMixer != null)
            {
                materialFromMixer = SelectMaterialFromMixer(FirstMixer!, Material);
                // 5. Si ninguno puede encolar → devolver el que termine primero
                return (FirstMixer, materialFromMixer);
            }
            materialFromMixer = SelectMaterialFromMixer(FirstMixer!, Material);
            // 5. Si ninguno puede encolar → devolver el que termine primero
            return (null!, null!);
        }

        Amount GetWashoutTime(ManufaturingEquipment mixer, IMaterial material)
        {
            Amount washoutTime = new Amount(0, TimeUnits.Minute);
            if (mixer.LastMaterial != null)
            {

                var washoutDef = mixer.WashoutTimes
                                .FirstOrDefault(x => x.ProductCategoryCurrent == mixer.LastMaterial.ProductCategory &&
                                                   x.ProductCategoryNext == material.ProductCategory);

                if (washoutDef != null)
                {
                    washoutTime = washoutDef.MixerWashoutTime;
                }
            }
            return washoutTime;
        }
        IEquipmentMaterial SelectMaterialFromMixer(ManufaturingEquipment mixer, IMaterial material)
        {

            var materialFoundFromMixer = mixer.EquipmentMaterials.FirstOrDefault(x => x.Material.Id == material.Id);

            return materialFoundFromMixer!;
        }

        public Amount WashingTime { get; set; } = new Amount(0, TimeUnits.Minute);
        public Amount PendingWashingTime => WashingTime - CurrentWashingTime;
        public Amount CurrentWashingTime { get; set; } = new Amount(0, TimeUnits.Minute);

        public bool IsMustWashTank()
        {
            if (CurrentOrder == null) return false;

            if (LastMaterial == null)
            {

                LastMaterial = CurrentOrder.Material;
                return false;
            }
            if (CurrentOrder.Material == null) return false;
            if (CurrentOrder.Material.Id == LastMaterial.Id) return false;

            var washDef = WashoutTimes
                .FirstOrDefault(x => x.ProductCategoryCurrent == CurrentOrder.Material?.ProductCategory &&
                                   x.ProductCategoryNext == LastMaterial.ProductCategory);


            if (washDef != null)
            {

                return true;
            }

            return false;
        }
        

        public Amount GetWashoutTime()
        {
            var result = new Amount(0, TimeUnits.Minute);
            if (CurrentOrder == null)
            {
                return result;
            }
            result = GetWashoutTime(LastMaterial, CurrentOrder.Material);
            LastMaterial = CurrentOrder.Material;

            return result;
        }
        public Amount GetWashoutTime(IMaterial current, IMaterial Next)
        {
            if (ManufactureAttached.Any(x => x.EquipmentMaterials.Any(x => x.Material.Id == Next.Id)))
            {
                if (current != null && Next != null)
                {
                    var washDef = WashoutTimes
                    .FirstOrDefault(x => x.ProductCategoryCurrent == current.ProductCategory &&
                                       x.ProductCategoryNext == Next.ProductCategory);
                    if (washDef != null)
                    {

                        return washDef.LineWashoutTime;
                    }
                }
            }



            return new Amount(0, TimeUnits.Second);
        }
        ManufaturingEquipment? IdentifyManufacturingEquipment(IVesselManufactureOrder order)
        {

            var wiptanks = order.Line.InletPumps
                .SelectMany(x => x.InletWipTanks).ToList();

            var manufactures = wiptanks.SelectMany(x => x.ManufactureAttached)
                .FirstOrDefault(x => x.EquipmentMaterials.Any(x => x.Material.Id == order.Material.Id));

            return manufactures;
        }
        public bool IsNextMaterialNeeded()
        {

            var currentOrderManufactureBy = IdentifyManufacturingEquipment(CurrentOrder);
            if (currentOrderManufactureBy is ProcessMixer)
            {
                if (NextOrder != null)
                {
                    return IsNextMaterialNeededByMixer(CurrentOrder, NextOrder);
                }
            }
            if (currentOrderManufactureBy is ProcessContinuousSystem)
            {
                return IsNextMaterialNeedBySKID(CurrentOrder);
            }



            return false;
        }
        public bool IsNextMaterialNeedBySKID(IVesselManufactureOrder _CurrentOrder)
        {
            var nextProductionOrder = _CurrentOrder.LineNextProductionOrder;
            if (nextProductionOrder != null)
            {
                var wiptanks = nextProductionOrder.Line.InletPumps
                    .SelectMany(x => x.InletWipTanks).ToList();

                var manufactures = wiptanks.SelectMany(x => x.ManufactureAttached)
                    .FirstOrDefault(x => x.EquipmentMaterials.Any(x => x.Material.Id == nextProductionOrder.Material.Id));
                if (manufactures != null && manufactures is ProcessMixer mixer)
                {
                    wiptanks = wiptanks.Where(x => x.ManufactureAttached.Any(x =>
                    x.EquipmentMaterials.Any(x => x.Material.Id == nextProductionOrder.Material.Id))).ToList();


                    var result = IsNextProductionBySKIDNeededToStart(_CurrentOrder, nextProductionOrder, wiptanks);
                    if (result)
                    {
                        return true;
                    }
                    return false;


                }

            }
            return false;
        }
        public bool IsNextProductionBySKIDNeededToStart(IVesselManufactureOrder _CurrentOrder, FromLineToWipProductionOrder nextproductionorder, List<ProcessWipTankForLine> wiptanks)
        {

            if (nextproductionorder == null)
                return false;

            var productionPlan = GetTimeToProduceProduct(nextproductionorder.Line, nextproductionorder.Material);
            if (productionPlan.SelectedMixer is null || productionPlan.SelectedRecipe is null)
                return false;

            if (_CurrentOrder.AverageOutletFlow.Value <= 0)
                return false;

            // Tank wash time (due to material change in the WIP tank)
            Amount tankWashTime = new Amount(0, TimeUnits.Minute);
            Amount currentLevelTanks = new Amount(0, MassUnits.KiloGram);
            foreach (var wipTank in wiptanks)
            {
                tankWashTime += GetWashoutTime(wipTank.LastMaterial, nextproductionorder.Material);
                currentLevelTanks = wipTank.CurrentLevel > currentLevelTanks ? wipTank.CurrentLevel : currentLevelTanks;
            }
            Amount changeovetime = new Amount(0, TimeUnits.Minute);
            if (_CurrentOrder.LineCurrentProductionOrder.Line.MustChangeFormat())
            {
                changeovetime = _CurrentOrder.LineCurrentProductionOrder.TimeToChangeSKU;
            }



            if (changeovetime > tankWashTime)
            {
                tankWashTime = changeovetime;
            }


            // Total time for mixer to complete next batch (includes mixer washout + production)
            var mixerTotalTime = productionPlan.BatchTime;

            if (_CurrentOrder.TimeToEmptyMassInProcess.Value > 0)
            {
                var timetoEmptyVessel = currentLevelTanks / _CurrentOrder.AverageOutletFlow;
                var timeUntilTankIsReady = _CurrentOrder.TimeToEmptyMassInProcess + tankWashTime + timetoEmptyVessel;

                // Start mixer IF it will finish BEFORE or EXACTLY when tank is ready
                if (timeUntilTankIsReady <= mixerTotalTime)
                {
                    if (!_CurrentOrder.IsSendToLineCurrentOrderIsProduced)
                    {
                        _CurrentOrder.SendToLineCurrentOrderIsProduced();
                    }

                    return true;
                }
                // Time from NOW until tank is ready (empty + washed)

            }
            return false;
        }
        public bool IsNextMaterialNeededByMixer(IVesselManufactureOrder _CurrentOrder, IVesselManufactureOrder _NextOrder)
        {
            if (_NextOrder == null || _CurrentOrder == null)
                return false;

            var productionPlan = GetTimeToProduceProduct(_NextOrder.Line, _NextOrder.Material);
            if (productionPlan.SelectedMixer is null || productionPlan.SelectedRecipe is null)
                return false;

            if (_CurrentOrder.AverageOutletFlow.Value <= 0)
                return false;

            // Tank wash time (due to material change in the WIP tank)
            Amount tankWashTime = new Amount(0, TimeUnits.Minute);
            if (_NextOrder.ManufactureOrdersFromMixers.Count == 0)
            {
                tankWashTime = GetWashoutTime(_CurrentOrder.Material, _NextOrder.Material);

            }
            Amount changeovetime = new Amount(0, TimeUnits.Minute);
            if (_CurrentOrder.LineCurrentProductionOrder.Line.MustChangeFormat())
            {
                changeovetime = _CurrentOrder.LineCurrentProductionOrder.TimeToChangeSKU;
            }

            if (changeovetime > tankWashTime)
            {
                tankWashTime = changeovetime;
            }
            // Total time for mixer to complete next batch (includes mixer washout + production)
            var mixerTotalTime = productionPlan.BatchTime;

            if (_NextOrder.TotalMassProducingInMixer == ZeroMass)
            {
                var timeUntilTankIsReady = _CurrentOrder.TimeToEmptyMassInProcess + tankWashTime;

                // Start mixer IF it will finish BEFORE or EXACTLY when tank is ready
                if (timeUntilTankIsReady <= mixerTotalTime)
                {
                    return TryToStartNewOrder(_NextOrder, productionPlan.SelectedMixer, productionPlan.SelectedRecipe);
                }
            }
            else
            {
                var futureLevel = _NextOrder.TotalMassProducingInMixer + productionPlan.SelectedRecipe.BatchSize;
                if (futureLevel <= Capacity)
                {
                    return TryToStartNewOrder(_NextOrder, productionPlan.SelectedMixer, productionPlan.SelectedRecipe);
                }
            }


            return false;
        }

        public bool IsWashingTimeCompleted()
        {
            if (PendingWashingTime <= ZeroTime)
            {
                return true;
            }
            return false;
        }

    }

}
