using Simulator.Shared.Enums.HCEnums.Enums;
using Simulator.Shared.Models.HCs.EquipmentPlannedDownTimes;

namespace Simulator.Shared.Simulations
{
    //public interface IEquipmentCalculation
    //{
    //    void Init();
    //    void Calculate(DateTime currentdate);
    //    IState InletState { get; set; }
    //    IState OutletState { get; set; }
    //}
    //public abstract class NewBaseEquipment : IEquipmentCalculation
    //{
    //    public Guid CurrentEventId { get; set; }
    //    public override string ToString()
    //    {
    //        return Name;
    //    }
    //    protected Amount OneSecond = new(1, TimeUnits.Second);
    //    public IState InletState { get; set; } = null!;
    //    public IState OutletState { get; set; } = null!;

    //    public virtual Guid Id { get; }
    //    public virtual string Name { get; } = string.Empty;
    //    protected List<EquipmentPlannedDownTimeDTO> PlannedDownTimes { get; set; } = new();

    //    public string OcupiedByName => OcupiedBy == null ? "None" : OcupiedBy.Name;
    //    public List<NewBaseEquipment> ProcessInletEquipments { get; set; } = new List<NewBaseEquipment>();
    //    public List<NewBaseEquipment> ProcessOutletEquipments { get; set; } = new List<NewBaseEquipment>();

    //    public List<BaseLine> OutletLines => ConnectedOutletEquipments.OfType<BaseLine>().ToList();
    //    public List<BasePump> OutletPumps => ConnectedOutletEquipments.OfType<BasePump>().ToList();
    //    public List<BaseTank> OutletTanks => ConnectedOutletEquipments.OfType<BaseTank>().ToList();
    //    public List<BaseMixer> OutletMixers => ConnectedOutletEquipments.OfType<BaseMixer>().ToList();


    //    public List<BaseLine> AvailableOutletLines => OutletLines.Where(x => x.OutletState is EquipmentAvailableState).ToList();
    //    public List<BasePump> AvailableOutletPumps => OutletPumps.Where(x => x.OutletState is EquipmentAvailableState).ToList();
    //    public List<BaseTank> AvailableOutletTanks => OutletTanks.Where(x => x.OutletState is EquipmentAvailableState).ToList();
    //    public List<BaseMixer> AvailableOutletMixers => OutletMixers.Where(x => x.OutletState is EquipmentAvailableState).ToList();


    //    public List<BasePump> InletPumps => ConnectedInletEquipments.OfType<BasePump>().ToList();
    //    public List<BaseTank> InletTanks => ConnectedInletEquipments.OfType<BaseTank>().ToList();
    //    public List<BaseMixer> InletMixers => ConnectedInletEquipments.OfType<BaseMixer>().ToList();
    //    public List<BaseOperator> InletOperators => ConnectedInletEquipments.OfType<BaseOperator>().ToList();


    //    public List<BasePump> AvailableInletPumps => InletPumps.Where(x => x.InletState is EquipmentAvailableState).ToList();
    //    public List<BaseTank> AvailableInletTanks => InletTanks.Where(x => x.InletState is EquipmentAvailableState).ToList();
    //    public List<BaseMixer> AvailableInletMixers => InletMixers.Where(x => x.InletState is EquipmentAvailableState).ToList();
    //    public List<BaseOperator> AvailableInletOperators => InletOperators.Where(x => x.InletState is EquipmentAvailableState).ToList();

    //    public NewBaseEquipment OcupiedBy => ProcessOutletEquipments.Count == 0 ? null! : ProcessOutletEquipments.First();

    //    public bool IsForWashing { get; set; }
    //    public ProccesEquipmentType EquipmentType { get; set; } = ProccesEquipmentType.None;
    //    public List<NewBaseEquipment> ConnectedOutletEquipments { get; private set; } = new List<NewBaseEquipment>();
    //    public List<NewBaseEquipment> ConnectedInletEquipments { get; private set; } = new List<NewBaseEquipment>();

    //    Queue<NewBaseEquipment> queueOutlet { get; set; } = new Queue<NewBaseEquipment>();
    //    public List<MaterialSimulation> MaterialSimulations { get; private set; } = new();
    //    public void InitInletConnectedEquipment()
    //    {
    //        foreach (var row in ConnectedInletEquipments)
    //        {
    //            row.Init();
    //        }
    //    }

    //    public virtual void Init()
    //    {

    //    }

    //    public virtual void Calculate(DateTime currentdate)
    //    {
    //        OutletState?.Calculate();
    //        InletState?.Calculate();
    //    }


    //    public void CalculateAtachedInlets(DateTime date)
    //    {
    //        foreach (var row in ConnectedInletEquipments)
    //        {
    //            row.Calculate(date);
    //        }
    //    }


    //    public void AddConnectedInletEquipment(NewBaseEquipment item)
    //    {
    //        if (!ConnectedInletEquipments.Contains(item))
    //        {
    //            ConnectedInletEquipments.Add(item);
    //            if (!item.ConnectedOutletEquipments.Contains(this))
    //                item.ConnectedOutletEquipments.Add(this);
    //        }

    //    }
    //    public virtual void AddProcessInletEquipment(NewBaseEquipment item)
    //    {
    //        if (!ProcessInletEquipments.Contains(item))
    //        {
    //            ProcessInletEquipments.Add(item);
    //            if (!item.ProcessOutletEquipments.Contains(this))
    //                item.ProcessOutletEquipments.Add(this);
    //        }

    //    }
    //    public virtual void RemoveProcessInletEquipment(NewBaseEquipment item)
    //    {
    //        if (ProcessInletEquipments.Contains(item))
    //        {
    //            ProcessInletEquipments.Remove(item);

    //            if (item.ProcessOutletEquipments.Contains(this))
    //            {
    //                item.ProcessOutletEquipments.Remove(this);
    //            }
    //        }

    //    }
    //    public virtual void RemoveProcessOutletEquipment(NewBaseEquipment item)
    //    {
    //        if (ProcessOutletEquipments.Contains(item))
    //        {
    //            ProcessOutletEquipments.Remove(item);

    //            if (item.ProcessInletEquipments.Contains(this))
    //            {
    //                item.ProcessInletEquipments.Remove(this);
    //            }
    //        }

    //    }

    //    public List<NewBaseEquipment> GetEquipmentListInlet(MaterialSimulation material)
    //    {
    //        List<NewBaseEquipment> retorno = ConnectedInletEquipments.Where(x =>
    //       x.IsForWashing == false

    //       && x.MaterialSimulations.Any(x => x.Id == material.Id)).ToList();
    //        return retorno;
    //    }
    //    public NewBaseEquipment GetEquipmentAtInlet(MaterialSimulation material)
    //    {
    //        List<NewBaseEquipment> retorno = ConnectedInletEquipments.Where(x =>
    //       x.IsForWashing == false
    //       && x.MaterialSimulations.Any(x => x.Id == material.Id)).ToList();
    //        return retorno.FirstOrDefault()!;
    //    }
    //    public NewBaseEquipment GetEquipmentAtOutlet()
    //    {
    //        List<NewBaseEquipment> retorno = ConnectedOutletEquipments.Where(x =>
    //       x.IsForWashing == false).ToList();
    //        return retorno.FirstOrDefault()!;
    //    }

    //    public bool IsEquipmentHasQueue => queueOutlet.Count > 0;

    //    public NewBaseEquipment GetFirstQueue => IsEquipmentHasQueue ? queueOutlet.Peek() : null!;
    //    public void RemoveEquipmentFromQueue()
    //    {
    //        queueOutlet.Dequeue();
    //    }
    //    public void PutEquipmentInQueue(NewBaseEquipment item)
    //    {
    //        if (ConnectedOutletEquipments.Count > 0)
    //        {
    //            if (!queueOutlet.Contains(item))
    //            {
    //                queueOutlet.Enqueue(item);
    //            }

    //        }
    //    }
    //    public NewBaseEquipment AddProcessEquipmentInletOrPutQueue2(MaterialSimulation material)
    //    {
    //        var inletEquipments = GetEquipmentListInlet(material);

    //        if (!inletEquipments.Any())
    //            return null!;

    //        // Buscar equipos libres (sin conexiones de salida)
    //        var freeEquipments = inletEquipments
    //            .Where(e => e.ProcessOutletEquipments.Count == 0)
    //            .ToList();

    //        // Si hay equipos libres, asignar inmediatamente
    //        if (freeEquipments.Any())
    //        {
    //            var selectedEquipment = freeEquipments.First();
    //            AddProcessInletEquipment(selectedEquipment);
    //            return selectedEquipment;
    //        }

    //        // Si no hay equipos libres, encontrar el equipo con la cola más corta
    //        var equipmentWithQueues = inletEquipments
    //            .Where(e => e.ProcessOutletEquipments.Count > 0) // Equipos ocupados
    //            .OrderBy(e => e.queueOutlet.Count) // Ordenar por tamaño de cola (menor primero)
    //            .ToList();

    //        if (equipmentWithQueues.Any())
    //        {
    //            var selectedEquipment = equipmentWithQueues.First(); // Equipo con cola más corta

    //            // Verificar si ya estamos en la cola
    //            if (!IsAlreadyInQueue(selectedEquipment))
    //            {
    //                selectedEquipment.queueOutlet.Enqueue(this);
    //            }

    //            return selectedEquipment; // En cola, esperando turno
    //        }

    //        return null!; // No se pudo asignar
    //    }

    //    // Método auxiliar para verificar si ya estamos en la cola de un equipo
    //    private bool IsAlreadyInQueue(NewBaseEquipment equipment)
    //    {
    //        return equipment.queueOutlet.Contains(this);
    //    }
    //    public NewBaseEquipment AddProcessEquipmentInletOrPutQueue(MaterialSimulation material)
    //    {
    //        var inletEquipments = GetEquipmentListInlet(material);

    //        if (!inletEquipments.Any())
    //            return null!;

    //        // Buscar equipos libres (sin conexiones de salida)
    //        var freeEquipments = inletEquipments
    //            .Where(e => e.ProcessOutletEquipments.Count == 0)
    //            .ToList();

    //        // Si hay equipos libres, verificar primero si estamos al frente de alguna cola
    //        if (freeEquipments.Any())
    //        {
    //            // Verificar si estamos al frente de la cola de algún equipo libre
    //            foreach (var freeEquipment in freeEquipments)
    //            {
    //                if (IsAtFrontOfQueue(freeEquipment))
    //                {
    //                    // Estamos al frente, remover de la cola y asignar
    //                    freeEquipment.queueOutlet.Dequeue(); // Removernos de la cola
    //                    AddProcessInletEquipment(freeEquipment);
    //                    return freeEquipment;
    //                }
    //            }

    //            // No estamos al frente de ninguna cola, asignar al primer equipo libre
    //            var selectedEquipment = freeEquipments.First();
    //            AddProcessInletEquipment(selectedEquipment);
    //            return selectedEquipment;
    //        }

    //        // Si no hay equipos libres, encontrar el equipo con la cola más corta
    //        var equipmentOrderedByQueueLength = inletEquipments
    //            .OrderBy(e => e.queueOutlet.Count) // Ordenar por tamaño de cola (menor primero)
    //            .ToList();

    //        if (equipmentOrderedByQueueLength.Any())
    //        {
    //            var selectedEquipment = equipmentOrderedByQueueLength.First(); // Equipo con cola más corta

    //            // Verificar si ya estamos en la cola para evitar duplicados
    //            if (!IsAlreadyInQueue(selectedEquipment))
    //            {
    //                selectedEquipment.queueOutlet.Enqueue(this);
    //            }

    //            return selectedEquipment; // En cola, esperando turno
    //        }

    //        return null!; // No se pudo asignar
    //    }




    //    // Método auxiliar para verificar si estamos al frente de la cola
    //    private bool IsAtFrontOfQueue(NewBaseEquipment equipment)
    //    {
    //        // Verificar si hay elementos en la cola y si somos el primero
    //        if (equipment.queueOutlet.Count > 0)
    //        {
    //            var frontEquipment = equipment.queueOutlet.Peek();
    //            return frontEquipment?.Id == Id;
    //        }
    //        return false;
    //    }



    //    public BasePump SearchInletWashingEquipment()
    //    {
    //        if (ConnectedInletEquipments.FirstOrDefault(x => x.IsForWashing == true) is BasePump pump && pump is not null)
    //        {
    //            if (pump.ConnectedOutletEquipments.Count == pump.MaxNumberEquipmentToWash)
    //            {
    //                if (!pump.queueOutlet.Contains(this))
    //                {
    //                    pump.queueOutlet.Enqueue(this);
    //                }
    //                return null!;
    //            }
    //            else if (pump.ConnectedOutletEquipments.Count <= pump.MaxNumberEquipmentToWash - 1)
    //            {
    //                if (pump.queueOutlet.Count == 0)
    //                {
    //                    AddProcessInletEquipment(pump);
    //                    return pump;
    //                }
    //                if (pump.queueOutlet.Peek().Id != Id) return null!;


    //                pump.queueOutlet.Dequeue();
    //                AddProcessInletEquipment(pump);
    //                return pump;



    //            }


    //        }
    //        return null!;

    //    }


    //    public NewBaseEquipment GetInletAttachedEquipment()
    //    {
    //        return ConnectedInletEquipments.Where(x => x.IsForWashing == false).Count() == 1 ? ConnectedInletEquipments.Where(x => x.IsForWashing == false).First() : null!;
    //    }
    //    public NewBaseEquipment GetInletProcessEquipment()
    //    {
    //        return ConnectedInletEquipments.Count == 1 ? ConnectedInletEquipments.First() : null!;
    //    }


    //    public void AddMaterialSimulation(MaterialSimulation material)
    //    {
    //        if (!MaterialSimulations.Any(x => x.Id == material.Id))
    //        {
    //            MaterialSimulations.Add(material);
    //            if (!material.ProcessEquipments.Any(x => x.Id == Id))
    //            {
    //                material.AddProcessEquipment(this);
    //            }
    //        }
    //        CurrentMaterialSimulation = MaterialSimulations.Count == 1 ? MaterialSimulations.First() : null!;
    //    }


    //    public MaterialSimulation CurrentMaterialSimulation { get; private set; } = null!;

    //    public string MaterialName => CurrentMaterialSimulation == null ? string.Empty : CurrentMaterialSimulation.CommonName;

    //    public virtual void SetCurrentMaterialSimulation(MaterialSimulation material)
    //    {
    //        CurrentMaterialSimulation = material;
    //    }


    //    public void SetMaterialsOutlet()
    //    {
    //        foreach (var row in ConnectedOutletEquipments)
    //        {
    //            foreach (var item in MaterialSimulations)
    //            {
    //                row.AddMaterialSimulation(item);
    //            }


    //        }
    //    }

    //    public bool HasAnyInletEquipmentMaterial(MaterialSimulation material)
    //    {
    //        return ConnectedInletEquipments.Any(eq => eq.MaterialSimulations.Any(x => x.Id == material.Id));
    //    }
    //    public NewSimulation Simulation { get; set; } = null!;

    //    // Evento para que cada instancia pueda disparar eventos
    //    public void StartEquipmentEvent(string description)
    //    {
    //        // Crear nuevo evento
    //        var eventArgs = new NewBaseEquipmentEventArgs(this, description);

    //        // Asignar este evento como el evento actual
    //        CurrentEventId = eventArgs.EventId;

    //        // Publicar el evento a través de la simulación
    //        Simulation?.PublishEquipmentEvent(eventArgs);


    //    }
    //    public void CloseCurrentEvent()
    //    {
    //        if (CurrentEventId != Guid.Empty && Simulation != null)
    //        {
    //            var currentEvent = Simulation.GetEquipmentEventById(CurrentEventId);
    //            if (currentEvent != null && currentEvent.EventStatus == EventStatus.Open)
    //            {
    //                currentEvent.CloseEvent();
    //                Simulation?.UpdateEquipmentEvent(currentEvent);
    //                CurrentEventId = Guid.Empty; // Limpiar referencia
    //            }
    //        }
    //    }
    //}
    //public class NewBaseEquipmentEventArgs : EventArgs
    //{
    //    public NewBaseEquipment Equipment { get; set; } = null!;
    //    public Guid EventId { get; set; } = Guid.NewGuid(); // ID único para cada evento
    //    public ProccesEquipmentType EquipmentType => Equipment?.EquipmentType ?? ProccesEquipmentType.None;
    //    public string EquipmentName => Equipment?.Name ?? string.Empty;
    //    public string Description { get; set; } = string.Empty;

    //    public DateTime StartDate { get; set; }
    //    public DateTime EndDate { get; set; }
    //    public TimeSpan Duration => EndDate == DateTime.MinValue ? TimeSpan.Zero : EndDate - StartDate;
    //    public EventStatus EventStatus { get; set; } = EventStatus.Open;

    //    // Constructor para eventos que inician
    //    public NewBaseEquipmentEventArgs(NewBaseEquipment equipment, string description)
    //    {
    //        Equipment = equipment;
    //        Description = description;
    //        StartDate = equipment?.Simulation?.CurrentDate ?? DateTime.Now;
    //        EventStatus = EventStatus.Open;
    //    }

    //    // Constructor vacío para serialización
    //    public NewBaseEquipmentEventArgs() { }

    //    // Método para cerrar el evento
    //    public void CloseEvent()
    //    {
    //        EndDate = Equipment?.Simulation?.CurrentDate ?? DateTime.Now;
    //        EventStatus = EventStatus.Closed;
    //    }
    //}
    //public enum EventStatus
    //{
    //    Open,
    //    Closed
    //}
}
