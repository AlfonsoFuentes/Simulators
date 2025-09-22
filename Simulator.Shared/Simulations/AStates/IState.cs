using Simulator.Shared.Simulations.Lines;

namespace Simulator.Shared.Simulations.States
{
    public interface IState
    {
        void CheckStatus();
        void Run();
        string StateLabel { get; set; }
        void Report();
        void Calculate();
    }
    public abstract class EquipmentState : IState
    {
        public string StateLabel { get; set; } = string.Empty;

        public void Calculate()
        {
            Run();
            CheckStatus();
            Report();

        }
        public virtual void Run() { }
        public virtual void CheckStatus() { }

        public virtual void Report(){}

        protected NewBaseEquipment Equipment { get; private set; }
        public EquipmentState(NewBaseEquipment equipment)
        {



            Equipment = equipment;

        }
    }
    public class EquipmentAvailableState : EquipmentState
    {
       
       
        public EquipmentAvailableState(NewBaseEquipment equipment):base(equipment)
        {
            StateLabel = "Available";
        }
        
    }
    public class EquipmentOcuppiedByState : EquipmentState
    {
      
        
        public EquipmentOcuppiedByState(NewBaseEquipment equipment, string occupedbyname):base(equipment)
        {
            StateLabel = $"Occupied by: {occupedbyname}";
           
        }
        
    }
    public class EquipmentPlannedDownTimeState : EquipmentState
    {
        
        Amount PlannedDownTime = null!;
        Amount Currentime = null!;
        Amount OneSecond = new Amount(1, TimeUnits.Second);
        public EquipmentPlannedDownTimeState(NewBaseEquipment equipment, Amount time):base(equipment)
        {
            StateLabel = $"Planned downtime";
       
            PlannedDownTime = time;
            Currentime = new Amount(0, TimeUnits.Second);
        }
        

        public override void CheckStatus()
        {
            if (Currentime >= PlannedDownTime)
            {
                Equipment.OutletState = new EquipmentAvailableState(Equipment);
            }

        }

        
        public override void Run()
        {
            Currentime += OneSecond;
        }
    }
    public class EquipmentWashingState : EquipmentState
    {

        Amount PlannedDownTime = null!;
        Amount Currentime = null!;
        Amount OneSecond = new Amount(1, TimeUnits.Second);
        public EquipmentWashingState(NewBaseEquipment equipment, Amount time) : base(equipment)
        {
            StateLabel = $"Washing";

            PlannedDownTime = time;
            Currentime = new Amount(0, TimeUnits.Second);
        }


        public override void CheckStatus()
        {
            if (Currentime >= PlannedDownTime)
            {
                Equipment.OutletState = new EquipmentAvailableState(Equipment);
            }

        }


        public override void Run()
        {
            Currentime += OneSecond;
        }
    }

}
