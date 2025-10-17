namespace Simulator.Shared.NuevaSimlationconQwen.Equipments.Pumps
{



    public abstract class ManufactureFeeder : Equipment, IManufactureFeeder
    {
        public LinkedList<IEquipment> WaitingQueue => _waitingQueue;
        private readonly LinkedList<IEquipment> _waitingQueue = new();

        public Amount Flow { get; set; } = new Amount(0, MassFlowUnits.Kg_sg);
        public Amount ActualFlow { get; set; } = new Amount(0, MassFlowUnits.Kg_sg);

        // IsForWashout es definido por la subclase (no puede cambiar en runtime)
        public abstract bool IsForWashout { get; set; }

        // Mapeo directo a OcupiedByName de Equipment
        public IEquipment OcuppiedBy { get; set; } = null!;

        public bool StarvedByInletState { get; set; }

        // Disponibilidad: basada en tu modelo de estados (corazón del simulador)


        // === Gestión de cola de espera ===
        public void EnqueueWaitingEquipment(IEquipment equipment)
        {
            if (equipment == null) return;
            if (!_waitingQueue.Contains(equipment))
            {
                string legend = "is not available";
                if (this.OcuppiedBy != null)
                {
                    legend = $"is used by {this.OcuppiedBy.Name}";
                }
                equipment.StartCriticalReport(this, $"Starved {equipment.Name}", $"{this.Name} {legend}");
                _waitingQueue.AddLast(equipment);
            }
        }



        public int GetWaitingQueueLength() => _waitingQueue.Count;

        public void NotifyNextWaitingEquipment()
        {
            if (_waitingQueue.Count == 0) return;

            var next = _waitingQueue.First!.Value;
            _waitingQueue.RemoveFirst();
            next.OnFeederMayBeAvailable(this);
        }

        // Las subclases deben definir su lógica específica de "starved por tanque"
        public virtual bool IsAnyTankInletStarved()
        {
            return false;
        }
        public virtual bool IsAnyTankInletStarvedRealesed()
        {
            return false;
        }



    }

    public interface IManufactureFeeder : IEquipment
    {
        Amount Flow { get; set; }
        Amount ActualFlow { get; set; }
        bool IsForWashout { get; set; }
        bool IsAnyTankInletStarved();
        bool IsAnyTankInletStarvedRealesed();
        IEquipment OcuppiedBy { get; set; }
        void EnqueueWaitingEquipment(IEquipment equipment);

        int GetWaitingQueueLength();
        void NotifyNextWaitingEquipment();


        bool StarvedByInletState { get; set; }
        LinkedList<IEquipment> WaitingQueue { get; }
    }
}
