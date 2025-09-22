using Simulator.Shared.NuevaSimlationconQwen.Equipments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulator.Shared.NuevaSimlationconQwen.Reports
{
    public class CriticalDowntimeReport
    {
        private readonly GeneralSimulation _simulation = null!;
        
        public Guid Id { get; } = Guid.NewGuid();
        public DateTime StartTime { get; }
        public DateTime? EndTime { get; private set; }
        public TimeSpan Duration => EndTime.HasValue ? EndTime.Value - StartTime : _simulation.CurrentDate - StartTime;

        public IEquipment Generator { get; } // ← Equipo que generó la parada (ej: línea)
        public IEquipment? Source { get; }   // ← Equipo que causó la parada (ej: tanque WIP, bomba, etc.)
        public string Reason { get; }
        public string Description { get; }

        public CriticalDowntimeReport(GeneralSimulation simulation,IEquipment generator, IEquipment? source, string reason, string description)
        {
            _simulation = simulation;
            StartTime = _simulation.CurrentDate;
            Generator = generator ?? throw new ArgumentNullException(nameof(generator));
            Source = source ?? throw new ArgumentNullException(nameof(source));
            Reason = reason ?? throw new ArgumentNullException(nameof(reason));
            Description = description ?? throw new ArgumentNullException(nameof(description));
        }

        public void End(DateTime end)
        {
            EndTime = end;
        }

        public bool IsCompleted => EndTime.HasValue;
    }
}
