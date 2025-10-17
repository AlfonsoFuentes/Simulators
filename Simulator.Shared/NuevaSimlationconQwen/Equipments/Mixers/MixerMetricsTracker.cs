using Simulator.Shared.Enums.HCEnums.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Simulator.Shared.StaticClasses.StaticClass;

namespace Simulator.Shared.NuevaSimlationconQwen.Equipments.Mixers
{  // En Simulator.Shared.NuevaSimlationconQwen.Metrics
    public enum MixerTimeType
    {
        Batching,
        Washing,
        StarvedByInitByOperator,
        StarvedByFeeder,
        StarvedByOperator,
        StarvedByWashoutPump,
        PlannedDowntime,
        NoBatchAssigned,
        TransferToWIP,
        StarvedByTransferToWIP,
        Other
    }
    //public class MixerMetricsTracker
    //{
    //    private readonly ProcessMixer _mixer;
    //    private readonly GeneralSimulation _simulation;
    //    private readonly Dictionary<MixerTimeType, Amount> _timeAccumulators = new();

    //    public MixerMetricsTracker(ProcessMixer mixer, GeneralSimulation simulation)
    //    {
    //        _mixer = mixer;
    //        _simulation = simulation;

    //        // Inicializa todos los acumuladores
    //        foreach (MixerTimeType type in Enum.GetValues<MixerTimeType>())
    //        {
    //            _timeAccumulators[type] = new Amount(0, TimeUnits.Second);
    //        }
    //    }
      


    //    // ✅ Un solo método para registrar tiempo
    //    public void RecordTime(MixerTimeType type)
    //    {
    //        _timeAccumulators[type] += _mixer.OneSecond;
    //    }

    //    // ✅ Acceso a tiempo específico
    //    public double GetPercentageTime(MixerTimeType type)
    //    {
    //        var totalTime = _simulation.CurrenTime.GetValue(TimeUnits.Second);
    //        var currentTypeTime = _timeAccumulators[type].GetValue(TimeUnits.Second);
    //        return totalTime > 0 ? Math.Round((currentTypeTime / totalTime) * 100.0, 2) : 0;

    //    }
    //    // En MixerMetricsTracker.cs

    //    public double GetStarvedTimePercentage() =>
    //        GetPercentageTime(MixerTimeType.StarvedByFeeder) +
    //        GetPercentageTime(MixerTimeType.StarvedByOperator) +
    //        GetPercentageTime(MixerTimeType.StarvedByWashoutPump) +
    //        GetPercentageTime(MixerTimeType.StarvedByTransferToWIP);

    //    public bool IsCurrentlyStarved() =>
    //        _mixer.InletState is IMixerStarved || _mixer.OutletState is IMixerStarved; // ajusta según tus estados


    //    public Amount GetStarvedTime(MixerTimeType type)
    //    {
    //        return new Amount(_timeAccumulators[type].GetValue(TimeUnits.Minute), TimeUnits.Minute);
    //    }
    //}
}
