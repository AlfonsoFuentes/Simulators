using Simulator.Shared.Enums.HCEnums.Enums;
using Simulator.Shared.NuevaSimlationconQwen.Equipments.Pumps;
using Simulator.Shared.NuevaSimlationconQwen.Equipments.Tanks;
using Simulator.Shared.NuevaSimlationconQwen.States.BaseClass;
using System.Reflection.Metadata.Ecma335;
using static Simulator.Shared.StaticClasses.StaticClass;

namespace Simulator.Shared.NuevaSimlationconQwen.Equipments.Mixers
{
    public interface IShortLabelInletStateMixer
    {
        string ShortLabel { get; }
    }
    public abstract class MixerInletState : InletState<ProcessMixer>, IShortLabelInletStateMixer
    {
        protected ProcessMixer _mixer = null!;
        protected MixerInletState(ProcessMixer mixer) : base(mixer)
        {
            _mixer = mixer;
        }
        public string ShortLabel { get; set; } = string.Empty;
    }

    public class MixerInletWaitingForManufactureOrderState : MixerInletState
    {

        public MixerInletWaitingForManufactureOrderState(ProcessMixer mixer) : base(mixer)
        {

            StateLabel = $"Waiting for Manufacture Order";
            AddTransition<MixerInletReviewForWashingAtInitBatchState>(mixer => mixer.InitBatchFromQueue());
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
            AddTransition<MixerInletStarvedWashingTankState>(mixer => !mixer.IsWashoutPumpAvailable());
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
            AddTransition<MixerInletReleaseWashingPumpTankState>(mixer => IsWashingTimeCompleted());
        }
        public override void Run(DateTime currentdate)
        {
            StateLabel = $"{Context.Name} Washing Tank {Math.Round(PendingTime.GetValue(TimeUnits.Minute), 1)}, min";
            CurrentTime += Context.OneSecond;
        }
        bool IsWashingTimeCompleted()
        {
            if (PendingTime <= Context.ZeroTime)
            {
                return true;
            }
            return false;
        }
    }
    public class MixerInletReleaseWashingPumpTankState : MixerInletState, ITankOuletStarved
    {



        public MixerInletReleaseWashingPumpTankState(ProcessMixer mixer) : base(mixer)
        {

            StateLabel = $"{mixer.Name} releasing Washout pump";
            AddTransition<MixerInletSelectNextStepState>(mixer => mixer.ReleaseWashingPump());

        }
        public override void Run(DateTime currentdate)
        {



        }
    }
    public class MixerInletStarvedWashingTankState : MixerInletState, ITankOuletStarved
    {



        public MixerInletStarvedWashingTankState(ProcessMixer mixer) : base(mixer)
        {

            StateLabel = $"{mixer.Name} Washing Starved";
            AddTransition<MixerInletWashingTankState>(mixer => mixer.IsWashoutPumpFree());
        }

    }

    public class MixerInletSelectNextStepState : MixerInletState
    {


        public MixerInletSelectNextStepState(ProcessMixer mixer) : base(mixer)
        {


            StateLabel = $"SelectNext Step";
            mixer.CurrentManufactureOrder.CurrentStep = null!;
            AddTransition<MixerSendOrderToTransferBatchState>(mixer => mixer.IsManufacturingRecipeFinished());
            AddTransition<MixerBatchingByTimeState>(_mixer => _mixer.IsCurrentStepDifferentThanAdd());
            AddTransition<MixerInletReviewStepByMassTankState>(_mixer => _mixer.IsCurrentStepIsAdd());

        }

    }
    public class MixerInletReviewStepByMassTankState : MixerInletState, ITankOuletStarved
    {



        public MixerInletReviewStepByMassTankState(ProcessMixer mixer) : base(mixer)
        {

            StateLabel = $"{mixer.Name} Selecting Feeder for Add Mass";
            AddTransition<MixerBatchingByMassState>(mixer => mixer.IsCurrentStepFeederAvailable());
            AddTransition<MixerBatchingByMassStarvedByFeederNoAvailableState>(mixer => mixer.IsFeederStartved);
        }

    }
    public class MixerBatchingByMassState : MixerInletState
    {



        public MixerBatchingByMassState(ProcessMixer mixer) : base(mixer)
        {


            StateLabel = $"{Context.CurrentManufactureOrder.CurrentStep.StepNumber} of " +
             $"{Context.CurrentManufactureOrder.TotalSteps} - " +
             $"Adding {Context.CurrentManufactureOrder.CurrentStep.RawMaterialName}";
            ShortLabel =

               $"{Context.PendingMass.ToString()} of {Context.RequiredMass.ToString()}";
            AddTransition<MixerInletSelectNextStepState>(_mixer =>_mixer.IsMassStepFinalized());
        }



        public override void Run(DateTime currentdate)
        {
            Context.CalculateMassStep();
            StateLabel = $"{Context.CurrentManufactureOrder.CurrentStep.StepNumber} of " +
               $"{Context.CurrentManufactureOrder.TotalSteps} - "+ 
               $"Adding {Context.CurrentManufactureOrder.CurrentStep.RawMaterialName}";
            ShortLabel =
                
               $"{Context.PendingMass.ToString()} of {Context.RequiredMass.ToString()}";

        }
       
    }


    public class MixerBatchingByMassStarvedByFeederNoAvailableState : MixerInletState
    {


        public MixerBatchingByMassStarvedByFeederNoAvailableState(ProcessMixer mixer) : base(mixer)
        {

            StateLabel = $"{mixer.CurrentManufactureOrder.CurrentStep.StepNumber} of {mixer.CurrentManufactureOrder.TotalSteps} - ";
            ShortLabel =
                $"Starved by {mixer.CurrentManufactureOrder.CurrentStep.RawMaterialName}";
            AddTransition<MixerBatchingByMassState>(_mixer => _mixer.IsFeederStarvedRealesed());
        }

        public override void Run(DateTime currentdate)
        {
            if (Context.CurrentManufactureOrder == null) return;
            Context.CurrentManufactureOrder.CurrentBatchTime += Context.OneSecond;
            Context.CurrentManufactureOrder.CurrentStarvedTime += Context.OneSecond;

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
            if (Context.CurrentManufactureOrder == null) return;

            _currentTime += _oneSecond;
            Context.CurrentManufactureOrder.CurrentBatchTime += _oneSecond;
            StateLabel = $"{Context.CurrentManufactureOrder.CurrentStep.StepNumber} " +
                $"of {Context.CurrentManufactureOrder.TotalSteps} - " +
                $"{Context.CurrentManufactureOrder.CurrentStep.BackBoneStepType} ";
            ShortLabel =
                $"{PendingTime.ToString()} of {_time.ToString()}";

        }
    }
    public class MixerSendOrderToTransferBatchState : MixerInletState
    {


        public MixerSendOrderToTransferBatchState(ProcessMixer mixer) : base(mixer)
        {

            StateLabel = $"Finishing Batch";
            AddTransition<MixerFinishingBatchState>(_mixer => _mixer.IsTransferOrderSent());
        }


    }
    public class MixerFinishingBatchState : MixerInletState
    {


        public MixerFinishingBatchState(ProcessMixer mixer) : base(mixer)
        {

            StateLabel = $"Finishing Batch";
            AddTransition<MixerInletWaitingForManufactureOrderState>(_mixer => _mixer.IsTransferFinished());
        }


    }


}

