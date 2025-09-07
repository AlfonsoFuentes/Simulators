using Simulator.Shared.Enums.HCEnums.Enums;
using Simulator.Shared.Models.HCs.BaseEquipments;
using Simulator.Shared.Models.HCs.EquipmentPlannedDownTimes;
using Simulator.Shared.Simulations.Materials;
using Simulator.Shared.Simulations.Pumps;

namespace Simulator.Shared.Simulations
{
    public abstract class NewBaseEquipment
    {
        public override string ToString()
        {
            return Name;
        }
        protected Amount OneSecond = new(1, TimeUnits.Second);
        public string CurrentEventId { get; set; } = null!; // Para eventos de parada

        public virtual Guid Id { get; }
        public virtual string Name { get; } = string.Empty;
        protected List<EquipmentPlannedDownTimeDTO> PlannedDownTimes { get; set; } = new();

        public string OcupiedByName => OcupiedBy == null ? "None" : OcupiedBy.Name;
        public List<NewBaseEquipment> ProcessInletEquipments { get; set; } = new List<NewBaseEquipment>();
        public List<NewBaseEquipment> ProcessOutletEquipments { get; set; } = new List<NewBaseEquipment>();
        public NewBaseEquipment OcupiedBy => ProcessOutletEquipments.Count == 0 ? null! : ProcessOutletEquipments.First();

        public bool IsForWashing { get; set; }
        public ProccesEquipmentType EquipmentType { get; set; } = ProccesEquipmentType.None;
        public List<NewBaseEquipment> ConnectedOutletEquipments { get; private set; } = new List<NewBaseEquipment>();
        public List<NewBaseEquipment> ConnectedInletEquipments { get; private set; } = new List<NewBaseEquipment>();

        Queue<NewBaseEquipment> queueOutlet { get; set; } = new Queue<NewBaseEquipment>();
        public List<MaterialSimulation> MaterialSimulations { get; private set; } = new();
        public void InitInletConnectedEquipment()
        {
            foreach (var row in ConnectedInletEquipments)
            {
                row.Init();
            }
        }

        public virtual void Init()
        {

        }

        public abstract void Calculate(DateTime currentdate);


        public void CalculateAtachedInlets(DateTime date)
        {
            foreach (var row in ConnectedInletEquipments)
            {
                row.Calculate(date);
            }
        }
        // Referencia a la simulación para obtener la fecha actual
       






        public void AddConnectedInletEquipment(NewBaseEquipment item)
        {
            if (!ConnectedInletEquipments.Contains(item))
            {
                ConnectedInletEquipments.Add(item);
                if (!item.ConnectedOutletEquipments.Contains(this))
                    item.ConnectedOutletEquipments.Add(this);
            }

        }
        public virtual void AddProcessInletEquipment(NewBaseEquipment item)
        {
            if (!ProcessInletEquipments.Contains(item))
            {
                ProcessInletEquipments.Add(item);
                if (!item.ProcessOutletEquipments.Contains(this))
                    item.ProcessOutletEquipments.Add(this);
            }

        }
        public virtual void RemoveProcessInletEquipment(NewBaseEquipment item)
        {
            if (ProcessInletEquipments.Contains(item))
            {
                ProcessInletEquipments.Remove(item);

                if (item.ProcessOutletEquipments.Contains(this))
                {
                    item.ProcessOutletEquipments.Remove(this);
                }
            }

        }
        public virtual void RemoveProcessOutletEquipment(NewBaseEquipment item)
        {
            if (ProcessOutletEquipments.Contains(item))
            {
                ProcessOutletEquipments.Remove(item);

                if (item.ProcessInletEquipments.Contains(this))
                {
                    item.ProcessInletEquipments.Remove(this);
                }
            }

        }

        public List<NewBaseEquipment> GetEquipmentListInlet(MaterialSimulation material)
        {
            List<NewBaseEquipment> retorno = ConnectedInletEquipments.Where(x =>
           x.IsForWashing == false

           && x.MaterialSimulations.Any(x => x.Id == material.Id)).ToList();
            return retorno;
        }
        public NewBaseEquipment GetEquipmentAtInlet(MaterialSimulation material)
        {
            List<NewBaseEquipment> retorno = ConnectedInletEquipments.Where(x =>
           x.IsForWashing == false
           && x.MaterialSimulations.Any(x => x.Id == material.Id)).ToList();
            return retorno.FirstOrDefault()!;
        }
        public NewBaseEquipment GetEquipmentAtOutlet()
        {
            List<NewBaseEquipment> retorno = ConnectedOutletEquipments.Where(x =>
           x.IsForWashing == false).ToList();
            return retorno.FirstOrDefault()!;
        }

        public bool IsEquipmentHasQueue => queueOutlet.Count > 0;

        public NewBaseEquipment GetFirstQueue => IsEquipmentHasQueue ? queueOutlet.Peek() : null!;
        public void RemoveEquipmentFromQueue()
        {
            queueOutlet.Dequeue();
        }
        public void PutEquipmentInQueue(NewBaseEquipment item)
        {
            if (ConnectedOutletEquipments.Count > 0)
            {
                if (!queueOutlet.Contains(item))
                {
                    queueOutlet.Enqueue(item);
                }

            }
        }
        public NewBaseEquipment AddProcessEquipmentInletOrPutQueue(MaterialSimulation material)
        {
            var equipmenlist = GetEquipmentListInlet(material);
            if (material.Id.ToString() == "16a43a89-0c98-4173-97fd-f3db45d73865")
            {

            }
            NewBaseEquipment retorno = null!;
            if (equipmenlist.Count > 0)
            {
                if (equipmenlist.All(x => x.ProcessOutletEquipments.Count > 0))
                {
                    retorno = equipmenlist.FirstOrDefault()!;
                }
                else
                {
                    retorno = equipmenlist.FirstOrDefault(x => x.ProcessOutletEquipments.Count == 0)!;
                }

            }

            if (retorno == null) return retorno!;

            if (retorno.ProcessOutletEquipments.Count > 0)
            {
                if (!retorno.queueOutlet.Contains(this))
                {
                    retorno.queueOutlet.Enqueue(this);
                }
                return null!;
            }
            else if (retorno.queueOutlet.Count == 0)
            {
                AddProcessInletEquipment(retorno);
                return retorno;

            }
            else if (retorno.queueOutlet.Peek().Id != Id) return null!;

            retorno.queueOutlet.Dequeue();
            AddProcessInletEquipment(retorno);
            return retorno;

        }


        public BasePump SearchInletWashingEquipment()
        {
            if (ConnectedInletEquipments.FirstOrDefault(x => x.IsForWashing == true) is BasePump pump && pump is not null)
            {
                if (pump.ConnectedOutletEquipments.Count == pump.MaxNumberEquipmentToWash)
                {
                    if (!pump.queueOutlet.Contains(this))
                    {
                        pump.queueOutlet.Enqueue(this);
                    }
                    return null!;
                }
                else if (pump.ConnectedOutletEquipments.Count <= pump.MaxNumberEquipmentToWash - 1)
                {
                    if (pump.queueOutlet.Count == 0)
                    {
                        AddProcessInletEquipment(pump);
                        return pump;
                    }
                    if (pump.queueOutlet.Peek().Id != Id) return null!;


                    pump.queueOutlet.Dequeue();
                    AddProcessInletEquipment(pump);
                    return pump;



                }


            }
            return null!;

        }


        public NewBaseEquipment GetInletAttachedEquipment()
        {
            return ConnectedInletEquipments.Where(x => x.IsForWashing == false).Count() == 1 ? ConnectedInletEquipments.Where(x => x.IsForWashing == false).First() : null!;
        }
        public NewBaseEquipment GetInletProcessEquipment()
        {
            return ConnectedInletEquipments.Count == 1 ? ConnectedInletEquipments.First() : null!;
        }


        public void AddMaterialSimulation(MaterialSimulation material)
        {
            if (!MaterialSimulations.Any(x => x.Id == material.Id))
            {
                MaterialSimulations.Add(material);
                if (!material.ProcessEquipments.Any(x => x.Id == Id))
                {
                    material.AddProcessEquipment(this);
                }
            }
            CurrentMaterialSimulation = MaterialSimulations.Count == 1 ? MaterialSimulations.First() : null!;
        }


        public MaterialSimulation CurrentMaterialSimulation { get; private set; } = null!;

        public string MaterialName => CurrentMaterialSimulation == null ? string.Empty : CurrentMaterialSimulation.CommonName;

        public virtual void SetCurrentMaterialSimulation(MaterialSimulation material)
        {
            CurrentMaterialSimulation = material;
        }


        public void SetMaterialsOutlet()
        {
            foreach (var row in ConnectedOutletEquipments)
            {
                foreach (var item in MaterialSimulations)
                {
                    row.AddMaterialSimulation(item);
                }

             
            }
        }

        public bool HasAnyInletEquipmentMaterial(MaterialSimulation material)
        {
            return ConnectedInletEquipments.Any(eq => eq.MaterialSimulations.Any(x => x.Id == material.Id));
        }
        public NewSimulation Simulation { get; set; } = null!;

        // Evento para que cada instancia pueda disparar eventos
        public event EventHandler<NewBaseEquipmentEventArgs> EquipmentEvent = null!;

        // Método protegido para que las clases derivadas puedan disparar eventos
        protected virtual void OnEquipmentEvent(NewBaseEquipmentEventArgs e)
        {
            EquipmentEvent?.Invoke(this, e);
        }

        // Método para INICIAR un evento con identificación única
        public string StartEquipmentEvent(
            string eventType,  // Tipo de evento: "LineStop", "MixerBlock", etc.
            string reason,     // Razón del evento
            string description,
            string details = "",
            string severity = "Info")
        {
            // Generar ID único para este evento específico
            var eventId = $"{this.Id}_{eventType}_{Guid.NewGuid():N}";

            var eventArgs = new NewBaseEquipmentEventArgs
            {
                Equipment = this,
                EventId = eventId,
                EventName = eventType,
                Description = description,
                Details = $"{reason}: {details}",
                Timestamp = Simulation?.CurrentDate ?? DateTime.Now,
                EventType = "Started",
                Severity = severity,
                Reason = reason
            };

            OnEquipmentEvent(eventArgs);
            return eventId; // Devolver el ID para usarlo al finalizar
        }

        // Método para FINALIZAR un evento específico usando su ID
        public void EndEquipmentEvent(
            string eventId,
            string endReason = "",
            string additionalDetails = "",
            string severity = "Info")
        {
            var eventArgs = new NewBaseEquipmentEventArgs
            {
                Equipment = this,
                EventId = eventId,
                EventName = "Event Completion",
                Description = string.IsNullOrEmpty(endReason) ? "Event completed" : endReason,
                Details = additionalDetails,
                Timestamp = Simulation?.CurrentDate ?? DateTime.Now,
                EventType = "Ended",
                Severity = severity
            };

            OnEquipmentEvent(eventArgs);
        }

        // Método para reportar eventos puntuales
        public void ReportEquipmentEvent(
            string eventType,
            string reason,
            string description,
            string details = "",
            TimeSpan duration = default,
            string severity = "Info")
        {
            var eventId = $"{this.Id}_{eventType}_{Guid.NewGuid():N}";

            var eventArgs = new NewBaseEquipmentEventArgs
            {
                Equipment = this,
                EventId = eventId,
                EventName = eventType,
                Description = description,
                Details = $"{reason}: {details}",
                Timestamp = Simulation?.CurrentDate ?? DateTime.Now,
                Duration = duration,
                EventType = "Instant",
                Severity = severity,
                Reason = reason
            };

            OnEquipmentEvent(eventArgs);
        }
    }
    public class NewBaseEquipmentEventArgs : EventArgs
    {
        public NewBaseEquipment Equipment { get; set; } = null!;
        public string EventId { get; set; } = string.Empty; // ID único para cada evento
        public string EventName { get; set; } = string.Empty; // Tipo de evento
        public string Description { get; set; } = string.Empty;
        public string Details { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty; // Razón específica del evento
        public DateTime Timestamp { get; set; }
        public TimeSpan Duration { get; set; } = TimeSpan.Zero;
        public string EventType { get; set; } = "Instant"; // Started, Ended, Instant
        public string Severity { get; set; } = "Info"; // Info, Warning, Error, Critical

        // PROPIEDAD QUE FALTABA: Clave única para correlacionar eventos de inicio y fin
        public string EventKey => string.IsNullOrEmpty(EventId) ? $"{Equipment?.Id}_{EventName}_{Timestamp.Ticks}" : EventId;
    }

}
