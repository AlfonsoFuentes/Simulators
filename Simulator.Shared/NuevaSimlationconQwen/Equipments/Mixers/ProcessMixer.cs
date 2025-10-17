using Simulator.Shared.Enums.HCEnums.Enums;
using Simulator.Shared.NuevaSimlationconQwen.Equipments.Lines;
using Simulator.Shared.NuevaSimlationconQwen.Equipments.Operators;
using Simulator.Shared.NuevaSimlationconQwen.Equipments.Pumps;
using Simulator.Shared.NuevaSimlationconQwen.ManufacturingOrders;
using Simulator.Shared.NuevaSimlationconQwen.Materials;
using Simulator.Shared.NuevaSimlationconQwen.Reports;

namespace Simulator.Shared.NuevaSimlationconQwen.Equipments.Mixers
{
    public partial class ProcessMixer : ManufaturingEquipment, ISetMaterialsAtOutlet, ILiveReportable
    {

        public List<BatchReport> BatchReports { get; set; } = new List<BatchReport>();
        public BatchReport CurrentBatchReport { get; set; } = null!;
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
            OutletState = new MixerOuletWaitingState(this);
        }
        public override void ReceiveManufactureOrderFromWIP(ITankManufactureOrder order)
        {
            CurrentBatchReport = new(this);
            MixerManufactureOrder newOrderMixer = new MixerManufactureOrder(this, order, CurrentBatchReport);
          
            CurrentBatchReport.DateReceivedToInitBatch = this.CurrentDate;
            CurrentBatchReport.BatchSize = newOrderMixer.BatchSize;
            CurrentBatchReport.TheroticalBatchTime = newOrderMixer.BatchTime;
      
          
            BatchReports.Add(CurrentBatchReport);
            ManufacturingOrders.Enqueue(newOrderMixer);
            order.AddMixerManufactureOrder(newOrderMixer);

        }

        public void InitBatchDate()
        {
            CurrentBatchReport.InitRealBatchDate(this.CurrentDate);
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

        public void ReceiveTransferFinalizedFromWIP()
        {
            //Recibe esta informacion la procesa el manejador de estados
            if (CurrentTransferRequest != null)
            {

                CurrentManufactureOrder = null!;
                CurrentLevel = ZeroMass;
                CurrentTransferRequest = null;
                OutletState = new MixerOuletWaitingState(this);
                InletState = new MixerInletWaitingForManufactureOrderState(this);

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
            SentNewTransferRequestToWIP();

            return true;
        }
        public bool IsCurrentStepByOperator()
        {
            if (CurrentManufactureOrder == null || CurrentManufactureOrder.CurrentStep == null) return false;
            if (CurrentManufactureOrder.CurrentStep.BackBoneStepType == BackBoneStepType.Adjust ||
                CurrentManufactureOrder.CurrentStep.BackBoneStepType == BackBoneStepType.Analisys ||
                CurrentManufactureOrder.CurrentStep.BackBoneStepType == BackBoneStepType.Connect_Mixer_WIP ||
                CurrentManufactureOrder.CurrentStep.BackBoneStepType == BackBoneStepType.Transfer_To_Drop)
            {

                return true;
            }
            return false;

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
            if (CurrentManufactureOrder == null || CurrentManufactureOrder.CurrentStep == null || !CurrentManufactureOrder.CurrentStep.RawMaterialId.HasValue) return false;
            var IsOperator = IsMaterialFeederOperator(CurrentManufactureOrder.CurrentStep.RawMaterialId.Value);
            var newFeeder = IsMaterialFeederAvailableForMixer(CurrentManufactureOrder.CurrentStep.RawMaterialId.Value);
            if (IsOperator && newFeeder == null)
            {
                if (ProcessOperator != null && !ProcessOperator.OperatorHasNotRestrictionToInitBatch && ProcessOperator.OcuppiedBy == this)
                {
                    //El operador es el feeder  no hay que asignar ni encolar
                    return true;
                }

            }
            if (newFeeder != null)
            {


                //Es bomba pero no es el operador o el operador es asignable porque no esta en uso en el mixer
                Feeder = newFeeder;
                Feeder.ActualFlow = Feeder.Flow;
                Feeder.OcuppiedBy = this;
                Feeder.OutletState = new FeederIsInUseByAnotherEquipmentState(Feeder);

                return true;

            }
            EnqueueForMaterialFeeder(CurrentManufactureOrder.CurrentStep.RawMaterialId.Value);


            return false;
        }
        public void ReleseCurrentMassStep()
        {

            if (Feeder != null)
            {
                ReleaseFeeder(Feeder);

            }
        }
        void SentNewTransferRequestToWIP()
        {
            if (CurrentManufactureOrder != null)
            {
                CurrentManufactureOrder.WIPOrder.RemoveManufactureOrdersFromMixers(CurrentManufactureOrder);

                TransferFromMixertoWIPOrder newTransferOrder =
                    new TransferFromMixertoWIPOrder(this, CurrentManufactureOrder.WIPOrder.Tank, CurrentManufactureOrder.BatchSize,
                    OutletPump?.Flow ?? new Amount(0, MassFlowUnits.Kg_min));
                CurrentManufactureOrder.WIPOrder.Tank.ReceiveTransferRequestFromMixer(newTransferOrder);
            }
        }




        public void ReceiveTransferOrderFromWIPToInit(TransferFromMixertoWIPOrder TransferRequest)
        {
            //El manejador de estados del mixer manejara esta informacion
            CurrentTransferRequest = TransferRequest;
            OutletState = new MixerOuletTransferingToWIPState(this);
            if (ProcessOperator != null)
            {
                ReleaseOperator();
            }
        }
        public ProcessOperator? ProcessOperator { get; set; } = null!;
        public bool IsOperatorAvailable()
        {

            var operators = InletEquipments.OfType<ProcessOperator>().ToList();

            if (operators.Any())
            {
                var firstoperator = operators.FirstOrDefault(f => f.OutletState is FeederAvailableState);
                if (firstoperator != null)
                {
                    ProcessOperator = AssignOperator();
                    return true;
                }
                else
                {
                    EnqueueForOperator();
                }

            }

            return false;
        }
        public virtual ProcessOperator? AssignOperator()
        {
            var feeder = InletEquipments
                .OfType<ProcessOperator>()
                .FirstOrDefault(f => f.OutletState is FeederAvailableState);

            if (feeder != null)
            {
                feeder.OcuppiedBy = this;

                feeder.OutletState = new FeederIsInUseByAnotherEquipmentState(feeder);


            }
            return feeder;
        }
        public virtual void EnqueueForOperator()
        {
            var candidates = InletEquipments
                .OfType<ProcessOperator>()

                .ToList();

            if (candidates.Any())
            {

                var best = candidates.OrderBy(f => f.GetWaitingQueueLength()).First();
                best.EnqueueWaitingEquipment(this);
            }
        }
        public void ReleaseOperator()
        {
            var _feeder = ProcessOperator;
            if (_feeder != null)
            {
                // 1. Liberar nombre
                _feeder.OcuppiedBy = null!;

                // 2. Cambiar estado a disponible
                _feeder.OutletState = new FeederAvailableState(_feeder);

                // 3. Notificar al siguiente en la cola
                NotifyNextWaitingEquipment(_feeder);
                ProcessOperator = null!;

            }
        }
        public void OnOperatorMayBeAvailable(ProcessMixer mixer, ProcessOperator feeder)
        {
            if (feeder.OutletState is FeederAvailableState || feeder.OutletState is FeederRealesStarvedState)
            {
                mixer.EndCriticalReport();
                feeder.OcuppiedBy = mixer;
                feeder.OutletState = new FeederIsInUseByAnotherEquipmentState(feeder);

                if (mixer.InletState is MixerBatchingByMassStarvedByFeederNoAvailableState)
                {
                    mixer.Feeder = feeder;
                }
                else if (mixer.InletState is MixerInletStarvedCatchOperatorStarvedTimeState ||
                    mixer.InletState is MixerBatchingByOperatorStarvedState ||
                    mixer.InletState is MixerInletStarvedCatchOperatorStarvedNoTimeState)
                {

                    mixer.ProcessOperator = feeder;
                }


            }
        }
        public void NotifyNextWaitingEquipment(ProcessOperator feeder)
        {
            if (feeder.WaitingQueue.Count == 0) return;

            var next = feeder.WaitingQueue.First!.Value;
            feeder.WaitingQueue.RemoveFirst();
            if (next is ProcessMixer mixer)
            {
                OnOperatorMayBeAvailable(mixer, feeder);

            }



        }

        public bool IsOperatorStarvedAtInit { get; set; } = false;
        public Amount OperatorStarvedTime = new Amount(0, TimeUnits.Second);
        public Amount OperatorStarvedCurrenTime = new Amount(0, TimeUnits.Second);
        public Amount OperatorStarvedPendingTime => OperatorStarvedTime - OperatorStarvedCurrenTime;
        public void CalculateOperatorStarvedTimeCompleted()
        {
            if (OperatorStarvedPendingTime <= ZeroTime)
            {
                if (IsOperatorStarvedAtInit)
                {

                    OperatorStarvedTime = new Amount(0, TimeUnits.Second);
                    OperatorStarvedCurrenTime = new Amount(0, TimeUnits.Second);
                    IsOperatorStarvedAtInit = false;
                }
                ReleaseOperator();

            }
            else
            {
                OperatorStarvedCurrenTime += OneSecond;
            }


        }
    }


}
