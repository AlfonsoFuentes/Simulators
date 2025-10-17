using Simulator.Shared.NuevaSimlationconQwen.Equipments.Mixers;
using Simulator.Shared.NuevaSimlationconQwen.Equipments.Pumps;
using Simulator.Shared.NuevaSimlationconQwen.Equipments.Tanks.States;
using Simulator.Shared.NuevaSimlationconQwen.ManufacturingOrders;
using Simulator.Shared.NuevaSimlationconQwen.Materials;
using Simulator.Shared.NuevaSimlationconQwen.Reports;

namespace Simulator.Shared.NuevaSimlationconQwen.Equipments.Tanks
{
    public class ProcessRecipedTank : ProcessBaseTankForRawMaterial, IRequestTansferTank
    {
      
        public ITankManufactureOrder CurrentTankManufactureOrder { get; set; } = null!;
        public Queue<TransferFromMixertoWIPOrder> TransfersOrdersFromMixers { get; set; } = new Queue<TransferFromMixertoWIPOrder>();
        private TransferFromMixertoWIPOrder? CurrentTransferFromMixer { get; set; } = null!;
        public List<ProcessPump> InletPumps => InletEquipments.OfType<ProcessPump>().ToList();
        public List<ProcessMixer> InletMixers => InletPumps.SelectMany(x => x.InletManufacturingEquipments.OfType<ProcessMixer>()).ToList();



        public override void ValidateOutletInitialState(DateTime currentdate)
        {
            CurrentLevel = InitialLevel;
            OutletState = new TankOutletInitializeTankState(this);
            InletState = new ProcessRecipedTankInletWaitingForInletMixerState(this);
            CurrentTankManufactureOrder = new RecipedTankManufactureOrder(this, Material!);

        }
        public void ReceiveTransferRequestFromMixer(TransferFromMixertoWIPOrder order)
        {
            TransfersOrdersFromMixers.Enqueue(order);
        }
        public bool IsMaterialNeeded()
        {
            var plan = GetTimeToProduceProduct(Material);
            if (plan.SelectedMixer is null || plan.SelectedRecipe is null)
                return false;
            if (CurrentTransferFromMixer == null && CurrentTankManufactureOrder.TotalMassProducingInMixer < LoLolevel)
            {
                return TryToStartNewOrder(plan.SelectedMixer, plan.SelectedRecipe);
            }

            if (CurrentTankManufactureOrder.TimeToEmptyMassInProcess.Value == 0)
                return false;

            if (CurrentTankManufactureOrder.TimeToEmptyMassInProcess > plan.BatchTime * 1.15)
                return false;

            var futurelevel = CurrentTankManufactureOrder.TotalMassStoragedOrProducing + plan.SelectedRecipe.BatchSize;
            var massoutletduringBatch = plan.TotalBatchTime * 0.85 * AverageFlowRate;
            futurelevel -= massoutletduringBatch;
            if (CurrentTransferFromMixer != null)
            {
                futurelevel += CurrentTransferFromMixer.PendingToReceive;
            }
            if (futurelevel <= Capacity)
            {
                return TryToStartNewOrder(plan.SelectedMixer, plan.SelectedRecipe);
            }

            return false;

        }
        public bool TryToStartNewOrder(ManufaturingEquipment mixer, IEquipmentMaterial recipe)
        {
            var lastMixer = CurrentTankManufactureOrder.LastInOrder;
            if (lastMixer == null)
            {
                StartNewOrder(mixer);
                return true;
            }

            if (lastMixer.ManufaturingEquipment.CurrentManufactureOrder.CurrentBatchTime > recipe.TransferTime)
            {
                StartNewOrder(mixer);
                return true;
            }

            return false;


        }
        public void StartNewOrder(ManufaturingEquipment mixer)
        {

            mixer.ReceiveManufactureOrderFromWIP(CurrentTankManufactureOrder);
        }
        public bool ReviewIfTransferCanInit()
        {
            if (TransfersOrdersFromMixers.Count == 0) return false;

            CurrentTransferFromMixer = TransfersOrdersFromMixers.Dequeue();
            CurrentTransferFromMixer.SourceMixer.ReceiveTransferOrderFromWIPToInit(CurrentTransferFromMixer);
            return true;


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
        public void ReportReinitTransferToMixer()
        {
            if (CurrentTransferFromMixer != null)
            {
                //CurrentTransferFromMixer.SourceMixer.ReceiveTransferReInitStarvedFromWIP();
            }
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
        (Amount TotalBatchTime, Amount BatchTime, ManufaturingEquipment SelectedMixer, IEquipmentMaterial SelectedRecipe)
          GetTimeToProduceProduct(IMaterial? Material)
        {
            if (Material == null) return new(null!, null!, null!, null!);
            var selectedMixerMaterial = SelectCandidateMixers(Material);
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
        (ManufaturingEquipment MixerCandidate, IEquipmentMaterial Recipe) SelectCandidateMixers(IMaterial Material)
        {
            if (Material is null) return (null!, null!);
            IEquipmentMaterial materialFromMixer = null!;
            // 1. Preferidos libres

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
        IEquipmentMaterial SelectMaterialFromMixer(ManufaturingEquipment mixer, IMaterial material)
        {

            var materialFoundFromMixer = mixer.EquipmentMaterials.FirstOrDefault(x => x.Material.Id == material.Id);

            return materialFoundFromMixer!;
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
    }


}
