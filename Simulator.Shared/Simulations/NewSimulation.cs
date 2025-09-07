using Simulator.Shared.Enums.HCEnums.Enums;
using Simulator.Shared.Models.HCs.Conectors;
using Simulator.Shared.Models.HCs.ContinuousSystems;
using Simulator.Shared.Models.HCs.Lines;
using Simulator.Shared.Models.HCs.MainProcesss;
using Simulator.Shared.Models.HCs.MaterialEquipments;
using Simulator.Shared.Models.HCs.Mixers;
using Simulator.Shared.Models.HCs.Operators;
using Simulator.Shared.Models.HCs.Pumps;
using Simulator.Shared.Models.HCs.SimulationPlanneds;
using Simulator.Shared.Models.HCs.SKULines;
using Simulator.Shared.Models.HCs.Tanks;
using Simulator.Shared.Simulations.Lines;
using Simulator.Shared.Simulations.Materials;
using Simulator.Shared.Simulations.Mixers;
using Simulator.Shared.Simulations.Operators;
using Simulator.Shared.Simulations.ProductionAnalyzers;
using Simulator.Shared.Simulations.Pumps;
using Simulator.Shared.Simulations.Skids;
using Simulator.Shared.Simulations.Tanks;
using Simulator.Shared.Simulations.Tanks.WIPInletSKIDs;
using Simulator.Shared.Simulations.Tanks.WIPLinetMixers;
using System.Diagnostics;
using static Simulator.Shared.Simulations.ProductionAnalyzers.ProductionAnalyzer;

namespace Simulator.Shared.Simulations
{
    public class NewSimulation
    {
        // Nuevas propiedades para control de simulación
        public bool IsSimulationRunning { get; set; } = false;
        public bool IsSimulationPaused { get; set; } = false;
        public bool StopSimulationRequested { get; set; } = false;

        public List<SKULineDTO> SKULines { get; set; } = new();

        public bool IsSimulationFinished { get; set; } = false;

        public SimulationPlannedDTO Planned { get; private set; } = null!;

        public NewSimulation()
        {

        }
        public ProductionAnalyzer ProductionAnalyzer { get; private set; } = null!;

        // En algún método de inicialización o cuando se crea la simulación:
        public List<EquipmentAnalysisResult> EquipmentAnalysisResults { get; private set; } = new();
        public void AnalyzeProductionCapabilities()
        {
            ProductionAnalyzer = new ProductionAnalyzer(this);
            ProductionAnalyzer.AnalyzeAllProductionCapabilities();
            EquipmentAnalysisResults = ProductionAnalyzer.GetAnalysisResultsForUI();

        }
        public void CleanAnalysisResults()
        {
            EquipmentAnalysisResults.Clear();
        }
        public List<WashoutSimulation> WashouTimes = new();

        List<ConectorSimulation> Conectors = new();

        public List<MaterialSimulation> MaterialSimulations { get; set; } = new();
        public List<RawMaterialSimulation> RawMaterialSimulations { get; set; } = new();
        public List<BackBoneRawMaterialSimulation> BackBoneRawMaterialSimulations { get; set; } = new();
        public List<ProductBackBoneSimulation> ProductBackBoneSimulations { get; set; } = new();
        public List<BackBoneSimulation> BackBoneSimulations { get; set; } = new();
        public List<NewBaseEquipment> SimulationEquipments => [.. Lines, .. Tanks, .. Mixers, .. Pumps, .. SKIDs, .. Operators];


        public List<SKUSimulation> SkuSimulations { get; set; } = new();
        public List<BaseLine> Lines { get; private set; } = new();
        public List<BaseLine> LinesOrdered => Lines.OrderBy(x => x.Name).ToList();
        public List<BaseLine> ScheduledLines { get; private set; } = new();

        public List<BaseMixer> Mixers { get; private set; } = new();

        public List<BaseMixer> MixersInProcess => Mixers.Where(x => x.HasProcessConnected == true).ToList();
        public List<BaseMixer> MixerOrdered => Mixers.Count == 0 ? new() : Mixers.OrderBy(x => x.Name).ToList();

        public List<BaseTank> Tanks { get; private set; } = new();
        public List<RawMaterialTank> RawMaterialTank { get; private set; } = new();
        public List<BackBoneRawMaterialTank> BackBoneRawMaterialTanks { get; private set; } = new();
        public List<BackBoneRawMaterialTank> BackBoneRawMaterialTanksInProcess => BackBoneRawMaterialTanks.Where(x => x.GetEquipmentOcupiedBy).ToList();

        public List<WIPInletMixer> WIPMixerTank { get; private set; } = new();
        public List<WIPInletMixer> WIPMixerTankOrdered => WIPMixerTank.Count == 0 ? new() : WIPMixerTank.OrderBy(x => x.Name).ToList();
        public List<WIPInletSKID> WIPSKIDTank { get; private set; } = new();
        public List<WIPInletSKID> WIPSKIDTankOrdered => WIPSKIDTank.Count == 0 ? new() : WIPSKIDTank.OrderBy(x => x.Name).ToList();

        public List<WIPForProductBackBone> WIPProductTanks { get; private set; } = new();
        public List<WIPForProductBackBone> WIPProductTanksInProcess => WIPProductTanks.Where(x => x.GetEquipmentOcupiedBy).ToList();

        public List<BaseSKID> SKIDs { get; private set; } = new List<BaseSKID>();
        public List<BaseSKID> SKIDsExcelResults => SKIDs.Where(x => x.HasExcelresult == true).ToList();
        public List<BaseOperator> Operators { get; private set; } = new();
        public List<BasePump> Pumps { get; private set; } = new();
        public Action UpdateModel { get; set; } = null!;
        public DateTime CurrentDate { get; set; }

        public DateTime InitDate { get; private set; } = DateTime.Now;


        public DateTime EndDate { get; private set; } = DateTime.Now;
        public Amount TotalSimulacionTime { get; private set; } = new Amount(TimeUnits.Hour);
        Amount OneSecond = new(1, TimeUnits.Second);

        public void AddConnector(NewBaseEquipment from, NewBaseEquipment to)
        {
            Conectors.Add(new()
            {
                From = from,
                To = to,


            });
        }
        void ConectLines(List<ConectorSimulation> Conectors)
        {

            var lineconectors = Conectors.Where(x => x.To is BaseLine).ToList();
            foreach (var lineconector in lineconectors)
            {
                var line = GetBaseLine(lineconector.To);
                if (lineconector.From is BasePump)
                {
                    var pump = GetBasePump(lineconector.From);


                    line.AddConnectedInletEquipment(pump);
                    if (!pump.IsForWashing)
                    {
                        var tank = pump.GetInletAttachedEquipment();
                        line.AddProcessInletEquipment(pump);

                        pump.AddProcessInletEquipment(tank);
                    }


                }


            }

        }
        void ConectPumps(List<ConectorSimulation> Conectors)
        {
            var pumpconectors = Conectors.Where(x => x.To is BasePump).ToList();
            foreach (var pumpconector in pumpconectors)
            {
                var pump = GetBasePump(pumpconector.To);
                if (pumpconector.From is BaseTank)
                {
                    var tank = GetBaseTank(pumpconector.From);


                    pump.AddConnectedInletEquipment(tank);
                    pump.AddProcessInletEquipment(tank);

                }
                else if (pumpconector.From is BaseMixer)
                {
                    var mixer = GetBaseMixer(pumpconector.From);


                    pump.AddConnectedInletEquipment(mixer);
                    pump.AddProcessInletEquipment(mixer);

                }


            }
        }
        void ConectTanks(List<ConectorSimulation> Conectors)
        {
            var tankconectors = Conectors.Where(x => x.To is BaseTank).ToList();
            foreach (var tankconector in tankconectors)
            {
                var tank = GetBaseTank(tankconector.To);

                if (tankconector.From is BasePump)
                {
                    var pump = GetBasePump(tankconector.From);

                    tank.AddConnectedInletEquipment(pump);
                }
                else if (tankconector.From is BaseSKID)
                {
                    var skid = GetBaseSKID(tankconector.From);
                    tank.AddConnectedInletEquipment(skid);

                }
            }
        }

        void ConectMixers(List<ConectorSimulation> Conectors)
        {
            var mixerconectors = Conectors.Where(x => x.To is BaseMixer).ToList();
            foreach (var mixerconector in mixerconectors)
            {
                var mixer = GetBaseMixer(mixerconector.To);

                if (mixerconector.From is BasePump)
                {

                    var pump = GetBasePump(mixerconector.From);
                    mixer.AddConnectedInletEquipment(pump);



                }
                else if (mixerconector.From is BaseOperator)
                {
                    var oper = GetBaseOperator(mixerconector.From);

                    mixer.AddConnectedInletEquipment(oper);

                }
            }
        }
        void ConectSkids(List<ConectorSimulation> Conectors)
        {
            var skidconectors = Conectors.Where(x => x.To is BaseSKID).ToList();
            foreach (var skidconector in skidconectors)
            {
                var skid = GetBaseSKID(skidconector.To);
                if (skidconector.From is BasePump)
                {
                    var pump = GetBasePump(skidconector.From);

                    skid.AddConnectedInletEquipment(pump);

                    skid.AddProcessInletEquipment(pump);
                }
            }
        }
        public void CreateProcess(List<MaterialEquipmentRecord> processEquipmentMaterials)
        {
            ConectPumps(Conectors);
            ConectTanks(Conectors);
            ConectMixers(Conectors);
            ConectSkids(Conectors);
            ConectLines(Conectors);
            SetMaterialToOtherEquipments();
            BackBoneSimulations.ForEach(x => x.Init(processEquipmentMaterials));
        }
        void SetMaterialToOtherEquipments()
        {
            Tanks.ForEach(x => x.SetMaterialsOutlet());
            Mixers.ForEach(x => x.SetMaterialsOutlet());
            SKIDs.ForEach(x => x.SetMaterialsOutlet());
        }
        void CreateSimulation()
        {
            ScheduledLines = InitLines(InitDate);



        }
        public void SetPlanned(SimulationPlannedDTO _Planned)
        {
            Planned = _Planned;
            InitDate = _Planned.InitDate!.Value;
            EndDate = _Planned.InitDate!.Value.AddHours(_Planned.Hours);
            TotalSimulacionTime = new(_Planned.Hours, TimeUnits.Hour);

            var _currentDate = _Planned.InitDate!.Value;
            CurrentDate = new(_currentDate.Year, _currentDate.Month, _currentDate.Day, 6, 0, 0);
            CreateSimulation();
            InitTanks();
            InitMixers();

        }

        public async Task<bool> RunSimulation()
        {
            IsSimulationRunning = true;
            IsSimulationPaused = false;
            StopSimulationRequested = false;
            IsSimulationFinished = false;

            Amount currentime = new(1, TimeUnits.Second);
            Stopwatch Elapsed = Stopwatch.StartNew();
            DateTime check = new DateTime(2023, 6, 29, 21, 59, 0);
            try
            {
                do
                {// Verificar si se solicitó pausa
                    while (IsSimulationPaused && !StopSimulationRequested)
                    {
                        await Task.Delay(100); // Esperar mientras está pausado
                        if (StopSimulationRequested) break;
                    }

                    // Verificar si se solicitó detener
                    if (StopSimulationRequested || !IsSimulationRunning)
                    {
                        break;
                    }


                    foreach (var line in ScheduledLines)
                    {
                        line.Calculate(CurrentDate);
                        CheckLineToRemove(line);
                    }
                    foreach (var wiptank in WIPProductTanksInProcess)
                    {
                        wiptank.Calculate(CurrentDate);
                    }
                    foreach (var row in BackBoneRawMaterialTanksInProcess)
                    {
                        row.Calculate(CurrentDate);
                    }
                    foreach (var mixer in MixersInProcess)
                    {
                        mixer.Calculate(CurrentDate);
                    }

                    ScheduledLines = RemoveLines(ScheduledLines);

                    await Task.Delay(10);
                    UpdateModel();

                    CurrentDate = CurrentDate.AddSeconds(1);

                    currentime += OneSecond;
                    if (ScheduledLines.Count == 0) break;
                    SimulationTime = Elapsed.Elapsed;
                } while (currentime < TotalSimulacionTime && IsSimulationRunning && !StopSimulationRequested);
            }
            catch (Exception ex)
            {
                string exm = ex.Message;
            }

            finally
            {
                Elapsed.Stop();
                SimulationTime = Elapsed.Elapsed;
                IsSimulationRunning = false;
                IsSimulationPaused = false;

                if (StopSimulationRequested)
                {
                    IsSimulationFinished = false; // No terminó naturalmente
                }
                else
                {
                    IsSimulationFinished = true; // Terminó naturalmente
                }

                UpdateModel(); // Actualizar UI al final
            }

            return true;

        }// Nuevos métodos para control de simulación
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
        public TimeSpan SimulationTime { get; set; } = new();

        public CurrentShift CurrentShift => CheckShift(CurrentDate);



        CurrentShift CheckShift(DateTime currentTime) =>
             currentTime.Hour switch
             {
                 >= 6 and < 14 => CurrentShift.Shift_1,
                 >= 14 and < 22 => CurrentShift.Shift_2,
                 _ => CurrentShift.Shift_3
             };

        List<BaseLine> newlines = new List<BaseLine>();

        List<BaseLine> RemoveLines(List<BaseLine> oldlines)
        {
            foreach (var dt in newlines)
            {
                oldlines.Remove(dt);
            }

            return oldlines;
        }
        void CheckLineToRemove(BaseLine line)
        {
            if (!line.LineScheduled)
            {
                newlines.Add(line);
            }
        }
        List<BaseLine> InitLines(DateTime currendate)
        {

            List<BaseLine> retorno = new();
            foreach (BaseLine? line in LinesOrdered)
            {
                var plannline = Planned.PlannedLines.FirstOrDefault(x => x.LineId == line.Id);
                if (plannline != null)
                {
                    line!.SetTime(currendate);
                    line.InitLine(plannline, SkuSimulations, SKULines);
                }


                if (line.LineScheduled)
                {
                    retorno.Add(line!);

                }

            }

            return retorno!;
        }
        void InitTanks()
        {
            foreach (var tank in Tanks)
            {
                tank.Init();
            }





        }
        void InitMixers()
        {
            try
            {
                foreach (var mixer in Mixers)
                {
                    mixer.Init(this);
                }
            }
            catch (Exception ex)
            {
                string message = ex.Message;
            }

            //Mixers.ForEach(x => x.Init(this));
        }
        public void AddLines(List<LineDTO> equipments)
        {
            equipments.ForEach(x => AddBaseLine(x));
        }
        public void AddTanks(List<TankDTO> equipments)
        {
            equipments.ForEach(x => AddBaseTank(x));
        }
        public void AddMixers(List<MixerDTO> equipments)
        {
            equipments.ForEach(x => AddBaseMixer(x));
        }
        public void AddPumps(List<PumpDTO> equipments)
        {
            equipments.ForEach(x => AddBasePump(x));
        }
        public void AddSkids(List<ContinuousSystemDTO> equipments)
        {
            equipments.ForEach(x => AddBaseSKID(x));
        }
        public void AddOperators(List<OperatorDTO> equipments)
        {
            equipments.ForEach(x => AddBaseOperator(x));
        }




        void AddBasePump(PumpDTO dto)
        {
            if (!Pumps.Any(x => x.Id == dto.Id))
            {

                var retorno = new BasePump(dto);
                Pumps.Add(retorno);
                // SUSCRIBIR el equipo al manejador de eventos
                SubscribeEquipmentToEvents(retorno);
            }



        }

        BasePump GetBasePump(NewBaseEquipment dto)
        {
            if (Pumps.Any(x => x.Id == dto.Id))
            {
                var pump = Pumps.Single(x => x.Id == dto.Id);
                return pump!;
            }

            return null!;
        }

        void AddBaseLine(LineDTO dto)
        {
            if (!Lines.Any(x => x.Id == dto.Id))
            {
                var retorno = new BaseLine();
                retorno.SetLine(dto, WashouTimes);
                // SUSCRIBIR el equipo al manejador de eventos
                SubscribeEquipmentToEvents(retorno);
                Lines.Add(retorno);
            }


        }
        BaseLine GetBaseLine(NewBaseEquipment dto)
        {
            if (Lines.Any(x => x.Id == dto.Id))
            {
                var line = Lines.Single(x => x.Id == dto.Id);
                return line!;
            }

            return null!;
        }
        void AddBaseTank(TankDTO dto)
        {
            if (!Tanks.Any(x => x.Id == dto.Id))
            {
                var retorno = AddTank(dto);
                if (retorno == null)
                {
                    return;
                }
                Tanks.Add(retorno);
                // SUSCRIBIR el equipo al manejador de eventos
                SubscribeEquipmentToEvents(retorno);
            }

        }
        BaseTank GetBaseTank(NewBaseEquipment dto)
        {
            if (Tanks.Any(x => x.Id == dto.Id))
            {
                var tank = Tanks.Single(x => x.Id == dto.Id);

                return tank!;
            }

            return null!;
        }
        BaseTank AddTank(TankDTO dto)
        {
            switch (dto.FluidStorage)
            {
                case FluidToStorage.RawMaterial:
                    {
                        var result = new RawMaterialTank(dto);
                        RawMaterialTank.Add(result);
                        // SUSCRIBIR el equipo al manejador de eventos
                        SubscribeEquipmentToEvents(result);
                        return result;
                    }

                case FluidToStorage.ProductBackBone:
                    {
                        switch (dto.TankCalculationType)
                        {
                            case TankCalculationType.BatchCycleTime:
                                {
                                    var result = new WIPInletMixer(dto);
                                    WIPMixerTank.Add(result);
                                    SubscribeEquipmentToEvents(result);
                                    return result;
                                }
                            case TankCalculationType.ContinuousSystemHiLoLevel:
                            case TankCalculationType.AutomaticHiLoLevel:
                                {
                                    var result = new WIPInletSKID(dto);
                                    WIPSKIDTank.Add(result);
                                    SubscribeEquipmentToEvents(result);
                                    return result;
                                }

                        }

                    }
                    break;
                case FluidToStorage.ProductBackBoneToWIPs:
                    {
                        var result = new WIPForProductBackBone(dto);
                        WIPProductTanks.Add(result);
                        SubscribeEquipmentToEvents(result);

                        return result;
                    }
                case FluidToStorage.RawMaterialBackBone:
                    {
                        switch (dto.TankCalculationType)

                        {
                            case TankCalculationType.BatchCycleTime:
                                {
                                    var result = new BackBoneRawMaterialTank(dto);
                                    BackBoneRawMaterialTanks.Add(result);
                                    SubscribeEquipmentToEvents(result);
                                    return result;

                                }
                            case TankCalculationType.AutomaticHiLoLevel:
                                {
                                    var result = new RawMaterialTank(dto);
                                    RawMaterialTank.Add(result);
                                    SubscribeEquipmentToEvents(result);
                                    return result;

                                }

                        }
                        break;
                    }


            }
            return null!;
        }
        void AddBaseSKID(ContinuousSystemDTO dto)
        {
            if (!SKIDs.Any(x => x.Id == dto.Id))
            {
                var retorno = new BaseSKID(dto);
                // SUSCRIBIR el equipo al manejador de eventos
                SubscribeEquipmentToEvents(retorno);
                SKIDs.Add(retorno);
            }


        }
        BaseSKID GetBaseSKID(NewBaseEquipment dto)
        {
            if (SKIDs.Any(x => x.Id == dto.Id))
            {
                var skid = SKIDs.Single(x => x.Id == dto.Id);
                return skid!;
            }

            return null!;
        }

        void AddBaseOperator(OperatorDTO dto)
        {
            if (!Operators.Any(x => x.Id == dto.Id))
            {
                var retorno = new BaseOperator(dto);
                // SUSCRIBIR el equipo al manejador de eventos
                SubscribeEquipmentToEvents(retorno);

                Operators.Add(retorno);
            }


        }
        BaseOperator GetBaseOperator(NewBaseEquipment dto)
        {
            if (Operators.Any(x => x.Id == dto.Id))
            {
                var oper = Operators.Single(x => x.Id == dto.Id);
                return oper!;
            }

            return null!;
        }

        void AddBaseMixer(MixerDTO dto)
        {
            if (!Mixers.Any(x => x.Id == dto.Id))
            {
                var retorno = new BaseMixer(dto, WashouTimes);

                // SUSCRIBIR el equipo al manejador de eventos
                SubscribeEquipmentToEvents(retorno);
                Mixers.Add(retorno);
            }




        }
        BaseMixer GetBaseMixer(NewBaseEquipment dto)
        {
            if (Mixers.Any(x => x.Id == dto.Id))
            {
                var mixer = Mixers.Single(x => x.Id == dto.Id);
                return mixer!;
            }

            return null!;
        }

        public void MapMaterialEquipments(List<MaterialEquipmentRecord> processEquipmentMaterials)
        {
            foreach (var item in processEquipmentMaterials)
            {
                if (item.MaterialId == Guid.Empty || item.EquipmentId == Guid.Empty)
                {
                    continue;
                }
                var equipment = SimulationEquipments.FirstOrDefault(x => x.Id == item.EquipmentId);
                var material = MaterialSimulations.FirstOrDefault(x => x.Id == item.MaterialId);

                if (material != null && equipment != null)
                {
                    equipment.AddMaterialSimulation(material);


                }



            }
        }
        public void MapConnectors(List<ConnectorRecord> connectors)
        {
            foreach (var item in connectors)
            {
                if (item.FromId == Guid.Empty || item.ToId == Guid.Empty)
                {
                    continue;
                }
                var from = SimulationEquipments.FirstOrDefault(x => x.Id == item.FromId);
                var to = SimulationEquipments.FirstOrDefault(x => x.Id == item.ToId);

                if (from != null && to != null)
                {
                    AddConnector(from, to);


                }



            }
        }
        public List<NewBaseEquipmentEventArgs> EquipmentEvents { get; private set; } = new();

        // Diccionario para rastrear eventos activos
        private Dictionary<string, NewBaseEquipmentEventArgs> ActiveEvents = new();

        // Método para suscribir un equipo al manejador de eventos y establecer referencia
        public void SubscribeEquipmentToEvents(NewBaseEquipment equipment)
        {
            equipment.Simulation = this; // ESTABLECER REFERENCIA A LA SIMULACIÓN
            equipment.EquipmentEvent += HandleEquipmentEvent;
        }

        // Método para desuscribir un equipo
        public void UnsubscribeEquipmentFromEvents(NewBaseEquipment equipment)
        {
            equipment.EquipmentEvent -= HandleEquipmentEvent;
        }

        // Manejador centralizado de eventos de equipos
        private void HandleEquipmentEvent(object? sender, NewBaseEquipmentEventArgs e)
        {
            switch (e.EventType)
            {
                case "Started":
                    // Registrar evento de inicio
                    EquipmentEvents.Add(e);
                    // Usar EventId para correlacionar eventos de inicio y fin
                    if (!string.IsNullOrEmpty(e.EventId))
                    {
                        ActiveEvents[e.EventId] = e;
                    }
                    break;

                case "Ended":
                    // Registrar evento de fin
                    EquipmentEvents.Add(e);

                    // Si existe un evento de inicio correspondiente, calcular duración
                    if (!string.IsNullOrEmpty(e.EventId) && ActiveEvents.ContainsKey(e.EventId))
                    {
                        var startedEvent = ActiveEvents[e.EventId];
                        var duration = e.Timestamp - startedEvent.Timestamp;
                        startedEvent.Duration = duration;
                        e.Duration = duration;

                        // Remover evento activo
                        ActiveEvents.Remove(e.EventId);
                    }
                    break;

                case "Instant":
                    // Evento puntual
                    EquipmentEvents.Add(e);
                    break;
            }

            // Notificar a la UI si es necesario
            UpdateModel?.Invoke();
        }
        // Agrega este método en la clase NewSimulation
        public void ClearEquipmentEvents()
        {
            EquipmentEvents.Clear();
            ActiveEvents.Clear();

            // Opcional: Notificar a la UI que se limpiaron los eventos
            UpdateModel?.Invoke();
        }

    }
}
