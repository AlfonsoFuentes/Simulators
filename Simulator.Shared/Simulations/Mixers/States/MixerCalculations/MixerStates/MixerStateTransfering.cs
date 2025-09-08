using Simulator.Shared.Simulations.Pumps;

namespace Simulator.Shared.Simulations.Mixers.States.MixerCalculations.MixerStates
{
    public class MixerStateTransfering : MixerState
    {


        public MixerStateTransfering(BaseMixer mixer) : base(mixer, "Transfering")
        {
            Init();
          


        }

        BasePump Pump { get; set; } = null!;

        public override void CheckState()
        {
            if (Mixer.ProducingTo.IsTankHiLevel)
            {
               
                Mixer.MixerState = new MixerStateStarvedByTankHiLevel(Mixer);
                if (Mixer.CurrentEventId == Guid.Empty)
                {
                    Mixer.StartEquipmentEvent($"Wip tank {Mixer.ProducingTo.Name} High Level");
                }
            }
            else if (Mixer.CurrentLevel == ZeroLevel)
            {
               
                Mixer.MixerState = new MixerStateRealisingWIP(Mixer);

            }

        }

        public void Init()
        {

            var Tank = Mixer.ProducingTo;
            bool TransferTankAvaialbleAtInit = Tank.GetTankAvailableforTransferMixer(Mixer);
            if (TransferTankAvaialbleAtInit)
            {
                Pump = Mixer.OutletPump;
                Mixer.InitBatchTransferDate = Mixer.CurrentDate;
                Tank.SetInletOcupiedBy(Pump);

            }


        }

        Amount OneSecond = new(1, TimeUnits.Second);

        public override void CalculateState()
        {
            if (Pump != null && Mixer.CurrentLevel != ZeroLevel)
            {
                
                var mass = Mixer.CurrentLevel < Pump.Flow * OneSecond ? Mixer.CurrentLevel : Pump.Flow * OneSecond;
                Mixer.AddMassToMixer(-mass);
                Pump.SetOutletFlow(mass / OneSecond);
            }
            else
            {
            
                Mixer.MixerState = new MixerStateStarvedByTankNoAvailable(Mixer);
                Mixer.StartEquipmentEvent($"Wip tank {Mixer.ProducingTo.Name} No Available");

            }



        }

       
    }

}
