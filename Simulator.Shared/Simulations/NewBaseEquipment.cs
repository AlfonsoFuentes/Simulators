using Simulator.Shared.Enums.HCEnums.Enums;
using Simulator.Shared.Models.HCs.BaseEquipments;
using Simulator.Shared.Models.HCs.EquipmentPlannedDownTimes;
using Simulator.Shared.Simulations.Materials;
using Simulator.Shared.Simulations.Pumps;

namespace Simulator.Shared.Simulations
{
    public abstract class NewBaseEquipment
    {
        public Guid CurrentEventId { get; set; }
        public override string ToString()
        {
            return Name;
        }
        protected Amount OneSecond = new(1, TimeUnits.Second);
       
       
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
        public void StartEquipmentEvent(string description)
        {
            // Crear nuevo evento
            var eventArgs = new NewBaseEquipmentEventArgs(this, description);

            // Asignar este evento como el evento actual
            CurrentEventId = eventArgs.EventId;

            // Publicar el evento a través de la simulación
            Simulation?.PublishEquipmentEvent(eventArgs);

           
        }
        public void CloseCurrentEvent()
        {
            if (CurrentEventId != Guid.Empty && Simulation != null)
            {
                var currentEvent = Simulation.GetEquipmentEventById(CurrentEventId);
                if (currentEvent != null && currentEvent.EventStatus == EventStatus.Open)
                {
                    currentEvent.CloseEvent();
                    Simulation?.UpdateEquipmentEvent(currentEvent);
                    CurrentEventId = Guid.Empty; // Limpiar referencia
                }
            }
        }
    }
    public class NewBaseEquipmentEventArgs : EventArgs
    {
        public NewBaseEquipment Equipment { get; set; } = null!;
        public Guid EventId { get; set; } = Guid.NewGuid(); // ID único para cada evento
        public ProccesEquipmentType EquipmentType => Equipment?.EquipmentType ?? ProccesEquipmentType.None;
        public string EquipmentName => Equipment?.Name ?? string.Empty;
        public string Description { get; set; } = string.Empty;

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public TimeSpan Duration => EndDate == DateTime.MinValue ? TimeSpan.Zero : EndDate - StartDate;
        public EventStatus EventStatus { get; set; } = EventStatus.Open;

        // Constructor para eventos que inician
        public NewBaseEquipmentEventArgs(NewBaseEquipment equipment, string description)
        {
            Equipment = equipment;
            Description = description;
            StartDate = equipment?.Simulation?.CurrentDate ?? DateTime.Now;
            EventStatus = EventStatus.Open;
        }

        // Constructor vacío para serialización
        public NewBaseEquipmentEventArgs() { }

        // Método para cerrar el evento
        public void CloseEvent()
        {
            EndDate = Equipment?.Simulation?.CurrentDate ?? DateTime.Now;
            EventStatus = EventStatus.Closed;
        }
    }
    public enum EventStatus
    {
        Open,
        Closed
    }
}
