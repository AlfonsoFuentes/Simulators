using Simulator.Shared.NuevaSimlationconQwen.Equipments;

namespace Simulator.Shared.NuevaSimlationconQwen.States.BaseClass
{
   
    public interface IEquipmentState
    {
       
        string StateLabel { get; set; }
        void CheckStatus(DateTime CurrentDate);
        void Run(DateTime CurrentDate);
        void Report(DateTime CurrentDate);
        void Calculate(DateTime currentdate);

    }
    public abstract class EquipmentState : IEquipmentState
    {
       
        public string StateLabel { get; set; } = string.Empty;
        public void Calculate(DateTime currentdate)
        {
            try
            {
                BeforeRun(currentdate);
                Run(currentdate);
                AfterRun(currentdate);

                BeforeCheckStatus(currentdate);
                CheckStatus(currentdate);
                AfterCheckStatus(currentdate);

                BeforeReport(currentdate);
                Report(currentdate);
                AfterReport(currentdate);
            }
            catch (Exception ex)
            {
                string msg = ex.Message;    
            }

            
        }
        public virtual void Run(DateTime currentdate) { }
        public virtual void CheckStatus(DateTime currentdate) { }
        public virtual void Report(DateTime currentdate) { }
        protected IEquipment Equipment { get; private set; }
        public EquipmentState(IEquipment equipment)
        {
            Equipment = equipment;
        }
        protected virtual void BeforeRun(DateTime currentdate) { }
        protected virtual void AfterRun(DateTime currentdate) { }
        protected virtual void BeforeCheckStatus(DateTime currentdate) { }
        protected virtual void AfterCheckStatus(DateTime currentdate) { }
        protected virtual void BeforeReport(DateTime currentdate) { }
        protected virtual void AfterReport(DateTime currentdate) { }
    }
}
