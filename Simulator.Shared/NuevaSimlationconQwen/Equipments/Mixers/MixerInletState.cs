using Simulator.Shared.NuevaSimlationconQwen.Equipments.Operators;
using Simulator.Shared.NuevaSimlationconQwen.Equipments.Tanks.States;
using Simulator.Shared.NuevaSimlationconQwen.ManufacturingOrders;
using Simulator.Shared.NuevaSimlationconQwen.Materials;
using Simulator.Shared.NuevaSimlationconQwen.States.BaseClass;

namespace Simulator.Shared.NuevaSimlationconQwen.Equipments.Mixers
{
    public interface IShortLabelInletStateMixer
    {
        string ShortLabel { get; }
    }
    public interface IMixerStarved
    {

    }
    public abstract class MixerInletState : InletState<ProcessMixer>, IShortLabelInletStateMixer
    {

        protected MixerInletState(ProcessMixer mixer) : base(mixer)
        {

        }
        public string ShortLabel { get; set; } = string.Empty;

        public override void BeforeRun(DateTime currentdate)
        {
            if (Context.IsOperatorStarvedAtInit)
            {
                Context.CalculateOperatorStarvedTimeCompleted();
            }

        }
    }

    public class MixerInletWaitingForManufactureOrderState : MixerInletState, IMixerStarved
    {

        public MixerInletWaitingForManufactureOrderState(ProcessMixer mixer) : base(mixer)
        {

            StateLabel = $"Waiting for Manufacture Order";
            AddTransition<MixerInletCatchOperatorAtInitState>(mixer => mixer.InitBatchFromQueue());

        }
        public override void Run(DateTime currentdate)
        {
            Context.CurrentBatchReport.RecordTime(MixerTimeType.NoBatchAssigned);

        }

    }


    public class MixerInletReviewForWashingAtInitBatchState : MixerInletState
    {
        public MixerInletReviewForWashingAtInitBatchState(ProcessMixer mixer) : base(mixer)
        {

            StateLabel = $"Reviewing Init Washout Batch";
         
            AddTransition<MixerInletReviewWashingTankState>(mixer => mixer.IsMustWashTank());
            AddTransition<MixerInletSelectNextStepState>();
        }


    }
    public class MixerInletReviewWashingTankState : MixerInletState
    {
        public MixerInletReviewWashingTankState(ProcessMixer mixer) : base(mixer)
        {
           
            StateLabel = $"{mixer.Name} review any washing pump available";
            AddTransition<MixerInletWashingTankState>(mixer => mixer.IsWashoutPumpAvailable());
            AddTransition<MixerInletStarvedWashingTankState>();
        }

    }
    public class MixerInletWashingTankState : MixerInletState
    {

        Amount WashingTime = new Amount(0, TimeUnits.Second);
        Amount CurrentTime = new Amount(0, TimeUnits.Second);
        Amount PendingTime => WashingTime - CurrentTime;
        public MixerInletWashingTankState(ProcessMixer mixer) : base(mixer)
        {
       
            WashingTime = mixer.GetWashoutTime();

            StateLabel = $"{mixer.Name} Washing Tank";
            AddTransition<MixerInletSelectNextStepState>(mixer => IsWashingTimeCompleted());

        }
        public override void Run(DateTime currentdate)
        {
            Context.CurrentBatchReport.RecordTime(MixerTimeType.Washing);
            StateLabel = $"{Context.Name} Washing Tank {Math.Round(PendingTime.GetValue(TimeUnits.Minute), 2)}, min";
            CurrentTime += Context.OneSecond;
        }
        bool IsWashingTimeCompleted()
        {
            if (PendingTime <= Context.ZeroTime)
            {
                Context.ReleaseWashoutPump();
                return true;
            }
            return false;
        }
    }

    public class MixerInletStarvedWashingTankState : MixerInletState, IMixerStarved
    {



        public MixerInletStarvedWashingTankState(ProcessMixer mixer) : base(mixer)
        {

            StateLabel = $"{mixer.Name} Washing Starved";



            AddTransition<MixerInletWashingTankState>(mixer =>
            {
                if (mixer.Feeder != null && mixer.Feeder.OcuppiedBy == mixer)
                {
                    return true;
                }
                return false;
            });

        }
        public override void Run(DateTime currentdate)
        {
            Context.CurrentBatchReport.RecordTime(MixerTimeType.StarvedByWashoutPump);

        }
    }

    public class MixerInletSelectNextStepState : MixerInletState
    {


        public MixerInletSelectNextStepState(ProcessMixer mixer) : base(mixer)
        {


            StateLabel = $"Select Next Step";
            mixer.CurrentManufactureOrder.CurrentStep = null!;
            mixer.InitBatchDate();
            AddTransition<MixerFinishingBatchState>(mixer => mixer.IsManufacturingRecipeFinished());
            AddTransition<MixerBatchingByOperatorReviewOperatorAvailabilityState>(_mixer => _mixer.IsCurrentStepByOperator());
            AddTransition<MixerBatchingByTimeState>(_mixer => _mixer.IsCurrentStepDifferentThanAdd());
            AddTransition<MixerBatchingByMassState>(_mixer => _mixer.IsCurrentStepFeederAvailable());
            AddTransition<MixerBatchingByMassStarvedByFeederNoAvailableState>();

        }

    }

    public class MixerBatchingByMassState : MixerInletState
    {
        public Amount RequiredMass = new Amount(0, MassUnits.KiloGram);
        public Amount PendingMass = new Amount(0, MassUnits.KiloGram);
        Amount StepMass = new Amount(0, MassUnits.KiloGram);
        IRecipeStep CurrentStep => CurrentManufactureOrder.CurrentStep;
        Amount BatchSize => CurrentManufactureOrder.BatchSize;
        IManufactureOrder CurrentManufactureOrder => Context.CurrentManufactureOrder;
        public MixerBatchingByMassState(ProcessMixer mixer) : base(mixer)
        {
            Context.CurrentBatchReport.RecordTime(MixerTimeType.Batching);

            CalculateMassSteRequirements();

            StateLabel = $"{Context.CurrentManufactureOrder.CurrentStep.StepNumber} of " +
             $"{Context.CurrentManufactureOrder.TotalSteps} - " +
             $"Adding {Context.CurrentManufactureOrder.CurrentStep.RawMaterialName}";
            ShortLabel =

               $"{PendingMass.ToString()} of {RequiredMass.ToString()}";

            AddTransition<MixerInletSelectNextStepState>(_mixer => IsMassStepFinalized());

        }



        public override void Run(DateTime currentdate)
        {
            Context.CurrentBatchReport.RecordTime(MixerTimeType.Batching);
            CalculateMassStep();
            StateLabel = $"{Context.CurrentManufactureOrder.CurrentStep.StepNumber} of " +
               $"{Context.CurrentManufactureOrder.TotalSteps} - " +
               $"Adding {Context.CurrentManufactureOrder.CurrentStep.RawMaterialName}";
            ShortLabel = $"{PendingMass.ToString()} of {RequiredMass.ToString()}";

        }

        public bool IsMassStepFinalized()
        {
            if (PendingMass <= Context.ZeroMass)
            {
                RequiredMass = new Amount(0, MassUnits.KiloGram);
                PendingMass = new Amount(0, MassUnits.KiloGram);
                StepMass = new Amount(0, MassUnits.KiloGram);
                Context.ReleseCurrentMassStep();
                return true;
            }
            return false;
        }
        public void CalculateMassSteRequirements()
        {


            var step = CurrentStep;
            var batchsize = BatchSize;
            RequiredMass = step.Percentage / 100 * batchsize;
            StepMass = Context.Feeder?.Flow * Context.OneSecond;
            PendingMass += RequiredMass;
            Context.Feeder!.ActualFlow = Context.Feeder.Flow;


        }

        public void CalculateMassStep()
        {


            if (Context.Feeder is ProcessOperator)
            {
                PendingMass -= RequiredMass;
                Context.CurrentLevel += RequiredMass;
            }
            else
            {
                if (PendingMass <= StepMass)
                {
                    StepMass = PendingMass;
                }
                PendingMass -= StepMass;
                Context.CurrentLevel += StepMass;

            }

        
        }


    }


    public class MixerBatchingByMassStarvedByFeederNoAvailableState : MixerInletState, IMixerStarved
    {


        public MixerBatchingByMassStarvedByFeederNoAvailableState(ProcessMixer mixer) : base(mixer)
        {

            StateLabel = $"{mixer.CurrentManufactureOrder.CurrentStep.StepNumber} of {mixer.CurrentManufactureOrder.TotalSteps} ";
            ShortLabel =
                $"Starved by {mixer.CurrentManufactureOrder.CurrentStep.RawMaterialName}";

            AddTransition<MixerBatchingByMassState>(_mixer =>
            {
                if (mixer.Feeder != null && mixer.Feeder.OcuppiedBy == mixer)
                {
                    return true;
                }
                return false;
            });


        }

        public override void Run(DateTime currentdate)
        {
            Context.CurrentBatchReport.RecordTime(MixerTimeType.StarvedByFeeder);
            if (Context.CurrentManufactureOrder == null) return;
   

        }
    }
    public class MixerBatchingByOperatorReviewOperatorAvailabilityState : MixerInletState
    {


        public MixerBatchingByOperatorReviewOperatorAvailabilityState(ProcessMixer mixer) : base(mixer)
        {

            StateLabel = $"Review Operator Availability";

            AddTransition<MixerBatchingByOperatorTimeState>(_mixer =>
            {
                var operators = mixer.InletOperators;
                if (operators.All(x => x.OperatorHasNotRestrictionToInitBatch))
                {
                    return true;
                }
                if (Context.ProcessOperator == null)
                {
                    //quiere decir que ya fue liberado o no se capturo al inicio del batch
                    if (mixer.IsOperatorAvailable())   //Se intenta capturar el operador o se encola
                    {
                        return true;
                    }

                }
                else
                {
                    if (Context.ProcessOperator.MaxRestrictionTime.Value == 0 && Context.ProcessOperator.OcuppiedBy == mixer)
                    {
                        //Se verifica si el operador ya no tiene restriccion de tiempo y esta ocupado por el mixer  
                        return true;
                    }
                    if (Context.OperatorStarvedPendingTime.Value > 0)
                    {
                        //El operador tiene tiempo de restriccion pendiente       lo que quiere decir que ya esta siendo usando por el mixer    

                        return true;
                    }
                    else
                    {
                        //El operador esta libre hay que capturalo si se puede
                        if (mixer.IsOperatorAvailable())   //Se intenta capturar el operador o se encola
                        {
                            return true;
                        }
                    }
                }


                return false;
            });

            AddTransition<MixerBatchingByOperatorStarvedState>();

        }


    }
    public class MixerBatchingByOperatorStarvedState : MixerInletState, IMixerStarved
    {
        public MixerBatchingByOperatorStarvedState(ProcessMixer mixer) : base(mixer)
        {

            StateLabel = $"Batch Starved By Operator";

            AddTransition<MixerBatchingByOperatorTimeState>(_mixer =>
            {
                if (mixer.ProcessOperator != null && mixer.ProcessOperator.OcuppiedBy == mixer)
                {

                    return true;
                }

                return false;
            });

        }
        public override void Run(DateTime currentdate)
        {
            Context.CurrentBatchReport.RecordTime(MixerTimeType.StarvedByOperator);
        }
    }
    public class MixerBatchingByOperatorTimeState : MixerInletState
    {

        private readonly Amount _time;
        private Amount _currentTime = new Amount(0, TimeUnits.Second);
        private readonly Amount _oneSecond = new Amount(1, TimeUnits.Second);

        public Amount PendingTime => _time - _currentTime;

        public MixerBatchingByOperatorTimeState(ProcessMixer mixer) : base(mixer)
        {
            _time = mixer.CurrentManufactureOrder.CurrentStep.Time;

            StateLabel = $"{mixer.CurrentManufactureOrder.CurrentStep.StepNumber} " +
                $"of {mixer.CurrentManufactureOrder.TotalSteps} - " +
                $"{mixer.CurrentManufactureOrder.CurrentStep.BackBoneStepType} ";
            ShortLabel =
                $"{PendingTime.ToString()} of {_time.ToString()}";
            AddTransition<MixerInletSelectNextStepState>(_mixer =>
            {

                if (PendingTime <= _mixer.ZeroTime)
                {
                    if (Context.ProcessOperator == null)
                    {
                        //no hay que liberar al operador porque no hay
                        return true;
                    }
                    if (Context.ProcessOperator.MaxRestrictionTime.Value == 0)
                    {
                        //El operador no tiene restriccion de tiempo
                        return true;
                    }
                    if (Context.OperatorStarvedPendingTime.Value <= 0)
                    {
                        //El operador ya cumplio su tiempo de restriccion quiere decir que si llego aqui fue porque se capturo de nuevo y hay que liberarlo
                        Context.ReleaseOperator();
                        return true;
                    }


                }
                return false;
            });

        }



        public override void Run(DateTime currentdate)
        {
            Context.CurrentBatchReport.RecordTime(MixerTimeType.Batching);

            _currentTime += _oneSecond;
    
            StateLabel = $"{Context.CurrentManufactureOrder.CurrentStep.StepNumber} " +
                $"of {Context.CurrentManufactureOrder.TotalSteps} - " +
                $"{Context.CurrentManufactureOrder.CurrentStep.BackBoneStepType} ";
            ShortLabel =
                $"{PendingTime.ToString()} of {_time.ToString()}";

        }
    }

    public class MixerBatchingByTimeState : MixerInletState
    {

        private readonly Amount _time;
        private Amount _currentTime = new Amount(0, TimeUnits.Second);
        private readonly Amount _oneSecond = new Amount(1, TimeUnits.Second);

        public Amount PendingTime => _time - _currentTime;

        public MixerBatchingByTimeState(ProcessMixer mixer) : base(mixer)
        {
            _time = mixer.CurrentManufactureOrder.CurrentStep.Time;

            StateLabel = $"{mixer.CurrentManufactureOrder.CurrentStep.StepNumber} " +
                $"of {mixer.CurrentManufactureOrder.TotalSteps} - " +
                $"{mixer.CurrentManufactureOrder.CurrentStep.BackBoneStepType} ";
            ShortLabel =
                $"{PendingTime.ToString()} of {_time.ToString()}";

            AddTransition<MixerInletSelectNextStepState>(_mixer => PendingTime <= _mixer.ZeroTime);

        }



        public override void Run(DateTime currentdate)
        {
            Context.CurrentBatchReport.RecordTime(MixerTimeType.Batching);

            _currentTime += _oneSecond;
   
            StateLabel = $"{Context.CurrentManufactureOrder.CurrentStep.StepNumber} " +
                $"of {Context.CurrentManufactureOrder.TotalSteps} - " +
                $"{Context.CurrentManufactureOrder.CurrentStep.BackBoneStepType} ";
            ShortLabel =
                $"{PendingTime.ToString()} of {_time.ToString()}";

        }
    }

    public class MixerFinishingBatchState : MixerInletState
    {


        public MixerFinishingBatchState(ProcessMixer mixer) : base(mixer)
        {

            StateLabel = $"Transfering Batch to Mixer";

        }


    }


}

