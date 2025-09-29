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
        public ReportColumn ReportColumn => ReportColumn.Column2_SkidsAndMixers;
        public ReportPriorityInColumn ReportPriority => ReportPriorityInColumn.Low;

        public List<ProcessLine> PreferredLines { get; set; } = new List<ProcessLine>();
        public override Amount CurrentLevel { get; set; } = new Amount(0, MassUnits.KiloGram);

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

            InletState = new MixerInletWaitingForManufactureOrderState(this);
            OutletState = new MixerOuletInitialState(this);
        }
        public override void ReceiveManufactureOrderFromWIP(IVesselManufactureOrder order)
        {
            MixerManufactureOrder newOrderMixer = new MixerManufactureOrder(this, order);
            ManufacturingOrders.Enqueue(newOrderMixer);
            order.AddMixerManufactureOrder(newOrderMixer);

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



        public bool IsMustWashTank()
        {
            if (CurrentManufactureOrder == null) return false;


            if (LastMaterial == null)
            {

                LastMaterial = CurrentManufactureOrder.Material;
                return false;
            }
            if (CurrentManufactureOrder.Material == null) return false;
            if (CurrentManufactureOrder.Material.Id == LastMaterial.Id) return false;

            var washDef = WashoutTimes
                .FirstOrDefault(x => x.ProductCategoryCurrent == CurrentManufactureOrder.Material?.ProductCategory &&
                                   x.ProductCategoryNext == LastMaterial.ProductCategory);


            if (washDef != null)
            {

                return true;
            }

            return false;
        }
        
        public Amount GetWashoutTime()
        {
            if (LastMaterial != null)
            {
                var washDef = WashoutTimes
                .FirstOrDefault(x => x.ProductCategoryCurrent == CurrentManufactureOrder.Material?.ProductCategory &&
                                   x.ProductCategoryNext == LastMaterial.ProductCategory);
                if (washDef != null)
                {
                    LastMaterial = CurrentManufactureOrder.Material;
                    return washDef.MixerWashoutTime;
                }
            }

            return new Amount(0, TimeUnits.Second);
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
            if (CurrentManufactureOrder == null || CurrentManufactureOrder.CurrentStep == null|| !CurrentManufactureOrder.CurrentStep.RawMaterialId.HasValue) return false;
            if(IsMaterialFeederAvailable(CurrentManufactureOrder.CurrentStep.RawMaterialId.Value))
            {
                Feeder = AssignMaterialFeeder(CurrentManufactureOrder.CurrentStep.RawMaterialId.Value);
                if (Feeder != null)
                {
                    CalculateMassSteRequirements(); 
                    return true;
                }
               
            }
            EnqueueForMaterialFeeder(CurrentManufactureOrder.CurrentStep.RawMaterialId.Value);
          

            return false;
        }
        
        
       
       
        public bool IsTransferOrderSent()
        {
            if (CurrentManufactureOrder != null)
            {
                CurrentManufactureOrder.WIPOrder.RemoveManufactureOrdersFromMixers(CurrentManufactureOrder);

                TransferFromMixertoWIPOrder newTransferOrder =
                    new TransferFromMixertoWIPOrder(this, CurrentManufactureOrder.WIPOrder.WIP, CurrentManufactureOrder.BatchSize,
                    OutletPump?.Flow ?? new Amount(0, MassFlowUnits.Kg_min));
                CurrentManufactureOrder.WIPOrder.WIP.ReceiveTransferRequestFromMixer(newTransferOrder);


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
            StepMass = Feeder?.Flow * OneSecond;
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

        public void ReleseCurrentMassStep()
        {
            if (Feeder != null)
            {
               
                ReleaseFeeder(Feeder);
            

            }
        }
    }


}
