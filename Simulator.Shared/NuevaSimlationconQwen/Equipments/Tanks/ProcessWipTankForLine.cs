using Simulator.Shared.Enums.HCEnums.Enums;
using Simulator.Shared.NuevaSimlationconQwen.Equipments.Mixers;
using Simulator.Shared.NuevaSimlationconQwen.Equipments.Pumps;
using Simulator.Shared.NuevaSimlationconQwen.Equipments.Skids;
using Simulator.Shared.NuevaSimlationconQwen.ManufacturingOrders;
using Simulator.Shared.NuevaSimlationconQwen.Materials;
using Simulator.Shared.NuevaSimlationconQwen.Reports;
using System.Numerics;

namespace Simulator.Shared.NuevaSimlationconQwen.Equipments.Tanks
{
    public class ProcessWipTankForLine : ProcessBaseTank, ILiveReportable
    {
        public WIPManufacturingOrder CurrentOrder { get; set; } = null!;
        public TankCalculationType TankCalculationType { get; set; } = TankCalculationType.None;

        public List<ProcessMixer> InletMixers => InletEquipments.SelectMany(x => x.InletEquipments.OfType<ProcessMixer>().ToList()).ToList();
        public List<ProcessContinuousSystem> InletSKIDS => InletEquipments.OfType<ProcessContinuousSystem>().ToList().ToList();
        List<ManufaturingEquipment> ManufactureAttached => [.. InletSKIDS, .. InletMixers];
        //WIPTankInletStateForMixerProductionManager managerforManufactire = null!;
        public ProcessPump? WIPTankPump => OutletPumps.FirstOrDefault();

        public override void ValidateOutletInitialState(DateTime currentdate)
        {
            CurrentLevel = InitialLevel;
            OutletState = new ProcessWipTankOutletInitializeTankState(this);



        }

        public ReportColumn ReportColumn => ReportColumn.Column3_WipTanks;
        public ReportPriorityInColumn ReportPriority => ReportPriorityInColumn.Low;

        public List<LiveReportItem> GetLiveReportItems()
        {
            var items = new List<LiveReportItem>
        {
            new LiveReportItem
            {
                Label = "WIP Tank",
                Value = Name,
                Style = new ReportStyle()
            }
            ,
            new LiveReportItem
            {
                Label = "Capacity",
                Value = $"{Capacity.ToString()}",
                Style = new ReportStyle()
            },
            new LiveReportItem
            {
                Label = "Level",
                Value = $"{CurrentLevel.ToString()}",
                Style = GetLevelStyle()
            },
            new LiveReportItem
            {
                Label = "State",
                Value = OutletState?.StateLabel ?? "Unknown",
                Style = GetStateStyle()
            }
            
            
        };
            if(CurrentOrder != null )
            {
                items.Add(new LiveReportItem
                {
                    Label = "Producing to",
                    Value = CurrentOrder?.LineName ?? "Unknown",
                    Style = GetStateStyle()
                });
                items.Add(new LiveReportItem
                {
                    Label = "Material",
                    Value = CurrentOrder?.MaterialName ?? "Unknown",
                    Style = GetStateStyle()
                });
                items.Add(new LiveReportItem
                {
                    Label = "Mass delivered",
                    Value = CurrentOrder?.MassDelivered.ToString() ?? "Unknown",
                    Style = GetStateStyle()
                });
               
                items.Add(new LiveReportItem
                {
                    Label = "Average Outlet flow",
                    Value = CurrentOrder?.AverageOutletFlow.ToString() ?? "Unknown",
                    Style = new ReportStyle()
                });
                
                items.Add(new LiveReportItem
                {
                    Label = "Time to empty Mass in process",
                    Value = CurrentOrder?.TimeToEmptyMassInProcess.ToString() ?? "Unknown",
                    Style = new ReportStyle()
                });
                foreach (var row in CurrentOrder!.ManufactureOrdersFromMixers)
                {
                    items.Add(new LiveReportItem
                    {
                        Label = row.Mixer.Name,
                        Value = row.Mixer.InletState?.StateLabel ?? "Unknown",
                        Style = new ReportStyle()
                    });
                }
            }
            
                

            return items;
        }

        private ReportStyle GetLevelStyle()
        {
            if (CurrentLevel < LoLolevel) return new ReportStyle { Color = "Red", FontEmphasis = "Bold" };
            if (CurrentLevel < LoLevel) return new ReportStyle { Color = "Orange" };
            return new ReportStyle { Color = "Green" };
        }

        private ReportStyle GetStateStyle() => new ReportStyle();


        public Queue<TransferFromMixertoWIPOrder> TransfersOrdersFromMixers { get; set; } = new Queue<TransferFromMixertoWIPOrder>();
        private TransferFromMixertoWIPOrder? CurrentTransferFromMixer { get; set; }




        public void ReceiveTransferRequestFromMixer(TransferFromMixertoWIPOrder order)
        {
            TransfersOrdersFromMixers.Enqueue(order);
        }
        public void ReceiveManufactureOrderFromMixer(MixerManufactureOrder order)
        {
            if (CurrentOrder == null) return;
            CurrentOrder.AddMixerManufactureOrder(order);

        }
        public void ReleaseManufactureOrderFromMixer(MixerManufactureOrder order)
        {
            if (CurrentOrder == null) return;
            CurrentOrder.RemoveManufactureOrdersFromMixers(order);
            
        }


        //void CalculateEstimatedTime()
        //{
        //    //siempre esta calculando pero primero verifica si hay ordenes de produccion porque puede suceder que se acabaron desde la linea hay orden de producction activa
        //    if (CurrentOrder == null ||
        //        CurrentOrder.MassPending.Value <= 0 ||

        //        CurrentOrder.AverageOuletFlow.Value <= 0)
        //    {


        //        return;
        //    }

        //    //Si a este WIP ya habia asignado produccion en manufactura eso es un inventario en proceso

        //    //Calculamos la masa actual mas inventario en proceso (es como si el inventario en proceso estuviera en el tanque WIP, no esta pero va estar
        //    var totalAvailableMassKg = CurrentLevel + TotalMassProducingInMixer;
        //    //Obtenemos el flujo promedio de la linea
        //    var consumptionRateKgPerMin = CurrentOrder.AverageOuletFlow;

        //    if (consumptionRateKgPerMin.Value > 0)
        //    {
        //        //Calculamos el tiempo en que se acabara el tanque al flujo de produccion de la linea para usarlo ShouldStart
        //        EstimatedTimeUntilEmpty = totalAvailableMassKg / consumptionRateKgPerMin;

        //    }
        //    else
        //    {
        //        EstimatedTimeUntilEmpty = EstimatedTimeUntilEmptyInfinite;
        //    }
        //}

        //public void ShouldStartBatch1()
        //{
        //    //Verificamos si hay orden de manufactura desde la linea abierta y si hay masa pendiente por producir
        //    if (CurrentOrder == null || CurrentOrder.MassPending.Value <= 0) return;

        //    // ✅ Obtener todos los mixers conectados al WIP que producen el material requerido
        //    var allMixers = InletMixers
        //        .Where(m => m.Materials.OfType<RecipedMaterial>()
        //                    .Any(rm => rm.Id == CurrentOrder.Material.Id))
        //        .ToList();

        //    if (!allMixers.Any()) return;

        //    //Igualamos esta variable para propositos posteriores o por si la linea no tiene mixers preferidos usar esta variable de busqueda solamente
        //    var nonPreferredMixers = allMixers;

        //    // ✅ PASO 1: ¿Hay algún mixer preferido?
        //    if (CurrentOrder.Line.PreferredManufacturer.Any())
        //    {
        //        //Si hay mixer preferido hay alguno que esta libre si CurrentOrder==null
        //        var freePreferredMixer = CurrentOrder.Line.PreferredManufacturer.FirstOrDefault(m => m.CurrentManufactureOrder == null);
        //        if (freePreferredMixer != null)
        //        {
        //            //revisa si hay que iniciar batche con el mixer candidato y lo inicia si es false sigue buscando otros mixers
        //            if (ReviewCandidateToInitBatch(freePreferredMixer, CurrentOrder.Material))
        //            {
        //                return;
        //            }

        //        }
        //        //Busca ahora en los no preferidos quien esta libre que pueda producir este material
        //        nonPreferredMixers = allMixers.Except(CurrentOrder.Line.PreferredManufacturer).ToList();
        //    }

        //    var freeNonPreferredMixer = nonPreferredMixers.FirstOrDefault(m => m.CurrentManufactureOrder == null);

        //    if (freeNonPreferredMixer != null)
        //    {
        //        //Si encontro candidado ahora revisa si hay que iniciar batche con el mixer candidato y lo inicia si es false sigue buscando otros mixers
        //        if (ReviewCandidateToInitBatch(freeNonPreferredMixer, CurrentOrder.Material))
        //        {
        //            return;
        //        }

        //    }
        //    //como llego hasta aqui se reviso que todos los mixers efectivamenete estan produciendo por CurrentOrder!=null
        //    //entonces se busca por la menor cola y luego por menor tiempo de batch

        //    // ✅ Función auxiliar para obtener tiempo pendiente de un mixer
        //    Func<ProcessMixer, Amount> GetPendingBatchTime = mixer =>
        //        mixer.CurrentManufactureOrder?.TheoricalPendingBatchTime ?? Amount.Zero(TimeUnits.Minute);


        //    // ✅ PASO 4: Ningún mixer está libre → buscar el mejor entre todos (preferidos + no preferidos)
        //    var allAvailableMixers = allMixers.Concat(nonPreferredMixers).ToList();
        //    var bestOverallMixer = allAvailableMixers
        //        .OrderBy(x => x.ManufacturingOrders.Count)
        //        .ThenBy(GetPendingBatchTime)
        //        .FirstOrDefault();

        //    // ✅ PASO 5: ¿El mejor mixer disponible (con menor tiempo pendiente) puede completar antes de que se vacíe el tanque?
        //    if (bestOverallMixer != null)
        //    {
        //        if (ReviewCandidateToInitBatch(bestOverallMixer, CurrentOrder.Material))
        //        {
        //            return;
        //        }

        //    }
        //    //si aun no se requiere o no se necesita mixer en la proxima vuelta sigue buscando
        //}

        //bool ReviewCandidateToInitBatch(ProcessMixer candidate, IMaterial materialToSearch)
        //{

        //    if (candidate != null)
        //    {
        //        //Como los mixers pueden tener diferentes tamaños producir un batche de un material en particular puede demorarse mas o menos tiempo dependiendo dle mixer
        //        //Previamente en la simulacion ya se calculo por mixer cuanto se demora el batche
        //        //Con esta busqueda en particular en el mixer conocemos las caracteristica especiales de ese material en el mixer
        //        var material = candidate.Materials.OfType<RecipedMaterial>().FirstOrDefault(x => x.Id == materialToSearch.Id);
        //        //confirmamos que efectivamente ese material lo puede producir ese mixer si no lo puede producir sigue buscando en los otros mixers mas adelante
        //        if (material != null)
        //        {
        //            //Esta funcion nos devuelve la suma del tiempo de lavado(si habria) tiempo de batche + tiempo de trasnferencia
        //            Amount washoutTime = new Amount(0, TimeUnits.Minute);
        //            if (candidate.CurrentMaterial != null)
        //            {
        //                var washoutDef = candidate.WashoutTimes
        //                    .FirstOrDefault(x => x.ProductCategoryCurrent == candidate.CurrentMaterial.ProductCategory &&
        //                                       x.ProductCategoryNext == material.ProductCategory);

        //                if (washoutDef != null)
        //                {
        //                    washoutTime = washoutDef.MixerWashoutTime;
        //                }
        //            }

        //            var batchTime = material.BatchCycleTime;
        //            var transferTime = material.TransferTime;
        //            var totalBatchTime = washoutTime + batchTime + transferTime;

        //            // Margen de seguridad del 10%
        //            totalBatchTime *= 1.1;

        //            //Comparamos con el tiempo que falta para terminar el batch si es menor o igual hay que empezar ya porque en el futuro el tanque WIP 
        //            //Si iniciamos ahora el batch cuando termine posiblemente el tanque WIP se quede sin nivel si no iniciamos ahora
        //            //Esto tambien se puede hacer por niveles
        //            if (CurrentOrder?.MassPending > ZeroMass && CurrentOrder?.AverageOuletFlow.Value > 0)
        //            {
        //                var lineflow = CurrentOrder?.AverageOuletFlow;
        //                var batchtime = totalBatchTime;
        //                var massdeliverded = batchtime * lineflow;
        //                var batchsize = material.BatchSize;
        //                var massproducing = TotalMassStoragedOrProducing;
        //                var finalLevel = massproducing + batchsize - massdeliverded;
        //                if (finalLevel <= HiLevel)
        //                {
        //                    if (ManufactureOrdersFromMixers.Count > 0)
        //                    {
        //                        var LowerBatchCycleTime = ManufactureOrdersFromMixers.MinBy(x => x.CurrentBatchTime.GetValue(TimeUnits.Minute));
        //                        if (LowerBatchCycleTime != null)
        //                        {
        //                            if (LowerBatchCycleTime.CurrentBatchTime < transferTime)
        //                            {
        //                                //Nos aseguramos que inicie despues del tiempo de transferencia para que no se cruce con el batche que esta produciendo para este WIP
        //                                return false;
        //                            }
        //                        }
        //                    }
        //                    //Creamos orden de manufactura en el WIP Se envia informacion al mixer candidato que necesitamos material en tanque WIP
        //                    var newOrder = new FromWIPToMixerManufactureOrder(material, this);
        //                    candidate.ReceiveManufactureOrderFromWIP(newOrder);

        //                    return true;
        //                    ////Quiere decir que si inicia produccion ahora cuando termine el batch si cabe en el tanque
        //                    //if (totalBatchTime <= EstimatedTimeUntilEmpty)
        //                    //{

        //                    //}
        //                }
        //                else if (batchsize > Capacity)
        //                {
        //                    //Casso especial cuando el tanque es muy pequeño, que inicie batche aunque no quepa
        //                    // se detendra la transferencia en algun nivel
        //                    var newOrder = new FromWIPToMixerManufactureOrder(material, this);
        //                    candidate.ReceiveManufactureOrderFromWIP(newOrder);

        //                    return true;
        //                }
        //            }
        //            else
        //            {
        //                StartTransferFromFirstQueue();
        //            }




        //        }
        //    }
        //    return false;
        //}

     


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
            if (CurrentTransferFromMixer != null)
            {
                var masstoreceive = CurrentTransferFromMixer.TransferFlow * OneSecond;
                var massreceived = new Amount(Math.Min(CurrentTransferFromMixer.PendingToReceive.GetValue(MassUnits.KiloGram), masstoreceive.GetValue(MassUnits.KiloGram)), MassUnits.KiloGram);
                CurrentLevel += massreceived;
                CurrentTransferFromMixer.MassReceived += massreceived;
                CurrentTransferFromMixer.SourceMixer.ReceiveReportOfMassDelivered(massreceived);
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
                if (CurrentLevel < CurrentTransferFromMixer.PendingToReceive)
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
                CurrentOrder.MassDelivered += mass;
                CurrentOrder.AddRunTime();

            }
        }
        
        public void ReceiveFromLineProductionOrder(FromLineToWipProductionOrder order)
        {
            if (ManufactureAttached.Any(y => y.EquipmentMaterials.Any(x => x.Material.Id == order.Material.Id)))
            {
                CurrentOrder = new WIPManufacturingOrder(this, order.Line, order.Material, order.TotalQuantityToProduce);
                CurrentOrder.MassToDeliver -= CurrentLevel;

                OutletManufactureOrderIsReceived = true;
                order.Line.ReceiveWipCanHandleMaterial(this);
            }

        }
        bool OutletManufactureOrderIsReceived = false;
        bool InletManufactureOrderIsReceived = false;
        public bool IsNewOrderReceived()
        {
            if (OutletManufactureOrderIsReceived)
            {
                OutletManufactureOrderIsReceived = false;
                return true;
            }

            return false;
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

        }
        public bool IsInletStateSelected()
        {
            if (CurrentOrder != null)
            {
                var manufactures = ManufactureAttached.FirstOrDefault(x => x.EquipmentMaterials.Any(x => x.Material.Id == CurrentOrder.Material.Id));
                if (manufactures != null && manufactures is ProcessContinuousSystem skid)
                {
                    InletSKIDS.ForEach(x => x.ReceiveManufacturingOrderFromWIP(CurrentOrder, this));
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

        public bool IsMassDeliveredCompleted()
        {
            if (CurrentOrder != null)
            {
                if (CurrentOrder.MassPendingToProduce <= ZeroMass)
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
            CurrentOrder = null!;
            return true;
        }


        // Método principal (usado por la máquina de estados)
        public bool IsMaterialNeeded()
        {
            if (CurrentOrder is null) return false;




            var plan = GetTimeToProduceProduct(CurrentOrder.Material);
            if (plan.SelectedMixer is null || plan.SelectedRecipe is null)
                return false;
            if (CurrentOrder.TotalMassStoragedOrProducing.Value == 0)
            {
                return TryToStartNewOrder(plan.SelectedMixer, plan.SelectedRecipe); ;
            }
            if (CurrentOrder.TimeToEmptyMassInProcess.Value == 0)
            {
                var futurelevel = CurrentOrder.TotalMassStoragedOrProducing + plan.SelectedRecipe.BatchSize;

                if (futurelevel <= Capacity)
                {
                    return TryToStartNewOrder(plan.SelectedMixer, plan.SelectedRecipe);
                }
            }

            if (CurrentOrder.TimeToEmptyMassInProcess.Value > 0 && CurrentOrder.TimeToEmptyMassInProcess <= plan.TotalBatchTime * 1.2)
            {
                return TryToStartNewOrder(plan.SelectedMixer, plan.SelectedRecipe);

            }
            return false;

        }
        public bool TryToStartNewOrder(ProcessMixer mixer, IEquipmentMaterial recipe)
        {
            if (CurrentOrder is null) return false;
            var lastMixer = CurrentOrder.LastInOrder;
            if (lastMixer is null)
            {
                StartNewOrder(mixer, recipe);
                return true;
            }
            if (lastMixer.Mixer.CurrentManufactureOrder.CurrentBatchTime > recipe.TransferTime)
            {
                StartNewOrder(mixer, recipe);
                return true;
            }
            return false;
        }
        public void StartNewOrder(ProcessMixer mixer, IEquipmentMaterial recipe)
        {
            var material = (IRecipedMaterial)recipe.Material;
            if (material == null) return;
            var newOrder = new FromWIPToMixerManufactureOrder(material, this);
            mixer.ReceiveManufactureOrderFromWIP(newOrder);
        }

        (Amount TotalBatchTime, ProcessMixer SelectedMixer, IEquipmentMaterial SelectedRecipe) GetTimeToProduceProduct(IMaterial material)
        {
            var selectedMixerMaterial = SelectCandidateMixers(material);
            if (selectedMixerMaterial.MixerCandidate is null)
                return (new Amount(0, TimeUnits.Minute), null!, null!);



            var washoutTime = GetWashoutTime(selectedMixerMaterial.MixerCandidate, material);
            var batchTime = selectedMixerMaterial.Recipe.BatchCycleTime;
            var transferTime = selectedMixerMaterial.Recipe.TransferTime;
            var totalTime = washoutTime + transferTime + batchTime;

            return (totalTime, selectedMixerMaterial.MixerCandidate, selectedMixerMaterial.Recipe);
        }
        (ProcessMixer MixerCandidate, IEquipmentMaterial Recipe) SelectCandidateMixers(IMaterial material)
        {
            if (CurrentOrder is null) return (null!, null!);
            IEquipmentMaterial materialFromMixer = null!;
            // 1. Preferidos libres
            if (CurrentOrder.Line.PreferredManufacturer.Any())
            {
                var mixer = CurrentOrder.Line.PreferredManufacturer
                    .FirstOrDefault(x => x.EquipmentMaterials.Any(m => m.Material.Id == material.Id) && x.CurrentManufactureOrder == null);
                if (mixer != null)
                {
                    materialFromMixer = SelectMaterialFromMixer(mixer, material);
                    return (mixer, materialFromMixer);
                }
            }

            // 2. Todos los mezcladores que producen el material
            var allMixersThatProduceMaterial = InletMixers
                .Where(x => x.EquipmentMaterials.Any(m => m.Material.Id == material.Id))
                .ToList();

            if (allMixersThatProduceMaterial.Count == 0) return (null!, null!);

            // 3. Si alguno está libre → devolver el primero
            var freeMixer = allMixersThatProduceMaterial.FirstOrDefault(x => x.CurrentManufactureOrder == null);
            if (freeMixer != null)
            {
                materialFromMixer = SelectMaterialFromMixer(freeMixer, material);
                return (freeMixer, materialFromMixer);
            }

            // 4. Todos ocupados → buscar el primero que pueda encolar (batchTime > transferTime)
            var orderedMixers = allMixersThatProduceMaterial
                .OrderBy(x => x.CurrentManufactureOrder.CurrentBatchTime.GetValue(TimeUnits.Minute))
                .ToList();

            foreach (var candidate in orderedMixers)
            {
                materialFromMixer = SelectMaterialFromMixer(candidate, material);
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
                materialFromMixer = SelectMaterialFromMixer(FirstMixer!, material);
                // 5. Si ninguno puede encolar → devolver el que termine primero
                return (FirstMixer, materialFromMixer);
            }
            materialFromMixer = SelectMaterialFromMixer(FirstMixer!, material);
            // 5. Si ninguno puede encolar → devolver el que termine primero
            return (null!, null!);
        }

        Amount GetWashoutTime(ProcessMixer mixer, IMaterial material)
        {
            Amount washoutTime = new Amount(0, TimeUnits.Minute);
            if (mixer.CurrentMaterial != null)
            {
                var washoutDef = mixer.WashoutTimes
                                .FirstOrDefault(x => x.ProductCategoryCurrent == mixer.CurrentMaterial.ProductCategory &&
                                                   x.ProductCategoryNext == material.ProductCategory);

                if (washoutDef != null)
                {
                    washoutTime = washoutDef.MixerWashoutTime;
                }
            }
            return washoutTime;
        }
        IEquipmentMaterial SelectMaterialFromMixer(ProcessMixer mixer, IMaterial material)
        {

            var materialFoundFromMixer = mixer.EquipmentMaterials.FirstOrDefault(x => x.Material.Id == material.Id);

            return materialFoundFromMixer!;
        }






    }

}
