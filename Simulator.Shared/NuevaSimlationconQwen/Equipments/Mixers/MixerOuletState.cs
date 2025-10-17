using Simulator.Shared.NuevaSimlationconQwen.States.BaseClass;

namespace Simulator.Shared.NuevaSimlationconQwen.Equipments.Mixers
{
    public abstract class MixerOuletState : OutletState<ProcessMixer>
    {
        protected MixerOuletState(ProcessMixer mixer) : base(mixer)
        {
        }


    }


    public class MixerOuletWaitingState : MixerOuletState
    {


        public MixerOuletWaitingState(ProcessMixer mixer) : base(mixer)
        {
            StateLabel = $"Waiting for Receive Transfer Request";



        }


    }


    public class MixerOuletTransferingToWIPState : MixerOuletState
    {


        public MixerOuletTransferingToWIPState(ProcessMixer mixer) : base(mixer)
        {
            if (mixer.CurrentTransferRequest != null)
            {
                StateLabel = $"Transfering to {mixer.CurrentTransferRequest.DestinationWip.Name}";
            }


            AddTransition<MixerOuletTransferingToWIPStarvedState>(mixer =>
            {
                if (mixer.CurrentTransferRequest?.IsTransferStarved == true)
                {

                    return true;
                }
                return false;
            });



        }
        public override void Run(DateTime currentdate)
        {
            Context.CurrentBatchReport.RecordTime(MixerTimeType.TransferToWIP);
        }



    }
    public class MixerOuletTransferingToWIPStarvedState : MixerOuletState, IMixerStarved
    {


        public MixerOuletTransferingToWIPStarvedState(ProcessMixer mixer) : base(mixer)
        {
            StateLabel = $"Transfering to WIP Starved";
            AddTransition<MixerOuletTransferingToWIPState>(mixer =>
            {
                if (mixer.CurrentTransferRequest?.IsTransferAvailable == true)
                {


                    return true;
                }
                return false;
            });


        }
        public override void Run(DateTime currentdate)
        {
            Context.CurrentBatchReport.RecordTime(MixerTimeType.StarvedByTransferToWIP);
        }
    }


}