using Simulator.Shared.Enums.HCEnums.Enums;
using Simulator.Shared.Models.HCs.BaseEquipments;
using Simulator.Shared.Models.HCs.ContinuousSystems;
using Simulator.Shared.Models.HCs.Lines;
using Simulator.Shared.Models.HCs.MaterialEquipments;
using Simulator.Shared.Models.HCs.Materials;
using Simulator.Shared.Models.HCs.Mixers;
using Simulator.Shared.Models.HCs.Operators;
using Simulator.Shared.Models.HCs.Pumps;
using Simulator.Shared.Models.HCs.SimulationPlanneds;
using Simulator.Shared.Models.HCs.StreamJoiners;
using Simulator.Shared.Models.HCs.Tanks;
using Simulator.Shared.Models.HCs.Washouts;
using Simulator.Shared.NuevaSimlationconQwen.Equipments;
using Simulator.Shared.NuevaSimlationconQwen.Equipments.Lines;
using Simulator.Shared.NuevaSimlationconQwen.Equipments.Mixers;
using Simulator.Shared.NuevaSimlationconQwen.Equipments.Operators;
using Simulator.Shared.NuevaSimlationconQwen.Equipments.Pumps;
using Simulator.Shared.NuevaSimlationconQwen.Equipments.Skids;
using Simulator.Shared.NuevaSimlationconQwen.Equipments.Tanks;
using Simulator.Shared.NuevaSimlationconQwen.ManufacturingOrders;
using Simulator.Shared.NuevaSimlationconQwen.Materials;
using Simulator.Shared.NuevaSimlationconQwen.Reports;
using Simulator.Shared.NuevaSimlationconQwen.States.PlannedDownTimes;
using Simulator.Shared.NuevaSimlationconQwen.StreanJoiners;
using Simulator.Shared.Simulations;

namespace Simulator.Shared.NuevaSimlationconQwen
{

    public class GeneralSimulation
    {

        public Guid MainProcessId { get; private set; }
        public CriticalDowntimeReportManager CriticalDowntimeReportManager { get; private set; } = null!;

        public ManufacturingSystemAnalizer ManufacturingSystemAnalizer { get; set; } = null!;
        public GeneralSimulation()
        {
            _messageService = new();

            ManufacturingSystemAnalizer = new ManufacturingSystemAnalizer(this);
        }
        private readonly SimulationMessageService _messageService;

        public IReadOnlyList<SimulationMessage> GetConfigurationWarnings()
        {
            return _messageService.GetMessages()
                                  .Where(m => m.Level == "Warning")
                                  .ToList();
        }
        public DateTime InitDate => Planned == null ? DateTime.Now : Planned.InitDate!.Value;
        public DateTime EndDate => Planned == null ? DateTime.Now : Planned.EndDate!.Value;
        public Amount TotalSimulacionTime { get; private set; } = new Amount(TimeUnits.Hour);
        Amount OneSecond = new(1, TimeUnits.Second);
        public bool IsSimulationRunning { get; set; } = false;
        public bool IsSimulationPaused { get; set; } = false;
        public bool StopSimulationRequested { get; set; } = false;
        public bool OnLiveReport { get; set; } = true;
        public List<IEquipment> Equipments { get; set; } = new();
        public List<IMaterial> Materials { get; set; } = new();
        List<WashoutTime> WashoutTimes { get; set; } = new();

        NewSimulationDTO NewSimulationDTO = null!;
        public SimulationPlannedDTO Planned { get; set; } = null!;
        //public List<IWashoutPump> WashoutPumps => Equipments.OfType<IWashoutPump>().ToList();
        public void ReadSimulationDataFromDTO(NewSimulationDTO NewSimulationDTO)
        {
            this.NewSimulationDTO = NewSimulationDTO;
            MainProcessId = NewSimulationDTO.Id;
            ReadMaterials(NewSimulationDTO.Materials);
            ReadPumps(NewSimulationDTO.Pumps);
            ReadTanks(NewSimulationDTO.Tanks);
            ReadMixers(NewSimulationDTO.Mixers);
            ReadSkids(NewSimulationDTO.Skids);
            ReadOperators(NewSimulationDTO.Operators);
            ReadLines(NewSimulationDTO.Lines);
            ReadStreamJoiners(NewSimulationDTO.StreamJoiners);
            ConnectEquipments(NewSimulationDTO);
            MapMaterialEquipments(NewSimulationDTO.MaterialEquipments);
            ReadPlannedDownTimes(NewSimulationDTO.AllEquipments);
            ReadWashoutTimes(NewSimulationDTO.WashouTimes);
            ManufacturingSystemAnalizer.Analyze(NewSimulationDTO.MaterialEquipments);
        }

        void ReadWashoutTimes(List<WashoutDTO> washouttimes)
        {
            washouttimes.ForEach(x => WashoutTimes.Add(
                new()
                {
                    LineWashoutTime = x.LineWashoutTime,
                    MixerWashoutTime = x.MixerWashoutTime,
                    ProductCategoryCurrent = x.ProductCategoryCurrent,
                    ProductCategoryNext = x.ProductCategoryNext,
                }

                ));
            Equipments.ForEach(x => x.WashoutTimes = WashoutTimes);
        }
        void ReadLines(List<LineDTO> lines)
        {
            foreach (var lineDto in lines)
            {
                var line = new ProcessLine()
                {

                    Id = lineDto.Id,
                    Name = lineDto.Name,
                    EquipmentType = ProccesEquipmentType.Line


                };
                Equipments.Add(line);
            }
        }
        void ReadStreamJoiners(List<StreamJoinerDTO> streamjoiners)
        {
            foreach (var streamjoinerdto in streamjoiners)
            {
                var streamjoiner = new ProcessStreamJoiner()
                {

                    Id = streamjoinerdto.Id,
                    Name = streamjoinerdto.Name,
                    EquipmentType = ProccesEquipmentType.StreamJoiner


                };
                Equipments.Add(streamjoiner);
            }
        }

        void ReadPumps(List<PumpDTO> pumps)
        {
            foreach (var pumpDTO in pumps)
            {
                var pump = new ProcessPump()
                {
                    Id = pumpDTO.Id,
                    Name = pumpDTO.Name,
                    EquipmentType = ProccesEquipmentType.Pump,
                    Flow = pumpDTO.Flow,
                    IsForWashout = pumpDTO.IsForWashing,
                };

                Equipments.Add(pump);



            }

        }
        void ConnectEquipments(NewSimulationDTO NewSimulationDTO)
        {
            var connectors = NewSimulationDTO.Connectors;
            var pumps = NewSimulationDTO.Pumps;


            foreach (var item in connectors)
            {
                if (item.FromId == Guid.Empty || item.ToId == Guid.Empty)
                {
                    continue;
                }

                var fromEquipment = Equipments.FirstOrDefault(x => x.Id == item.FromId);
                var toEquipment = Equipments.FirstOrDefault(x => x.Id == item.ToId);




                // Conectar si ambos equipos existen (o se crearon)
                if (fromEquipment != null && toEquipment != null)
                {
                    fromEquipment.AddOutletEquipment(toEquipment);
                }
                else
                {
                    // Registrar advertencia si no se pudo conectar
                    _messageService.AddWarning($"Cannot connect equipment: FromId={item.FromId}, ToId={item.ToId}. One or both are missing and no pump found.", "GeneralSimulation");
                }
            }


        }
        void MapMaterialEquipments(List<MaterialEquipmentRecord> processEquipmentMaterials)
        {
            foreach (var item in processEquipmentMaterials)
            {
                if (item.MaterialId == Guid.Empty || item.EquipmentId == Guid.Empty)
                {
                    continue;
                }
                var equipment = Equipments.FirstOrDefault(x => x.Id == item.EquipmentId);
                var material = Materials.FirstOrDefault(x => x.Id == item.MaterialId);

                if (material != null && equipment != null)
                {
                    equipment.AddMaterial(material);
                    if (equipment is ProcessBaseTankForRawMaterial tank)
                    {
                        tank.SetMaterialsAtOutlet(material);
                    }
                    if (equipment is ProcessMixer mixer)
                    {
                        mixer.SetMaterialsAtOutlet(material);
                        //if (material is RecipedMaterial recipedMaterial)
                        //{
                        //    var capacity = item.Capacity;
                        //    recipedMaterial.SetBatchSize(capacity);
                        //}
                    }

                }



            }
        }
        public void SetPlanned(SimulationPlannedDTO planned)
        {
            Planned = new();
            Planned = planned;
            CurrentDate = InitDate;
            ReadPlannedLines(NewSimulationDTO, planned);
            ReadOperatorsForMixers(planned);
        }
        void ReadOperatorsForMixers(SimulationPlannedDTO planned)
        {
            var operatorsforMixers = Equipments.OfType<ProcessMixer>().SelectMany(x => x.InletEquipments.OfType<ProcessOperator>()).ToList();
            foreach (var operators in operatorsforMixers)
            {
                operators.OperatorHasNotRestrictionToInitBatch = planned.OperatorHasNotRestrictionToInitBatch;
                operators.MaxRestrictionTime = planned.MaxRestrictionTime;
            }




        }
        void ReadPlannedLines(NewSimulationDTO simulationDto, SimulationPlannedDTO planned)
        {
            var lineFactory = new ProcessLineFactory(_messageService);

            foreach (var linePlanned in planned.PlannedLines)
            {
                // Validar duplicados
                var existingLine = Equipments.OfType<ProcessLine>().FirstOrDefault(e => e.Id == linePlanned.LineId);
                if (existingLine == null)
                {
                    AddWarningMessageServices($"Line with ID {linePlanned.LineId} not found in simulation data.");

                    continue;
                }

                var lineDto = simulationDto.Lines.FirstOrDefault(x => x.Id == linePlanned.LineId);
                if (lineDto == null)
                {
                    AddWarningMessageServices($"Line with ID {linePlanned.LineId} not found in simulation data.");
                    continue;
                }

                var skus = simulationDto.SKULines.Where(x => x.LineId == linePlanned.LineId).ToList();
                if (!skus.Any())
                {
                    //favor crear message service aqui
                    AddWarningMessageServices($"No SKUs found for Line ID {linePlanned.LineId} (\"{linePlanned.LineName}\").");
                    continue;
                }
                lineFactory.CreateSKUs(existingLine, linePlanned, lineDto, skus, Materials);
                foreach (var preferedmixer in linePlanned.PreferedMixerDTOs)
                {
                    var mixer = Equipments.OfType<ProcessMixer>().FirstOrDefault(x => x.Id == preferedmixer.MixerId);
                    if (mixer != null)
                    {
                        existingLine.PreferredManufacturer.Add(mixer);
                        mixer.PreferredLines.Add(existingLine);
                    }
                }

            }

        }

        private void ReadMaterials(List<MaterialDTO> materials)
        {
            var materialFactoryProvider = new MaterialFactoryProvider();
            var createdMaterials = materialFactoryProvider.CreateMaterials(materials, _messageService);
            Materials.AddRange(createdMaterials);
        }
        void ReadTanks(List<TankDTO> tanks)
        {
            TankFactoryProvider _tankFactoryProvider = new TankFactoryProvider();
            var createdTanks = _tankFactoryProvider.CreateTanks(tanks, _messageService);

            Equipments.AddRange(createdTanks);
        }
        void ReadMixers(List<MixerDTO> mixers)
        {
            foreach (var mixerDTO in mixers)
            {
                var mixer = new ProcessMixer()
                {
                    Id = mixerDTO.Id,
                    Name = mixerDTO.Name,
                    EquipmentType = ProccesEquipmentType.Mixer,


                };

                Equipments.Add(mixer);
            }
        }

        void ReadSkids(List<ContinuousSystemDTO> skids)
        {
            foreach (var skidDTO in skids)
            {
                var skid = new ProcessContinuousSystem()
                {
                    Id = skidDTO.Id,
                    Name = skidDTO.Name,
                    EquipmentType = ProccesEquipmentType.ContinuousSystem,
                    Capacity = skidDTO.Flow
                };
                Equipments.Add(skid);
            }
        }
        void ReadOperators(List<OperatorDTO> operators)
        {
            foreach (var operatorDTO in operators)
            {
                var oper = new ProcessOperator()
                {
                    Id = operatorDTO.Id,
                    Name = operatorDTO.Name,
                    EquipmentType = ProccesEquipmentType.Operator,
                };
                Equipments.Add(oper);
            }
        }
        public void Init(DateTime currendate)
        {
            CalculateTopologicalLevels();



            var orderedEquipments = Equipments.OrderByDescending(e => e.TopologicalLevel).ToList();
            CriticalDowntimeReportManager = new CriticalDowntimeReportManager(this);

            // ✅ Asignar CriticalDowntimeReportManager a TODOS los equipos
            Equipments.ForEach(x =>
            {

                x.ReportManager = CriticalDowntimeReportManager;
                x.Init(currendate);
            });



        }

        public Amount CurrenTime { get; set; } = new Amount(0, TimeUnits.Second);
        public async Task RunSimulationAsync()
        {
            Init(InitDate);
            IsSimulationRunning = true;
            IsSimulationPaused = false;
            StopSimulationRequested = false;
            IsSimulationFinished = false;
            var totaltime = EndDate - InitDate;
            TotalSimulacionTime = new Amount(totaltime.TotalSeconds, TimeUnits.Second);
            CurrenTime = new Amount(0, TimeUnits.Second);


            var orderedEquipments = Equipments.OrderBy(e => e.TopologicalLevel).ToList();
            do
            {
                while (IsSimulationPaused && !StopSimulationRequested)
                {

                    if (StopSimulationRequested) break;
                }

                // Verificar si se solicitó detener
                if (StopSimulationRequested || !IsSimulationRunning)
                {
                    break;
                }



                foreach (var equipment in orderedEquipments)
                {
                    if (equipment.Name == "T.WIPSKID")
                    {

                    }
                    equipment.Calculate(CurrentDate);
                }

                CurrentDate = CurrentDate.AddSeconds(1);
                CurrenTime += OneSecond;

                await UpdateModel();


            }
            while (CurrenTime < TotalSimulacionTime && IsSimulationRunning && !StopSimulationRequested);
            if (!OnLiveReport) await UpdateModel();
        }

        private void CalculateTopologicalLevels()
        {
            foreach (var equipment in Equipments)
            {
                equipment.TopologicalLevel = -1;
            }

            var queue = new Queue<IEquipment>();

            foreach (var line in Equipments.OfType<ProcessLine>())
            {
                line.TopologicalLevel = 0;
                queue.Enqueue(line);
            }

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                var nextLevel = current.TopologicalLevel + 1;

                foreach (var inlet in current.InletEquipments)
                {
                    if (inlet.TopologicalLevel == -1)
                    {
                        inlet.TopologicalLevel = nextLevel;
                        queue.Enqueue(inlet);
                    }
                    else
                    {
                        inlet.TopologicalLevel = Math.Max(inlet.TopologicalLevel, nextLevel);
                    }
                }
            }

            // Asignar nivel alto a equipos no conectados a líneas
            foreach (var equipment in Equipments)
            {
                if (equipment.TopologicalLevel == -1)
                {
                    equipment.TopologicalLevel = 999;
                }
            }
        }
        public void AddWarningMessageServices(string message)
        {
            _messageService.AddWarning(message, "GeneralSimulation");
        }
        public void PauseSimulation()
        {
            if (IsSimulationRunning && !IsSimulationPaused)
            {
                IsSimulationPaused = true;
            }
        }

        public void ResumeSimulation()
        {
            if (IsSimulationRunning && IsSimulationPaused)
            {
                IsSimulationPaused = false;
            }
        }

        public void StopSimulation()
        {
            StopSimulationRequested = true;
            IsSimulationRunning = false;
            IsSimulationPaused = false;
        }
        public Func<Task> UpdateModel { get; set; } = null!;
        public DateTime CurrentDate { get; set; }
        public TimeSpan SimulationTime { get; set; } = new();
        public bool IsSimulationFinished { get; set; } = false;
        public void ResetSimulation()
        {
            StopSimulation();
            // Reiniciar variables de simulación
            CurrentDate = InitDate;
            SimulationTime = TimeSpan.Zero;
            IsSimulationFinished = false;
            StopSimulationRequested = false;
            // Reiniciar otros estados según sea necesario
            UpdateModel?.Invoke();
        }
        public CurrentShift CurrentShift => CheckShift(CurrentDate);



        CurrentShift CheckShift(DateTime currentTime) =>
             currentTime.Hour switch
             {
                 >= 6 and < 14 => CurrentShift.Shift_1,
                 >= 14 and < 22 => CurrentShift.Shift_2,
                 _ => CurrentShift.Shift_3
             };

        void ReadPlannedDownTimes(List<BaseEquipmentDTO> equipments)
        {
            foreach (var equipment in equipments)
            {
                var eq = Equipments.FirstOrDefault(x => x.Id == equipment.Id);
                if (eq != null)
                {
                    foreach (var planneddowtime in equipment.PlannedDownTimes)
                    {
                        eq.PlannedDownTimes.Add(new PlannedDownTime
                        {
                            Start = planneddowtime.StartTime!.Value,
                            End = planneddowtime.EndTime!.Value,
                        });
                    }
                }
            }
        }



    }
}
