using Simulator.Shared.Enums.HCEnums.Enums;
using Simulator.Shared.NuevaSimlationconQwen.Equipments.Lines;
using Simulator.Shared.NuevaSimlationconQwen.Equipments.Mixers;
using Simulator.Shared.NuevaSimlationconQwen.Equipments.Pumps;
using Simulator.Shared.NuevaSimlationconQwen.Equipments.Skids;
using Simulator.Shared.NuevaSimlationconQwen.Equipments.Tanks.States;
using Simulator.Shared.NuevaSimlationconQwen.ManufacturingOrders;
using Simulator.Shared.NuevaSimlationconQwen.Materials;
using Simulator.Shared.NuevaSimlationconQwen.Reports;

namespace Simulator.Shared.NuevaSimlationconQwen.Equipments.Tanks
{
    public class ProcessWipTankForLine : ProcessBaseTank, ILiveReportable, IRequestTansferTank
    {

        private IWIPManufactureOrder _NextOrder { get; set; } = null!;

        public IWIPManufactureOrder CurrentOrder => _CurrentOrder;
        private IWIPManufactureOrder _CurrentOrder { get; set; } = null!;

        private Queue<TransferFromMixertoWIPOrder> TransfersOrdersFromMixers { get; set; } = new Queue<TransferFromMixertoWIPOrder>();
        private TransferFromMixertoWIPOrder? CurrentTransferFromMixer { get; set; }
        public List<ProcessMixer> InletMixers => InletEquipments.SelectMany(x => x.InletEquipments.OfType<ProcessMixer>().ToList()).ToList();
        public List<ProcessContinuousSystem> InletSKIDS => InletEquipments.OfType<ProcessContinuousSystem>().ToList().ToList();
        List<ManufaturingEquipment> ManufactureAttached => [.. InletSKIDS, .. InletMixers];

        public ProcessPump? WIPTankPump => OutletPumps.FirstOrDefault();

        public override void ValidateOutletInitialState(DateTime currentdate)
        {
            CurrentLevel = InitialLevel;

            OutletState = new ProcessWipTankOutletInitializeTankState(this);


        }

        public WipTankForLineReport CurrentReport => CurrentOrder?.Report ?? new WipTankForLineReport();
        public void ReceiveInitFromLineProductionOrder(FromLineToWipProductionOrder order)
        {
            var manufactures = ManufactureAttached.FirstOrDefault(x => x.EquipmentMaterials.Any(x => x.Material.Id == order.Material.Id));
            if (manufactures != null)
            {
                OutletState = new ProcessWipTankOutletReviewInitInletStateTankState(this);
                if (manufactures is ProcessContinuousSystem skid)
                {
                    if (_CurrentOrder == null)
                    {
                        _CurrentOrder = new WIPInletSKIDManufacturingOrder(this, order);

                        order.ReceiveWipCanHandleMaterial(this);
                        _CurrentOrder.AddMassProduced(CurrentLevel);
                        


                    }
                    else
                    {
                        _NextOrder = new WIPInletSKIDManufacturingOrder(this, order);
                        order.ReceiveWipCanHandleMaterial(this);
                    }
                }
                else
                {
                    if (_CurrentOrder == null)
                    {
                        _CurrentOrder = new WIPInletMixerManufacturingOrder(this, order);

                        order.ReceiveWipCanHandleMaterial(this);
                        _CurrentOrder.AddMassProduced(CurrentLevel);
                        
                    }
                    else
                    {
                        _NextOrder = new WIPInletMixerManufacturingOrder(this, order);
                        order.ReceiveWipCanHandleMaterial(this);
                    }
                }

            }


        }
        //outlet state Methods
        public bool IsMustWashTank()
        {
            if (_CurrentOrder == null) return false;

            if (LastMaterial == null)
            {

                LastMaterial = _CurrentOrder.Material;
                return false;
            }
            if (_CurrentOrder.Material == null) return false;
            if (_CurrentOrder.Material.Id == LastMaterial.Id) return false;

            var washDef = WashoutTimes
                .FirstOrDefault(x => x.ProductCategoryCurrent == _CurrentOrder.Material?.ProductCategory &&
                                   x.ProductCategoryNext == LastMaterial.ProductCategory);


            if (washDef != null)
            {

                return true;
            }

            return false;
        }
        public void SelectInletStateBasedOnManufacturingEquipment()
        {
            if (_CurrentOrder == null) return;

            var manufactures = ManufactureAttached.FirstOrDefault(x =>
                x.EquipmentMaterials.Any(m => m.Material.Id == _CurrentOrder.Material.Id));

            if (manufactures is ProcessContinuousSystem)
            {
                InletSKIDS.ForEach(x => x.ReceiveManufactureOrderFromWIP(_CurrentOrder));
                InletState = new TankInletManufacturingOrderReceivedSKIDState(this);
            }
            else
            {
                InletState = new TankInletWaitingForInletMixerState(this);
            }
        }
        public Amount GetWashoutTime()
        {
            var result = new Amount(0, TimeUnits.Minute);
            if (_CurrentOrder == null)
            {
                return result;
            }
            result = GetWashoutTime(LastMaterial, _CurrentOrder.Material);
            LastMaterial = _CurrentOrder.Material;

            return result;
        }
        private Amount GetWashoutTime(IMaterial current, IMaterial Next)
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
        public bool IsCurrentOrderRealesed()
        {
            if (_NextOrder != null)
            {
                _CurrentOrder = _NextOrder;
                _NextOrder = null!;

                return true;
            }
            _CurrentOrder = null!;

            return true;
        }
        public bool IsCurrentOrderMassDeliveredCompleted()
        {
            if (_CurrentOrder != null)
            {
                if (_CurrentOrder.MassPendingToDeliver <= ZeroMass)
                {

                    return true;
                }
            }
            return false;
        }
        ManufaturingEquipment? IdentifyManufacturingEquipment(IWIPManufactureOrder order)
        {

            var wiptanks = order.Line.InletPumps
                .SelectMany(x => x.InletWipTanks).ToList();

            var manufactures = wiptanks.SelectMany(x => x.ManufactureAttached)
                .FirstOrDefault(x => x.EquipmentMaterials.Any(x => x.Material.Id == order.Material.Id));

            return manufactures;
        }
        public bool IsNextOrderMaterialNeeded()
        {

            var currentOrderManufactureBy = IdentifyManufacturingEquipment(_CurrentOrder);
            if (currentOrderManufactureBy is ProcessMixer)
            {
                if (_NextOrder != null)
                {
                    return IsNextMaterialNeededByMixer(_CurrentOrder, _NextOrder);
                }
            }
            if (currentOrderManufactureBy is ProcessContinuousSystem)
            {
                return IsNextMaterialNeedBySKID(_CurrentOrder);
            }



            return false;
        }
        public override void CalculateOutletLevel()
        {
            base.CalculateOutletLevel();

            if (_CurrentOrder != null)
            {
                _CurrentOrder.AddMassDelivered(MassDeliveredBySecond);
                _CurrentOrder.AddRunTime();

            }
        }
        public override void CalculateRunTime()
        {
            base.CalculateRunTime();
            if (_CurrentOrder != null)
            {

                _CurrentOrder.AddRunTime();

            }
        }
        //inlet state Methods inletMixers
        public bool IsCurrentOrderMassProducedCompleted()
        {
            if (_CurrentOrder != null)
            {
                if (TransfersOrdersFromMixers.Count > 0 || CurrentTransferFromMixer != null || _CurrentOrder.ManufactureOrdersFromMixers.Count > 0)
                {
                    return false;
                }
                if (_CurrentOrder.IsPendingToProduceCompleted())
                {
                    return true;
                }

            }


            return false;
        }
        public bool ReviewIfTransferCanInit()
        {
            if (TransfersOrdersFromMixers.Count == 0) return false;

            CurrentTransferFromMixer = TransfersOrdersFromMixers.Dequeue();
            CurrentTransferFromMixer.SourceMixer.ReceiveTransferOrderFromWIPToInit(CurrentTransferFromMixer);
            return true;
        }
        public bool IsHighLevelDuringMixerTransfer()
        {
            if (CurrentTransferFromMixer != null && base.IsTankHigherThenHiLevel())
            {
                StartCriticalReport(
                        this,
                        "Starved by High Level",
                        $"Tank {Name} is full."
                    );
                return true;
            }
            return false;
        }
        public bool IsTransferFinalized()
        {
            if (CurrentTransferFromMixer != null)
            {
                if (CurrentTransferFromMixer.IsTransferComplete)
                {
                    CurrentTransferFromMixer.SendMixerTransferIsFinished();
                    CurrentTransferFromMixer = null;
                    return true;
                }
            }
            return false;
        }
        public bool IfTransferStarvedByHighLevelCanResume()
        {
            if (CurrentTransferFromMixer != null)
            {
                if (CurrentTransferFromMixer.CanTransferWithoutOverflowingDestination())
                {
                    EndCriticalReport();
                    return true;
                }
            }
            return false;
        }
        public void ReceiveTransferRequestFromMixer(TransferFromMixertoWIPOrder order)
        {
            TransfersOrdersFromMixers.Enqueue(order);
        }
        public void SetCurrentMassTransfered()
        {

            if (CurrentTransferFromMixer != null)
            {
                CurrentTransferFromMixer.SetCurrentMassTransfered();
            }
        }

        //inlet state Methods SKIDS
        public bool IsSKIDMustStop()
        {
            if (base.IsTankHigherThenHiLevel())
            {
                StopSkid();
                return true;
            }
            return false;
        }
        public bool IsSKIDCanStart()
        {
            if (_CurrentOrder == null) return false;

            if (IsTankInLoLevel())
            {
                StartSkid();
                return true;
            }
            return false;
        }
        void StartSkid()
        {
            InletSKIDS.ForEach(x => x.Produce());

        }
        void StopSkid()
        {
            InletSKIDS.ForEach(x => x.Stop());

        }
        public bool IsSKIDWIPProducedCompleted()
        {

            if (_CurrentOrder.IsPendingToProduceCompleted())
            {

                InletSKIDS.ForEach(x => x.ReceiveTotalStop());
                return true;
            }



            return false;
        }
        public void ReceiveProductFromSKID(Amount flow)
        {
            var mass = flow * OneSecond;
            CurrentLevel += mass;
            if (_CurrentOrder != null)
            {
                _CurrentOrder.AddMassProduced(mass);
            }
        }


        public bool IsMaterialNeeded()
        {
            if (_CurrentOrder is null) return false;




            var plan = GetTimeToProduceProduct(_CurrentOrder.Line, _CurrentOrder.Material);
            if (plan.SelectedMixer is null || plan.SelectedRecipe is null)
                return false;

            if (_CurrentOrder.TotalMassStoragedOrProducing.Value == 0)
            {
                return TryToStartNewOrder(_CurrentOrder, plan.SelectedMixer, plan.SelectedRecipe); ;
            }
            var futurelevel = _CurrentOrder.TotalMassStoragedOrProducing + plan.SelectedRecipe.BatchSize;
            if (_CurrentOrder.TimeToEmptyMassInProcess.Value > 0)
            {
                var massoutletduringBatch = plan.TotalBatchTime * 0.85 * _CurrentOrder.AverageOutletFlow;
                futurelevel -= massoutletduringBatch;

            }
            if (CurrentTransferFromMixer != null)
            {
                futurelevel += CurrentTransferFromMixer.PendingToReceive;
            }
            if (futurelevel <= Capacity)
            {
                return TryToStartNewOrder(_CurrentOrder, plan.SelectedMixer, plan.SelectedRecipe);
            }

            return false;

        }
        public bool TryToStartNewOrder(IWIPManufactureOrder order, ManufaturingEquipment mixer, IEquipmentMaterial recipe)
        {
            if (order is null) return false;
            var lastMixer = order.LastInOrder;
            if (lastMixer is null)
            {
                StartNewOrder(order, mixer);
                return true;
            }
            if (lastMixer.ManufaturingEquipment.CurrentManufactureOrder.CurrentBatchTime > recipe.TransferTime)
            {
                StartNewOrder(order, mixer);
                return true;
            }
            return false;
        }
        public void StartNewOrder(IWIPManufactureOrder order, ManufaturingEquipment mixer)
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
            var mixers = InletMixers;
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

            //foreach (var candidate in orderedMixers)
            //{
            //    materialFromMixer = SelectMaterialFromMixer(candidate, Material);
            //    // Asegúrate de que materialFromMixer no sea null
            //    if (materialFromMixer != null &&
            //        candidate.CurrentManufactureOrder.CurrentBatchTime > materialFromMixer.TransferTime)
            //    {

            //        return (candidate, materialFromMixer); // ¡Encontramos uno que puede encolar!
            //    }
            //}
            var FirstMixer = orderedMixers.FirstOrDefault();
            if (FirstMixer != null)
            {
                materialFromMixer = SelectMaterialFromMixer(FirstMixer!, Material);
                // 5. Si ninguno puede encolar → devolver el que termine primero
                return (FirstMixer, materialFromMixer);
            }

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








        public bool IsNextMaterialNeedBySKID(IWIPManufactureOrder _CurrentOrder)
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
        public bool IsNextProductionBySKIDNeededToStart(IWIPManufactureOrder _CurrentOrder, FromLineToWipProductionOrder nextproductionorder, List<ProcessWipTankForLine> wiptanks)
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
                    //if (!_CurrentOrder.IsSendToLineCurrentOrderIsProduced)
                    //{
                    //    _CurrentOrder.SendToLineCurrentOrderIsProduced();
                    //}

                    return true;
                }
                // Time from NOW until tank is ready (empty + washed)

            }
            return false;
        }
        public bool IsNextMaterialNeededByMixer(IWIPManufactureOrder _CurrentOrder, IWIPManufactureOrder _NextOrder)
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



    }

}
