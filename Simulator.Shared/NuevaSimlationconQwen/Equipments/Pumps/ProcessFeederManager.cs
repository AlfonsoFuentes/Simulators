using Simulator.Shared.NuevaSimlationconQwen.States.PlannedDownTimes;

namespace Simulator.Shared.NuevaSimlationconQwen.Equipments.Pumps
{


    public class ProcessFeederManager
    {
        private readonly List<IManufactureFeeder> _allfeeders2;
        private readonly Queue<QueuedRequest> _waitingQueue = new();
        private readonly Dictionary<Equipment, List<IManufactureFeeder>> _assignments = new();

        public ProcessFeederManager(List<IManufactureFeeder> allfeeders)
        {
            _allfeeders2 = allfeeders ?? throw new ArgumentNullException(nameof(allfeeders));
        }

        // Tipos de solicitud
        private enum RequestType
        {
            FeederByMaterial,
            WashoutPump
        }

        // Registro de solicitud en cola
        private record QueuedRequest(Equipment Equipment, RequestType Type, Guid? MaterialId = null);

        public bool IsInQueue(Equipment equipment)
        {
            return _assignments.ContainsKey(equipment);
        }

        public IManufactureFeeder? TryAssignWash(Equipment equipment)
        {
            // ✅ Si YA tiene un recurso asignado, no hacer nada
            if (_assignments.ContainsKey(equipment))
            {
                return null; // Ya tiene recurso → consideramos que "pudo asignar"
            }
            var washoutPumps = _allfeeders2.FirstOrDefault(x => x.IsForWashout);
            if (washoutPumps == null) { return null; }

            if (!HasWashoutPumpConnected(equipment)) return null;
            return TryAssignFeeder(washoutPumps.Material!.Id, equipment);
        }

        public IManufactureFeeder? TryAssignFeeder(Guid? materialId, Equipment requestor)
        {
            if (materialId == null) return null;

            // Obtener lista de feeders asignados al equipo (crear si no existe)
            if (!_assignments.ContainsKey(requestor))
            {
                _assignments.TryAdd(requestor, new List<IManufactureFeeder>());
            }
            var assignedFeeders = _assignments[requestor];

            // Verificar si ya tiene un feeder para este material
            var existingFeeder = assignedFeeders.FirstOrDefault(f => f.EquipmentMaterials.Any(m => m.Material.Id == materialId));
            if (existingFeeder != null)
            {
                return existingFeeder; // Ya tiene → devolverlo
            }

            // Buscar un feeder libre para este material
            var candidateFeeder = GetFreeFeederWithMaterial(requestor, materialId.Value);
            if (candidateFeeder == null)
            {
                // ✅ Solo encola si NO está ya en la cola
                if (!IsEquipmentInQueue(requestor))
                {
                    EnqueueEquipment(requestor, RequestType.FeederByMaterial, materialId);
                }
                return null;
            }    // Asignar el feeder
            assignedFeeders.Add(candidateFeeder);
            candidateFeeder.OcupiedByName = requestor.Name;


            return candidateFeeder;

        }

        private bool IsEquipmentInQueue(Equipment equipment)
        {
            return _waitingQueue.Any(request => request.Equipment == equipment);
        }

        private IManufactureFeeder? GetFreeFeederWithMaterial(Equipment equipment, Guid materialId)
        {
            return equipment.InletEquipments
                .OfType<IManufactureFeeder>()
                .Where(f => f.EquipmentMaterials.Any(m => m.Material.Id == materialId))
                .FirstOrDefault(f => !IsFeederInUse(f) && !IsFeederInScheduledDowntime(f));
        }

        private ProcessPump? GetFreeWashoutPump(Equipment equipment)
        {
            return equipment.InletEquipments
                .OfType<ProcessPump>() // ← Solo washout pumps
                .FirstOrDefault(p => p.IsForWashout == true && !IsFeederInUse(p) && !IsFeederInScheduledDowntime(p));
        }

        private bool HasWashoutPumpConnected(Equipment equipment)
        {
            return equipment.InletEquipments.OfType<ProcessPump>().Any();
        }

        public bool AnyWashoutPumpAvailable()
        {
            var washoutpumps = _allfeeders2.Any(x => x.IsForWashout == true && !IsFeederInUse(x) && !IsFeederInScheduledDowntime(x));
            return washoutpumps;
        }

        private void AssignFeeder(Equipment equipment, IManufactureFeeder feeder)
        {
            if (!_assignments.ContainsKey(equipment))
            {
                _assignments[equipment] = new List<IManufactureFeeder>();
            }
            _assignments[equipment].Add(feeder);
        }

        public IManufactureFeeder AssignWashingPump(Equipment equipment)
        {
            var feeder = _allfeeders2.FirstOrDefault(x => x.IsForWashout && !IsFeederInUse(x) && !IsFeederInScheduledDowntime(x));
            if (feeder == null) { return null!; }
            AssignFeeder(equipment, feeder);
            return feeder;
        }

        public bool ReleaseEquipment(Equipment equipment)
        {
            if (_assignments.Remove(equipment, out var feeders))
            {
                // Opcional: resetear flujos o estado de los feeders
                foreach (var feeder in feeders)
                {
                    feeder.OcupiedByName = string.Empty;
                    feeder.ActualFlow = new Amount(0, MassFlowUnits.Kg_sg);
                }
                ProcessQueue(); // Procesar cola
                return true;
            }
            return false;
        }

        // Métodos alias (opcionales)
        public void ReleaseFeeder(Equipment equipment) => ReleaseEquipment(equipment);
        public void ReleaseWash(Equipment equipment) => ReleaseEquipment(equipment);

        public bool IsFeederInUse(IManufactureFeeder feeder)
        {
            return _assignments.Values.Any(feederList => feederList.Contains(feeder));
        }
        public bool IsFeederInUseByThisEquipment(IManufactureFeeder feeder, Equipment equipment)
        {
            if (_assignments.ContainsKey(equipment))
            {
                if (_assignments[equipment].Any(x => x == feeder))
                {
                    return true;
                }
            }
            return false;

        }
        private bool IsFeederInScheduledDowntime(IManufactureFeeder feeder)
        {
            return feeder.PlannedDownTimeState is ScheduledPlannedDownTimeState;
        }

        private string? GetMaterialName(Guid? materialId)
        {
            if (materialId == null) return null;

            foreach (var feeder in _allfeeders2)
            {
                var material = feeder.EquipmentMaterials.FirstOrDefault(m => m.Material.Id == materialId);
                if (material != null)
                {
                    return material.Material.CommonName;
                }
            }
            return null;
        }

        public void EnqueueWashoutRequest(Equipment equipment)
        {
            if (IsEquipmentInQueue(equipment)) return;
            EnqueueEquipment(equipment, RequestType.WashoutPump);
        }

        private void EnqueueEquipment(Equipment equipment, RequestType type, Guid? materialId = null!)
        {
            string reason = type switch
            {
                RequestType.FeederByMaterial => "Feeder not available for material",
                RequestType.WashoutPump => "Washout pump not available",
                _ => "Resource not available"
            };
            string materialname = GetMaterialName(materialId) ?? string.Empty;
            string description = type switch
            {
                RequestType.FeederByMaterial => $"Material ID: {materialname}",
                RequestType.WashoutPump => "Washout pump required but not available",
                _ => "Waiting for resource assignment"
            };

            equipment.StartCriticalReport(source: equipment, reason: reason, description: description);
            _waitingQueue.Enqueue(new QueuedRequest(equipment, type, materialId));
        }

        private void ProcessQueue()
        {
            var tempQueue = new Queue<QueuedRequest>();

            while (_waitingQueue.Count > 0)
            {
                var request = _waitingQueue.Dequeue();
                IManufactureFeeder? assignedResource = null;

                if (request.MaterialId.HasValue)
                {
                    assignedResource = GetFreeFeederWithMaterial(request.Equipment, request.MaterialId.Value);
                    if (assignedResource != null)
                    {
                        AssignFeeder(request.Equipment, assignedResource);

                        if (request.Equipment.ActiveDowntimeReportId.HasValue)
                        {
                            request.Equipment.EndCriticalReport();
                        }

                        request.Equipment.GetReleaseFromManager(assignedResource);
                    }
                }

                if (assignedResource == null)
                {
                    tempQueue.Enqueue(request);
                }
            }

            while (tempQueue.Count > 0)
            {
                _waitingQueue.Enqueue(tempQueue.Dequeue());
            }
        }
    }

    // Extensión útil para Dictionary



}
