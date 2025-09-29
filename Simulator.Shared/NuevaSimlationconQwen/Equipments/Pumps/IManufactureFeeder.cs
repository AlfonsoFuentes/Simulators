namespace Simulator.Shared.NuevaSimlationconQwen.Equipments.Pumps
{
    // 👇 NUEVA CLASE: ManufactureFeeder.cs
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public abstract class ManufactureFeeder : Equipment, IManufactureFeeder
    {
        private readonly LinkedList<IEquipment> _waitingQueue = new();

        public Amount Flow { get; set; } = new Amount(0, MassFlowUnits.Kg_sg);
        public Amount ActualFlow { get; set; } = new Amount(0, MassFlowUnits.Kg_sg);

        // IsForWashout es definido por la subclase (no puede cambiar en runtime)
        public abstract bool IsForWashout { get; set; }

        // Mapeo directo a OcupiedByName de Equipment
        public string OcuppiedBy
        {
            get => OcupiedByName;
            set => OcupiedByName = value;
        }

        // Disponibilidad: basada en tu modelo de estados (corazón del simulador)
        public bool IsAvailableForAssignment()
        {
            return !(OutletState is IFeederStarved) && !(OutletState is IFeederInUse);
        }

        // === Gestión de cola de espera ===
        public void EnqueueWaitingEquipment(IEquipment equipment)
        {
            if (equipment == null) return;
            if (!_waitingQueue.Contains(equipment))
            {
                equipment.StartCriticalReport(this, $"Starved {equipment.Name}", $"{this.Name} is used by {this.OcupiedByName}");
                _waitingQueue.AddLast(equipment);
            }
        }

        public void RemoveWaitingEquipment(IEquipment equipment)
        {
            if (equipment == null) return;
            var node = _waitingQueue.First;
            while (node != null)
            {
                if (node.Value == equipment)
                {
                    _waitingQueue.Remove(node);
                    break;
                }
                node = node.Next;
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
        public abstract bool IsAnyTankInletStarved();
    }

    public interface IManufactureFeeder : IEquipment
    {
        Amount Flow { get; set; }
        Amount ActualFlow { get; set; }
        bool IsForWashout { get; set; }

        bool IsAnyTankInletStarved();

        // 👇 ELIMINAR: bool IsInUse();
        // 👇 AGREGAR: disponibilidad basada en estado
        bool IsAvailableForAssignment();

        string OcuppiedBy { get; set; }

        // 👇 GESTIÓN DE COLA (agregar estos 4)
        void EnqueueWaitingEquipment(IEquipment equipment);
        void RemoveWaitingEquipment(IEquipment equipment);
        int GetWaitingQueueLength();
        void NotifyNextWaitingEquipment();
    }
}
