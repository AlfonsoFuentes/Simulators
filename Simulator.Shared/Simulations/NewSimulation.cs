using Simulator.Shared.Enums.HCEnums.Enums;
using Simulator.Shared.Models.HCs.SimulationPlanneds;
using Simulator.Shared.Models.HCs.SKULines;
using System.Diagnostics;

namespace Simulator.Shared.Simulations
{
    //public partial class NewSimulation
    //{
        // Nuevas propiedades para control de simulación
    //    public bool IsSimulationRunning { get; set; } = false;
    //    public bool IsSimulationPaused { get; set; } = false;
    //    public bool StopSimulationRequested { get; set; } = false;

    //    public List<SKULineDTO> SKULines { get; set; } = new();

    //    public bool IsSimulationFinished { get; set; } = false;

    //    public SimulationPlannedDTO Planned { get; private set; } = null!;

    //    public NewSimulation()
    //    {

    //    }
    //    public ProductionAnalyzer ProductionAnalyzer { get; private set; } = null!;

    //    // En algún método de inicialización o cuando se crea la simulación:
    //    public List<EquipmentAnalysisResult> EquipmentAnalysisResults { get; private set; } = new();
    //    public void AnalyzeProductionCapabilities()
    //    {
    //        ProductionAnalyzer = new ProductionAnalyzer(this);
    //        ProductionAnalyzer.AnalyzeAllProductionCapabilities();
    //        EquipmentAnalysisResults = ProductionAnalyzer.GetAnalysisResultsForUI();

    //    }
    //    public void CleanAnalysisResults()
    //    {
    //        EquipmentAnalysisResults.Clear();
    //    }
    //    public List<WashoutSimulation> WashouTimes = new();

    //    List<ConectorSimulation> Conectors = new();

    //    public List<MaterialSimulation> MaterialSimulations { get; set; } = new();
    //    public List<RawMaterialSimulation> RawMaterialSimulations { get; set; } = new();
    //    public List<BackBoneRawMaterialSimulation> BackBoneRawMaterialSimulations { get; set; } = new();
    //    public List<ProductBackBoneSimulation> ProductBackBoneSimulations { get; set; } = new();
    //    public List<BackBoneSimulation> BackBoneSimulations { get; set; } = new();
    //    public List<NewBaseEquipment> SimulationEquipments => [.. Lines, .. Tanks, .. Mixers, .. Pumps, .. SKIDs, .. Operators];


    //    public List<SKUSimulation> SkuSimulations { get; set; } = new();
    //    public List<BaseLine> Lines { get; private set; } = new();
    //    public List<BaseLine> LinesOrdered => Lines.OrderBy(x => x.Name).ToList();
    //    public List<BaseLine> ScheduledLines { get; private set; } = new();

    //    public List<BaseMixer> Mixers { get; private set; } = new();

    //    public List<BaseMixer> MixersInProcess => Mixers.Where(x => x.HasProcessConnected == true).ToList();
    //    public List<BaseMixer> MixerOrdered => Mixers.Count == 0 ? new() : Mixers.OrderBy(x => x.Name).ToList();

    //    public List<BaseTank> Tanks { get; private set; } = new();
    //    public List<RawMaterialTank> RawMaterialTank { get; private set; } = new();
    //    public List<BackBoneRawMaterialTank> BackBoneRawMaterialTanks { get; private set; } = new();
    //    public List<BackBoneRawMaterialTank> BackBoneRawMaterialTanksInProcess => BackBoneRawMaterialTanks.Where(x => x.GetEquipmentOcupiedBy).ToList();

    //    public List<WIPInletMixer> WIPMixerTank { get; private set; } = new();
    //    public List<WIPInletMixer> WIPMixerTankOrdered => WIPMixerTank.Count == 0 ? new() : WIPMixerTank.OrderBy(x => x.Name).ToList();
    //    public List<WIPInletSKID> WIPSKIDTank { get; private set; } = new();
    //    public List<WIPInletSKID> WIPSKIDTankOrdered => WIPSKIDTank.Count == 0 ? new() : WIPSKIDTank.OrderBy(x => x.Name).ToList();

    //    public List<WIPForProductBackBone> WIPProductTanks { get; private set; } = new();
    //    public List<WIPForProductBackBone> WIPProductTanksInProcess => WIPProductTanks.Where(x => x.GetEquipmentOcupiedBy).ToList();

    //    public List<BaseSKID> SKIDs { get; private set; } = new List<BaseSKID>();
    //    public List<BaseSKID> SKIDsExcelResults => SKIDs.Where(x => x.HasExcelresult == true).ToList();
    //    public List<BaseOperator> Operators { get; private set; } = new();
    //    public List<BasePump> Pumps { get; private set; } = new();
    //    public Action UpdateModel { get; set; } = null!;
    //    public DateTime CurrentDate { get; set; }

    //    public DateTime InitDate { get; private set; } = DateTime.Now;


    //    public DateTime EndDate { get; private set; } = DateTime.Now;
    //    public Amount TotalSimulacionTime { get; private set; } = new Amount(TimeUnits.Hour);
    //    Amount OneSecond = new(1, TimeUnits.Second);

       
    //    void SetMaterialToOtherEquipments()
    //    {
    //        Tanks.ForEach(x => x.SetMaterialsOutlet());
    //        Mixers.ForEach(x => x.SetMaterialsOutlet());
    //        SKIDs.ForEach(x => x.SetMaterialsOutlet());
    //    }
    //    void CreateSimulation()
    //    {
    //        ScheduledLines = InitLines(InitDate);



    //    }
    //    public void SetPlanned(SimulationPlannedDTO _Planned)
    //    {
    //        Planned = _Planned;
    //        InitDate = _Planned.InitDate!.Value;
    //        EndDate = _Planned.InitDate!.Value.AddHours(_Planned.Hours);
    //        TotalSimulacionTime = new(_Planned.Hours, TimeUnits.Hour);

    //        var _currentDate = _Planned.InitDate!.Value;
    //        CurrentDate = new(_currentDate.Year, _currentDate.Month, _currentDate.Day, 6, 0, 0);
    //        CreateSimulation();
    //        InitTanks();
    //        InitMixers();

    //    }

    //    public async Task<bool> RunSimulation()
    //    {
    //        IsSimulationRunning = true;
    //        IsSimulationPaused = false;
    //        StopSimulationRequested = false;
    //        IsSimulationFinished = false;

    //        Amount currentime = new(1, TimeUnits.Second);
    //        Stopwatch Elapsed = Stopwatch.StartNew();
    //        DateTime check = new DateTime(2023, 6, 29, 21, 59, 0);
    //        try
    //        {
    //            do
    //            {// Verificar si se solicitó pausa
    //                while (IsSimulationPaused && !StopSimulationRequested)
    //                {
    //                    await Task.Delay(100); // Esperar mientras está pausado
    //                    if (StopSimulationRequested) break;
    //                }

    //                // Verificar si se solicitó detener
    //                if (StopSimulationRequested || !IsSimulationRunning)
    //                {
    //                    break;
    //                }


    //                foreach (var line in ScheduledLines)
    //                {
    //                    line.Calculate(CurrentDate);
    //                    CheckLineToRemove(line);
    //                }
    //                foreach (var wiptank in WIPProductTanksInProcess)
    //                {
    //                    wiptank.Calculate(CurrentDate);
    //                }
    //                foreach (var row in BackBoneRawMaterialTanksInProcess)
    //                {
    //                    row.Calculate(CurrentDate);
    //                }
    //                foreach (var mixer in MixersInProcess)
    //                {
    //                    mixer.Calculate(CurrentDate);
    //                }

    //                ScheduledLines = RemoveLines(ScheduledLines);

    //                await Task.Delay(10);
    //                UpdateModel();

    //                CurrentDate = CurrentDate.AddSeconds(1);

    //                currentime += OneSecond;
    //                if (ScheduledLines.Count == 0) break;
    //                SimulationTime = Elapsed.Elapsed;
    //            } while (currentime < TotalSimulacionTime && IsSimulationRunning && !StopSimulationRequested);
    //        }
    //        catch (Exception ex)
    //        {
    //            string exm = ex.Message;
    //        }

    //        finally
    //        {
    //            Elapsed.Stop();
    //            SimulationTime = Elapsed.Elapsed;
    //            IsSimulationRunning = false;
    //            IsSimulationPaused = false;

    //            if (StopSimulationRequested)
    //            {
    //                IsSimulationFinished = false; // No terminó naturalmente
    //            }
    //            else
    //            {
    //                IsSimulationFinished = true; // Terminó naturalmente
    //            }

    //            UpdateModel(); // Actualizar UI al final
    //        }

    //        return true;

    //    }// Nuevos métodos para control de simulación
    //    public void PauseSimulation()
    //    {
    //        if (IsSimulationRunning && !IsSimulationPaused)
    //        {
    //            IsSimulationPaused = true;
    //        }
    //    }

    //    public void ResumeSimulation()
    //    {
    //        if (IsSimulationRunning && IsSimulationPaused)
    //        {
    //            IsSimulationPaused = false;
    //        }
    //    }

    //    public void StopSimulation()
    //    {
    //        StopSimulationRequested = true;
    //        IsSimulationRunning = false;
    //        IsSimulationPaused = false;
    //    }

    //    public void ResetSimulation()
    //    {
    //        StopSimulation();
    //        // Reiniciar variables de simulación
    //        CurrentDate = InitDate;
    //        SimulationTime = TimeSpan.Zero;
    //        IsSimulationFinished = false;
    //        StopSimulationRequested = false;
    //        // Reiniciar otros estados según sea necesario
    //        UpdateModel?.Invoke();
    //    }
    //    public TimeSpan SimulationTime { get; set; } = new();

    //    public CurrentShift CurrentShift => CheckShift(CurrentDate);



    //    CurrentShift CheckShift(DateTime currentTime) =>
    //         currentTime.Hour switch
    //         {
    //             >= 6 and < 14 => CurrentShift.Shift_1,
    //             >= 14 and < 22 => CurrentShift.Shift_2,
    //             _ => CurrentShift.Shift_3
    //         };

    //    List<BaseLine> newlines = new List<BaseLine>();

    //    List<BaseLine> RemoveLines(List<BaseLine> oldlines)
    //    {
    //        foreach (var dt in newlines)
    //        {
    //            oldlines.Remove(dt);
    //        }

    //        return oldlines;
    //    }
    //    void CheckLineToRemove(BaseLine line)
    //    {
    //        if (!line.LineScheduled)
    //        {
    //            newlines.Add(line);
    //        }
    //    }
       

    //    void SubscribeEquipmentToEvents(NewBaseEquipment equipment)
    //    {
    //        equipment.Simulation = this;
    //    }
    //    public Dictionary<Guid, NewBaseEquipmentEventArgs> EquipmentEventsRegistry { get; private set; } = new();
    //    // Método para publicar un nuevo evento
    //    public void PublishEquipmentEvent(NewBaseEquipmentEventArgs eventArgs)
    //    {
    //        if (eventArgs != null && eventArgs.EventId != Guid.Empty)
    //        {
    //            // AQUÍ ESTÁ EL PROBLEMA: 
    //            // EquipmentEventsRegistry[eventArgs.EventId] = eventArgs;
    //            // Esta línea sí agrega el evento, pero...

    //            // MEJORAR LA CLARIDAD:
    //            if (!EquipmentEventsRegistry.ContainsKey(eventArgs.EventId))
    //            {
    //                // AGREGAR nuevo evento
    //                EquipmentEventsRegistry.Add(eventArgs.EventId, eventArgs);
    //            }
    //            else
    //            {
    //                // ACTUALIZAR evento existente
    //                EquipmentEventsRegistry[eventArgs.EventId] = eventArgs;
    //            }

    //            UpdateModel?.Invoke(); // Notificar a la UI
    //        }
    //    }

    //    // Método auxiliar para verificar si se agregó correctamente
    //    public bool ContainsEquipmentEvent(Guid eventId)
    //    {
    //        return EquipmentEventsRegistry.ContainsKey(eventId);
    //    }

    //    public int GetEquipmentEventsCount()
    //    {
    //        return EquipmentEventsRegistry.Count;
    //    }

    //    // Método para actualizar un evento existente
    //    public void UpdateEquipmentEvent(NewBaseEquipmentEventArgs eventArgs)
    //    {
    //        if (eventArgs != null && EquipmentEventsRegistry.ContainsKey(eventArgs.EventId))
    //        {
    //            EquipmentEventsRegistry[eventArgs.EventId] = eventArgs;
    //            UpdateModel?.Invoke(); // Notificar a la UI
    //        }
    //    }

    //    // Método para obtener un evento por ID
    //    public NewBaseEquipmentEventArgs GetEquipmentEventById(Guid eventId)
    //    {
    //        return EquipmentEventsRegistry.TryGetValue(eventId, out var eventArgs) ? eventArgs : null!;
    //    }

    //    // Método para obtener todos los eventos (para la UI)
    //    public List<NewBaseEquipmentEventArgs> GetAllEquipmentEvents()
    //    {
    //        return EquipmentEventsRegistry.Values.ToList();
    //    }

    //    // Método para obtener eventos abiertos
    //    public List<NewBaseEquipmentEventArgs> GetOpenEquipmentEvents()
    //    {
    //        return EquipmentEventsRegistry.Values
    //            .Where(e => e.EventStatus == EventStatus.Open)
    //            .ToList();
    //    }

    //    // Método para obtener eventos cerrados
    //    public List<NewBaseEquipmentEventArgs> GetClosedEquipmentEvents()
    //    {
    //        return EquipmentEventsRegistry.Values
    //            .Where(e => e.EventStatus == EventStatus.Closed)
    //            .ToList();
    //    }

    //    // Método para limpiar eventos (al reiniciar simulación)
    //    public void ClearEquipmentEvents()
    //    {
    //        EquipmentEventsRegistry.Clear();
    //        // También limpiar CurrentEventId de todos los equipos
    //        ClearAllEquipmentCurrentEventIds();
    //    }

    //    // Método auxiliar para limpiar CurrentEventId de todos los equipos
    //    private void ClearAllEquipmentCurrentEventIds()
    //    {
    //        foreach (var equipment in SimulationEquipments)
    //        {
    //            equipment.CurrentEventId = Guid.Empty;
    //        }
    //    }
    //}
}
