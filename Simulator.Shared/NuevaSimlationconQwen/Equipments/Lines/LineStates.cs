using Simulator.Shared.NuevaSimlationconQwen.States.BaseClass;

namespace Simulator.Shared.NuevaSimlationconQwen.Equipments.Lines
{
    public interface IProducerState
    {

    }
    public interface IProducerAUState
    {

    }
    public interface IStarvedLine
    {

    }
    public abstract class LineOutletState : OutletState<ProcessLine>
    {
        protected ProcessLine _line { get; set; }

        public LineOutletState(ProcessLine line) : base(line)
        {
            _line = line;
        }
    }
    public class LineStateInitialState : LineOutletState
    {

        public LineStateInitialState(ProcessLine mixer) : base(mixer)
        {

            StateLabel = $"Initial to State";
            AddTransition<LineReviewAtInitStateInitialState>();
        }


    }
    public class LineReviewAtInitStateInitialState : LineOutletState
    {

        public LineReviewAtInitStateInitialState(ProcessLine line) : base(line)
        {


            StateLabel = $"Review Initial to State";
            AddTransition<LineStateNotScheduledState>(line => !line.IsLineScheduled);
            AddTransition<LineStateReviewProducingState>(line => line.SelectProductionRun());

        }
    }
    public class LineStateNotScheduledState : LineOutletState
    {
        public LineStateNotScheduledState(ProcessLine equipment) : base(equipment)
        {
            StateLabel = $"{equipment.Name} Not Scheduled";
        }


    }

    public class LineStateReviewProducingState : LineOutletState
    {


        public LineStateReviewProducingState(ProcessLine line) : base(line)
        {

            StateLabel = $"{line.Name} Review Producing";
            AddTransition<LineStateNotThisShiftScheduledState>(line => !line.IsScheduledForShift(line.ActualShift));
            AddTransition<LineStateStarvedByTankLowLevelState>(line => line.IsLineStarvedByLowLevelWips());
            AddTransition<LineStateProducingState>(line => !line.IsLineStarvedByLowLevelWips());



        }

    }
    public class LineStateNotThisShiftScheduledState : LineOutletState
    {


        public LineStateNotThisShiftScheduledState(
            ProcessLine line) : base(line)
        {

            StateLabel = $"{line.Name} Not Scheduled this Shift";
            AddTransition<LineStateReviewProducingState>(line => line.IsScheduledForShift(line.ActualShift));
        }

    }
    public class LineStateStarvedByTankLowLevelState : LineOutletState, IStarvedLine
    {

        public LineStateStarvedByTankLowLevelState(ProcessLine line) : base(line)
        {

            StateLabel = $"{line.Name} Starved by Low Level WIP";

            AddTransition<LineStateReviewProducingState>(line => line.IsLineAvailableAfterStarved());
        }



    }
    public class LineStateProducingStarvedByAuState : LineOutletState, IProducerAUState
    {


        public LineStateProducingStarvedByAuState(ProcessLine line) : base(line)
        {
            line.CalculateAU();
            StateLabel = $"{line.Name} Starved by AU";
            AddTransition<LineStatePlannedDowntimeState>(line => line.IsPlannedDowntime());
            AddTransition<LineStateProducingState>(line => line.MustRunProducing());

        }



        public override void Run(DateTime currentdate)
        {
            _line.RunByAu();
        }
    }
    public class LineStatePlannedDowntimeState : LineOutletState
    {


        public LineStatePlannedDowntimeState(ProcessLine line) : base(line)
        {

            StateLabel = $"{line.Name} Starved by planned down time";
            AddTransition<LineStateReviewProducingState>(line => line.IsPlannedDowntimeAchieved());

        }

    }

    public class LineStateProducingState : LineOutletState, IProducerState
    {


        public LineStateProducingState(ProcessLine line) : base(line)
        {


            StateLabel = $"{line.Name} Producing";
            AddTransition<LineStatePlannedDowntimeState>(line => line.IsPlannedDowntime());
            // ✅ 1. ¿Terminó la producción? → Revisar WIPs/tanques
            AddTransition<LineStateReviewWIPsTanksState>(line => line.IsCurrentProductionFinished());

            // ✅ 2. ¿Hambrienta por WIP? → Detener
            AddTransition<LineStateStarvedByTankLowLevelState>(line => line.IsLineStarvedByLowLevelWips());

            // ✅ 3. ¿No en turno? → Detener
            AddTransition<LineStateNotThisShiftScheduledState>(line => !line.IsScheduledForShift(line.ActualShift));

            // ✅ 4. ¿Debe pasar a AU? → Cambiar modo
            AddTransition<LineStateProducingStarvedByAuState>(line => line.MustRunByAu());


        }


        public override void Run(DateTime currentdate)
        {
            _line.RunByProducing();
        }
    }
    public class LineStateReviewWIPsTanksState : LineOutletState
    {


        public LineStateReviewWIPsTanksState(ProcessLine line) : base(line)
        {

            StateLabel = $"{line.Name} Review WIPs Tanks to empty";
            AddTransition<LineStateProducingToEmptyWIPsState>(line => line.MustEmptyWipTanks());

            AddTransition<LineStateChangeFormatState>(line => line.MustChangeFormat());
            AddTransition<LineStateNotScheduledState>(line => line.IfCanStopLineCompletely());
            AddTransition<LineStateReviewProducingState>(line => line.SelectProductionRun());

        }

    }
    public class LineStatePlannedToEmptyDowntimeState : LineOutletState
    {


        public LineStatePlannedToEmptyDowntimeState(ProcessLine line) : base(line)
        {

            StateLabel = $"{line.Name} Starved by planned down time";
            AddTransition<LineStateProducingToEmptyWIPsState>(line => line.IsPlannedDowntimeAchieved());

        }

    }
    public class LineStateNotThisShiftToEmptyTankScheduledState : LineOutletState
    {


        public LineStateNotThisShiftToEmptyTankScheduledState(
            ProcessLine line) : base(line)
        {

            StateLabel = $"{line.Name} Not Scheduled this Shift";
            AddTransition<LineStateProducingToEmptyWIPsState>(line => line.IsScheduledForShift(line.ActualShift));
        }

    }
    public class LineStateProducingToEmptyWIPsState : LineOutletState, IProducerState
    {


        public LineStateProducingToEmptyWIPsState(ProcessLine line) : base(line)
        {


            StateLabel = $"{line.Name} Producing";
            AddTransition<LineStatePlannedToEmptyDowntimeState>(line => line.IsPlannedDowntime());


            // ✅ 2. ¿Hambrienta por WIP? → Detener
            AddTransition<LineStateStarvedToEmptyTankLowLevelState>(line => line.IsLineStarvedByLowLevelWipsWhenEmptyTankToChangeMaterial());

            AddTransition<LineStateNotThisShiftToEmptyTankScheduledState>(line => !line.IsScheduledForShift(line.ActualShift));

            // ✅ 4. ¿Debe pasar a AU? → Cambiar modo
            AddTransition<LineStateProducingToEmptyStarvedByAuState>(line => line.MustRunByAu());


        }


        public override void Run(DateTime currentdate)
        {
            _line.RunByProducing();
        }
    }
    public class LineStateStarvedToEmptyTankLowLevelState : LineOutletState, IStarvedLine
    {

        public LineStateStarvedToEmptyTankLowLevelState(ProcessLine line) : base(line)
        {

            StateLabel = $"{line.Name} Starved by Low Level WIP";
            
            AddTransition<LineStateChangeFormatAfterEmptyTankState>(line => line.MustChangeFormat());
            AddTransition<LineStateReviewProducingState>(line => line.SelectProductionRun());
        }



    }
    public class LineStateProducingToEmptyStarvedByAuState : LineOutletState, IProducerState
    {


        public LineStateProducingToEmptyStarvedByAuState(ProcessLine line) : base(line)
        {
            line.CalculateAU();
            StateLabel = $"{line.Name} Starved by AU";
            AddTransition<LineStatePlannedToEmptyDowntimeState>(line => line.IsPlannedDowntime());
            AddTransition<LineStateProducingToEmptyWIPsState>(line => line.MustRunProducing());

        }



        public override void Run(DateTime currentdate)
        {
            _line.RunByAu();
        }
    }

    public class LineStateReviewNextSKUState : LineOutletState
    {


        public LineStateReviewNextSKUState(ProcessLine line) : base(line)
        {

            StateLabel = $"{line.Name} Review Next SKU";
            AddTransition<LineStateNotScheduledState>(line => line.IfCanStopLineCompletely());
            AddTransition<LineStateReviewProducingState>(line => line.SelectProductionRun());


        }




    }
    public class LineStateChangeFormatState : LineOutletState
    {

        Amount _currentTime = new Amount(0, TimeUnits.Second);

        readonly Amount _totalTime = new Amount(0, TimeUnits.Second);
        Amount PendingTime => _totalTime - _currentTime;
        public LineStateChangeFormatState(ProcessLine line) : base(line)
        {
            _totalTime = line.TimeToChangeFormat; // ← Tiempo de cambio de formato desde la línea
            _line.SetPumpsFlowToZero();
          
            StateLabel = $"{line.Name} Changing Format ({Math.Round(_totalTime.GetValue(TimeUnits.Minute))} min)";
            AddTransition<LineStateReviewNextSKUState>(line => IsTimeChangingSKUAchieved());

        }



        public override void Run(DateTime currentdate)
        {
            // ✅ Acumula tiempo de cambio de formato
            _currentTime += Context.OneSecond;
        }
        bool IsTimeChangingSKUAchieved()
        {
            StateLabel = $"{_line.Name} Changing Format ({Math.Round(PendingTime.GetValue(TimeUnits.Minute))} min)";
            if (PendingTime <= _line.ZeroTime)

            {
                return true;
            }
            return false;
        }
    }
    public class LineStateChangeFormatAfterEmptyTankState : LineOutletState
    {

        Amount _currentTime = new Amount(0, TimeUnits.Second);

        readonly Amount _totalTime = new Amount(0, TimeUnits.Second);
        Amount PendingTime => _totalTime - _currentTime;
        public LineStateChangeFormatAfterEmptyTankState(ProcessLine line) : base(line)
        {
            _totalTime = line.TimeToChangeFormat; // ← Tiempo de cambio de formato desde la línea
            _line.SetPumpsFlowToZero();
            _line.SelectProductionRun();
            StateLabel = $"{line.Name} Changing Format ({Math.Round(_totalTime.GetValue(TimeUnits.Minute))} min)";
            AddTransition<LineStateReviewProducingState>(line => IsTimeChangingSKUAchieved());

        }

        public override void Run(DateTime currentdate)
        {
            // ✅ Acumula tiempo de cambio de formato
            _currentTime += Context.OneSecond;
        }
        bool IsTimeChangingSKUAchieved()
        {
            StateLabel = $"{_line.Name} Changing Format ({Math.Round(PendingTime.GetValue(TimeUnits.Minute))} min)";
            if (PendingTime <= _line.ZeroTime)

            {
                return true;
            }
            return false;
        }
    }




}
