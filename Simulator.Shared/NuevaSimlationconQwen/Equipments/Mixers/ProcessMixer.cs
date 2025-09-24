using Simulator.Shared.Enums.HCEnums.Enums;
using Simulator.Shared.NuevaSimlationconQwen.Equipments.Lines;
using Simulator.Shared.NuevaSimlationconQwen.Equipments.Operators;
using Simulator.Shared.NuevaSimlationconQwen.Equipments.Pumps;
using Simulator.Shared.NuevaSimlationconQwen.ManufacturingOrders;
using Simulator.Shared.NuevaSimlationconQwen.Materials;
using Simulator.Shared.NuevaSimlationconQwen.Reports;

namespace Simulator.Shared.NuevaSimlationconQwen.Equipments.Mixers
{
    public class ProcessMixer : ManufaturingEquipment, ISetMaterialsAtOutlet, ILiveReportable
    {
        public List<ProcessLine> PreferredLines { get; set; } = new List<ProcessLine>();  
        public Amount CurrentLevel { get; set; } = new Amount(0, MassUnits.KiloGram);

        public List<ProcessPump> InletPumps => InletEquipments.OfType<ProcessPump>().ToList();
        public List<ProcessOperator> InletOperators => InletEquipments.OfType<ProcessOperator>().ToList();

        public List<ProcessPump> OutletPumps => OutletEquipments.OfType<ProcessPump>().ToList();
        public ProcessPump? OutletPump => OutletPumps.FirstOrDefault();
        public virtual void SetMaterialsAtOutlet(IMaterial material)
        {
            foreach (var outlet in OutletPumps)
            {
                outlet.AddMaterial(material);

            }
        }
        public override void OnInit(DateTime currentdate)
        {
            base.OnInit(currentdate);
            //OutletState = new MixerOuletInitialState(this);
            InletState = new MixerInletWaitingForManufactureOrderState(this);
            OutletState = new MixerOuletInitialState(this);
        }
        public ReportColumn ReportColumn => ReportColumn.Column2_SkidsAndMixers;
        public ReportPriorityInColumn ReportPriority => ReportPriorityInColumn.Low;
        private ReportStyle GetStateStyle() => new ReportStyle();
        public List<LiveReportItem> GetLiveReportItems()
        {
            var items = new List<LiveReportItem>
        {
            new LiveReportItem
            {
                Label = "Mixer",
                Value = Name,
                Style = new ReportStyle()
            },

        };
            items.Add(new LiveReportItem
            {
                Label = "Current Level",
                Value = $"{CurrentLevel.ToString()}",
                Style = new ReportStyle()
            });

            if (CurrentTransferRequest != null && CurrentManufactureOrder != null)
            {

                items.Add(new LiveReportItem
                {
                    Label = "Transfering to",
                    Value = $"{CurrentTransferRequest.DestinationWip.Name}",
                    Style = new ReportStyle()
                });
                items.Add(new LiveReportItem
                {
                    Label = "Material",
                    Value = $"{CurrentManufactureOrder.Material.CommonName}",
                    Style = new ReportStyle()
                });
                items.Add(new LiveReportItem
                {
                    Label = "State",
                    Value = $"{OutletState?.StateLabel}",
                    Style = new ReportStyle()
                });

            }
            else if (CurrentManufactureOrder != null)
            {
                items.Add(new LiveReportItem
                {
                    Label = "Batch Size",
                    Value = $"{CurrentManufactureOrder.BatchSize.ToString()}",
                    Style = new ReportStyle()
                });

                items.Add(new LiveReportItem
                {
                    Label = "Material",
                    Value = $"{CurrentManufactureOrder.Material.CommonName}",
                    Style = new ReportStyle()
                });
                items.Add(new LiveReportItem
                {
                    Label = "Producing To",
                    Value = $"{CurrentManufactureOrder.Wip.Name}",
                    Style = new ReportStyle()
                });
                items.Add(new LiveReportItem
                {
                    Label = "Step",
                    Value = $"{InletState?.StateLabel}",
                    Style = new ReportStyle()
                });
                if (InletState is IShortLabelInletStateMixer label && !string.IsNullOrEmpty(label.ShortLabel))
                {
                    items.Add(new LiveReportItem
                    {
                        Label = "Step State",
                        Value = $"{label.ShortLabel}",
                        Style = new ReportStyle()
                    });
                }
                items.Add(new LiveReportItem
                {
                    Label = "Current Batch Time",
                    Value = $"{CurrentManufactureOrder.CurrentBatchTime.ToString()}",
                    Style = new ReportStyle()
                });
                items.Add(new LiveReportItem
                {
                    Label = "Starved Time",
                    Value = $"{CurrentManufactureOrder.CurrentStarvedTime.ToString()}",
                    Style = new ReportStyle
                    {
                        Color = "Orange",
                        FontEmphasis = "Bold"
                    }
                });
            }
            return items;
        }
        //Lo que estamos produciendo ahora mismo
        public MixerManufactureOrder CurrentManufactureOrder { get; set; } = null!;
        public Queue<MixerManufactureOrder> ManufacturingOrders { get; set; } = new();

        public void ReceiveManufactureOrderFromWIP(FromWIPToMixerManufactureOrder order)
        {
            //Recibe informacion de que hay que producir y lo mete en la cola del mixer seleccionado por el WIP

            MixerManufactureOrder newOrderMixer = new MixerManufactureOrder(this, order.WIPTank, order.Material);
            ManufacturingOrders.Enqueue(newOrderMixer);
            order.WIPTank.ReceiveManufactureOrderFromMixer(newOrderMixer);
        }


        public bool InitBatchFromQueue()
        {
            if (CurrentManufactureOrder == null && CurrentTransferRequest == null && ManufacturingOrders.Count > 0)
            {
                CurrentLevel = new Amount(0, MassUnits.KiloGram);

                CurrentManufactureOrder = ManufacturingOrders.Dequeue();

                return true;
            }
            return false;
        }


        public TransferFromMixertoWIPOrder? CurrentTransferRequest { get; set; } = null;


        public void ReceiveTransferReInitStarvedFromWIP()
        {
            //Recibe esta informacion la procesa el manejador de estados
            TransferdStarvedReleased = true;

        }
        public void ReceiveTransferFinalizedFromWIP()
        {
            //Recibe esta informacion la procesa el manejador de estados
            if (CurrentTransferRequest != null)
            {
                CurrentManufactureOrder = null!;
                CurrentLevel = ZeroMass;
                CurrentTransferRequest = null;
                InletFinalizedCurrentTransferReceived = true;
                OutletFinalizedCurrentTransferReceived = true;
            }

        }
      

        public void ReceiveReportOfMassDelivered(Amount massdelivered)
        {
            //Recibe esta informacion la procesa el manejador de estados
            if (CurrentTransferRequest != null)
            {
                CurrentLevel -= massdelivered;
            }

        }
        public bool IsCurrentBatchFinalized => CurrentManufactureOrder?.IsBatchFinished ?? false;

        public IMaterial? LastMaterial { get; set; } = null!;


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

            LastMaterial = CurrentOrder.Material;
            if (washDef != null)
            {

                return true;
            }

            return false;
        }
        public bool IsWashoutPumpAvailable()
        {
            if (!ProcessFeederManager.AnyWashoutPumpAvailable())
            {
                // ❌ No hay bombas → encolar y retornar false
                ProcessFeederManager.EnqueueWashoutRequest(this);
                return false;
            }

            Feeder = ProcessFeederManager.AssignWashingPump(this);

            if (Feeder != null)
            {
                // ✅ Asignación exitosa → retornar true
                return true;
            }
            // ❌ Asignación falló → encolar y retornar false
            ProcessFeederManager.EnqueueWashoutRequest(this);
            return false;
        }
        public Amount GetWashoutTime()
        {
            if (LastMaterial != null)
            {
                var washDef = WashoutTimes
                .FirstOrDefault(x => x.ProductCategoryCurrent == CurrentOrder.Material?.ProductCategory &&
                                   x.ProductCategoryNext == LastMaterial.ProductCategory);
                if (washDef != null)
                {
                    LastMaterial = CurrentOrder.Material;
                    return washDef.MixerWashoutTime;
                }
            }

            return new Amount(0, TimeUnits.Second);
        }
        public bool ReleaseWashingPump()
        {
            if (ProcessFeederManager.ReleaseEquipment(this))
            {

                Feeder = null!;
                return true;
            }
            return false;
        }
        public bool IsWashoutPumpFree()
        {
            if (Feeder != null)
            {
                return true;
            }
            return false;
        }

        public bool IsManufacturingRecipeFinished()
        {
            if (CurrentManufactureOrder != null && CurrentManufactureOrder.RecipeSteps.Any())
            {
                CurrentManufactureOrder.CurrentStep = CurrentManufactureOrder.RecipeSteps.Dequeue();

                return false;
            }


            return true;
        }
        public bool IsCurrentStepDifferentThanAdd()
        {
            if (CurrentManufactureOrder == null || CurrentManufactureOrder.CurrentStep == null) return false;
            if (CurrentManufactureOrder.CurrentStep.BackBoneStepType != BackBoneStepType.Add)
            {

                return true;
            }
            return false;

        }
        public bool IsCurrentStepIsAdd()
        {
            if (CurrentManufactureOrder == null || CurrentManufactureOrder.CurrentStep == null) return false;
            if (CurrentManufactureOrder.CurrentStep.BackBoneStepType == BackBoneStepType.Add)
            {
                return true;
            }

            return false;
        }
        public bool IsCurrentStepFeederAvailable()
        {
            if (CurrentManufactureOrder == null || CurrentManufactureOrder.CurrentStep == null) return false;

            var step = ProcessFeederManager.TryAssignFeeder(CurrentManufactureOrder.CurrentStep.RawMaterialId, this);

            if (step == null)
            {
                IsFeederStartved = true;
                return false;
            }
            Feeder = step;
            step.ActualFlow = step.Flow;
            CalculateMassSteRequirements();
            return true;
        }
        public bool IsFeederStartved = false;

        public bool IsFeederStarvedRealesed()
        {
            if (!IsFeederStartved) return true;
            return false;
        }
        public override void GetReleaseFromManager(IManufactureFeeder feeder)
        {
            Feeder = feeder;
            Feeder.ActualFlow = Feeder.Flow;
            CalculateMassSteRequirements();
            IsFeederStartved = false;
        }
        public void ReleseCurrentMassStep()
        {
            if (Feeder != null)
            {
                Feeder.ActualFlow = ZeroFlow;
                Feeder = null!;
                ProcessFeederManager.ReleaseEquipment(this);
            }
        }
        public bool IsTransferOrderSent()
        {
            if (CurrentManufactureOrder != null)
            {
                CurrentManufactureOrder.Wip.ReleaseManufactureOrderFromMixer1(CurrentManufactureOrder);
                TransferFromMixertoWIPOrder newTransferOrder =
                    new TransferFromMixertoWIPOrder(this, CurrentManufactureOrder.Wip, CurrentManufactureOrder.BatchSize,
                    OutletPump?.Flow ?? new Amount(0, MassFlowUnits.Kg_min));
                CurrentManufactureOrder.Wip.ReceiveTransferRequestFromMixer(newTransferOrder);


            }
            return true;
        }
        bool TransferRequestReceived = false;
        public void ReceiveTransferOrderFromWIPToInit(TransferFromMixertoWIPOrder TransferRequest)
        {
            //El manejador de estados del mixer manejara esta informacion
            CurrentTransferRequest = TransferRequest;
            TransferRequestReceived = true;
        }
        public bool IsTransferRequestReceived()
        {
            if (TransferRequestReceived)
            {
                TransferRequestReceived = false;
                return true;
            }
            return false;
        }
        public bool IsTransferStarved()
        {
            if (TransferStarved)
            {
                TransferStarved = false;
                return true;
            }
            return false;
        }
        bool TransferStarved = false;
        public void ReceiveTransferStarvedFromWIP()
        {
            //Recibe esta informacion la procesa el manejador de estados
            TransferStarved = true;

        }
        bool TransferdStarvedReleased = false;
        public bool IsTranferStarvedReleased()
        {
            if (TransferdStarvedReleased)
            {
                TransferdStarvedReleased = false;
                return true;
            }
            return false;
        }
        bool InletFinalizedCurrentTransferReceived = false;
        bool OutletFinalizedCurrentTransferReceived = false;
        public bool IsOutletTransferFinished()
        {
            if (OutletFinalizedCurrentTransferReceived)
            {
                OutletFinalizedCurrentTransferReceived = false;
                return true;
            }
            return false;
        }
        public bool IsInletTransferFinished()
        {
            if (InletFinalizedCurrentTransferReceived)
            {
                InletFinalizedCurrentTransferReceived = false;
                return true;
            }
            return false;
        }
        public void CalculateMassSteRequirements()
        {
            if (CurrentManufactureOrder == null && CurrentManufactureOrder!.CurrentStep is null) return;

            var step = CurrentManufactureOrder.CurrentStep;
            var batchsize = CurrentManufactureOrder.BatchSize;
            RequiredMass = step.Percentage / 100 * batchsize;
            StepMass = Feeder.Flow * OneSecond;
            PendingMass += RequiredMass;



        }
        public Amount RequiredMass = new Amount(0, MassUnits.KiloGram);
        public Amount PendingMass = new Amount(0, MassUnits.KiloGram);
        Amount StepMass = new Amount(0, MassUnits.KiloGram);
        public bool IsMassStepFinalized()
        {
            if (PendingMass <= ZeroMass)
            {
                RequiredMass = new Amount(0, MassUnits.KiloGram);
                PendingMass = new Amount(0, MassUnits.KiloGram);
                StepMass = new Amount(0, MassUnits.KiloGram);
                ReleseCurrentMassStep();
                return true;
            }
            return false;
        }
        public void CalculateMassStep()
        {


            if (Feeder is ProcessOperator)
            {
                PendingMass -= RequiredMass;
                CurrentLevel += RequiredMass;
            }
            else
            {
                if (PendingMass <= StepMass)
                {
                    StepMass = PendingMass;
                }
                PendingMass -= StepMass;
                CurrentLevel += StepMass;
            }

            CurrentManufactureOrder.CurrentBatchTime += OneSecond;
        }

    }


}
