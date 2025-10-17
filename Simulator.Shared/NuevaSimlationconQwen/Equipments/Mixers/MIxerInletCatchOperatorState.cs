
using Simulator.Shared.NuevaSimlationconQwen.Reports;

namespace Simulator.Shared.NuevaSimlationconQwen.Equipments.Mixers
{
    public class MixerInletCatchOperatorAtInitState : MixerInletState
    {
        public MixerInletCatchOperatorAtInitState(ProcessMixer mixer) : base(mixer)
        {

            StateLabel = $"Catching Operator";
            AddTransition<MixerInletReviewForWashingAtInitBatchState>(mixer =>
            {
                var operators = mixer.InletOperators;
                if (operators.All(x => x.OperatorHasNotRestrictionToInitBatch))
                {

                    return true;
                }
                return false;
            });
            AddTransition<MixerInletReviewCatchOperatorIfHaveStarvedTimeState>();
        }
    }
    public class MixerInletReviewCatchOperatorIfHaveStarvedTimeState : MixerInletState
    {
        public MixerInletReviewCatchOperatorIfHaveStarvedTimeState(ProcessMixer mixer) : base(mixer)
        {

            StateLabel = $"Review operator if has restriction Time";
            AddTransition<MixerInletTryCatchOperatorStarvedTimeState>(mixer =>
            {
                var operators = mixer.InletOperators.FirstOrDefault();
                if (operators != null && operators.MaxRestrictionTime.Value > 0)
                {
                    return true;
                }
                return false;
            });
            AddTransition<MixerInletTryCatchOperatorStarvedNoTimeState>();
        }
    }
    public class MixerInletTryCatchOperatorStarvedTimeState : MixerInletState
    {
        public MixerInletTryCatchOperatorStarvedTimeState(ProcessMixer mixer) : base(mixer)
        {

            StateLabel = $"Try Catching Operator If Has restriction time";
            AddTransition<MixerInletReviewForWashingAtInitBatchState>(mixer =>
            {
                if (mixer.IsOperatorAvailable())
                {
                    Context.IsOperatorStarvedAtInit = true;
                    Context.OperatorStarvedTime = mixer.ProcessOperator?.MaxRestrictionTime ?? new Amount(0, TimeUnits.MilliSecond);
                    return true;
                }
                return false;

            });
            AddTransition<MixerInletStarvedCatchOperatorStarvedTimeState>();
        }
    }
    public class MixerInletStarvedCatchOperatorStarvedTimeState : MixerInletState, IMixerStarved
    {
        public MixerInletStarvedCatchOperatorStarvedTimeState(ProcessMixer mixer) : base(mixer)
        {
            mixer.StartCriticalReport(mixer, "Starved by Operator", "Operator no available");

            StateLabel = $"Starved Catching Operator If has restriction time";
            AddTransition<MixerInletReviewForWashingAtInitBatchState>(mixer =>
            {
                if (mixer.ProcessOperator != null && mixer.ProcessOperator.OcuppiedBy == mixer)
                {
                    Context.IsOperatorStarvedAtInit = true;
                    Context.OperatorStarvedTime = mixer.ProcessOperator?.MaxRestrictionTime ?? new Amount(0, TimeUnits.MilliSecond);
                    
                    return true;
                }
                return false;

            });

        }
        public override void Run(DateTime currentdate)
        {
            Context.CurrentBatchReport.RecordTime(MixerTimeType.StarvedByInitByOperator);
        }
    }

    public class MixerInletTryCatchOperatorStarvedNoTimeState : MixerInletState
    {
        public MixerInletTryCatchOperatorStarvedNoTimeState(ProcessMixer mixer) : base(mixer)
        {

            StateLabel = $"Try Catching Operator";
            AddTransition<MixerInletReviewForWashingAtInitBatchState>(mixer =>
            {
                if (mixer.IsOperatorAvailable())
                {

                    return true;
                }
                return false;

            });
            AddTransition<MixerInletStarvedCatchOperatorStarvedNoTimeState>();
        }
    }
    public class MixerInletStarvedCatchOperatorStarvedNoTimeState : MixerInletState, IMixerStarved
    {
        public MixerInletStarvedCatchOperatorStarvedNoTimeState(ProcessMixer mixer) : base(mixer)
        {
            mixer.StartCriticalReport(mixer, "Starved by Operator", "Operator no available");
            StateLabel = $"Starved Catching Operator No time";
            AddTransition<MixerInletReviewForWashingAtInitBatchState>(mixer =>
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
            Context.CurrentBatchReport.RecordTime(MixerTimeType.StarvedByInitByOperator);
        }
    }
}
