namespace Simulator.Shared.Simulations.Lines.States
{
   
    public abstract class LineState
    {
        protected Amount OneSecond = new(1, TimeUnits.Second);
        public string StateLabel { get; private set; } = string.Empty;

        public void Calculate()
        {
            Run();
            CheckStatus();
            
        }
        public abstract void Run();
        protected abstract void CheckStatus();
        
        protected BaseLine Line { get; private set; }
        public LineState(BaseLine line, string stateLabel)
        {


            StateLabel = stateLabel;
            Line = line;
            Line.StorageDataToSimulation(stateLabel);
        }


    }

}
