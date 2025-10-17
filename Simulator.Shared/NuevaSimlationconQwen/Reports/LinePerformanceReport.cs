using Simulator.Shared.NuevaSimlationconQwen.Equipments.Lines;
using Simulator.Shared.NuevaSimlationconQwen.Equipments.Mixers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Simulator.Shared.StaticClasses.StaticClass;

namespace Simulator.Shared.NuevaSimlationconQwen.Reports
{


    public class WipTankForLineReport
    {
        public string Name { get; set; } = string.Empty;
        public Amount TotalStarvedTime => new Amount(Batches.Sum(b => b.TotalStarvedTime.GetValue(TimeUnits.Minute)), TimeUnits.Minute);
        public Amount TotalRealBatchTime => new Amount(Batches.Sum(b => b.RealBatchTime.GetValue(TimeUnits.Minute)), TimeUnits.Minute);
        public double OverallStarvationPercentage =>
            TotalRealBatchTime.GetValue(TimeUnits.Minute) > 0
                ? Math.Round((TotalStarvedTime.GetValue(TimeUnits.Minute) / TotalRealBatchTime.GetValue(TimeUnits.Minute)) * 100, 2)
                : 0;
        public Amount TotalStarvedTimeInitBatch => new Amount(Batches.Sum(b => b.StarvedTimeInitBatch.GetValue(TimeUnits.Minute)), TimeUnits.Minute);
        public Amount TotalStarvedTimeByFeeder => new Amount(Batches.Sum(b => b.StarvedTimeByFeeder.GetValue(TimeUnits.Minute)), TimeUnits.Minute);
        public Amount TotalStarvedTimeByOperator => new Amount(Batches.Sum(b => b.StarvedTimeByOperator.GetValue(TimeUnits.Minute)), TimeUnits.Minute);
        public Amount TotalStarvedTimeByWashoutPump => new Amount(Batches.Sum(b => b.StarvedTimeByWashoutPump.GetValue(TimeUnits.Minute)), TimeUnits.Minute);
        public Amount TotalStarvedTimeTransferingToWip => new Amount(Batches.Sum(b => b.StarvedTimeTransferingToWip.GetValue(TimeUnits.Minute)), TimeUnits.Minute);


        public List<BatchReport> Batches { get; set; } = new List<BatchReport>();
    }
    public class BatchReport
    {
        private readonly Dictionary<MixerTimeType, Amount> _timeAccumulators = new();
        private readonly ProcessMixer _mixer;
        public BatchReport(ProcessMixer mixer)
        {
            BatchId = Guid.NewGuid();
            _mixer = mixer;
            MixerName = mixer.Name;
            foreach (MixerTimeType type in Enum.GetValues<MixerTimeType>())
            {
                _timeAccumulators[type] = new Amount(0, TimeUnits.Second);
            }
        }
        public Guid BatchId { get; set; }
        public string MixerName { get; set; } = string.Empty;
        public DateTime DateReceivedToInitBatch { get; set; }
        public DateTime? DateRealInitiatedBatch { get; private set; }

        string GetStateLabel()
        {
            if (_mixer.CurrentTransferRequest != null)
            {
                return _mixer.OutletState?.StateLabel ?? string.Empty;
            }
            else if (_mixer.CurrentManufactureOrder != null)
            {
                return _mixer.InletState?.StateLabel ?? string.Empty;
            }
            return string.Empty;
        }
        public string MixerState => GetStateLabel();
        public string RealInitiatedBatchString => DateRealInitiatedBatch?.ToString("G") ?? "Not init yet";
        public void InitRealBatchDate(DateTime date)
        {
            if (DateRealInitiatedBatch.HasValue)
            {
                return;
            }
            DateRealInitiatedBatch = date;
        }
        public void RecordTime(MixerTimeType type)
        {
            _timeAccumulators[type] += _mixer.OneSecond;
        }
        public Amount BatchingTime => _timeAccumulators[MixerTimeType.Batching];
        public Amount WashingTime => _timeAccumulators[MixerTimeType.Washing];
        public Amount TransferingToWipTime => _timeAccumulators[MixerTimeType.TransferToWIP];

        public Amount StarvedTimeInitBatch => _timeAccumulators[MixerTimeType.StarvedByInitByOperator];
        public Amount StarvedTimeByFeeder => _timeAccumulators[MixerTimeType.StarvedByFeeder];
        public Amount StarvedTimeByOperator => _timeAccumulators[MixerTimeType.StarvedByOperator];
        public Amount StarvedTimeByWashoutPump => _timeAccumulators[MixerTimeType.StarvedByWashoutPump];
        public Amount TheroticalBatchTime { get; set; } = new Amount(0, TimeUnits.Minute);
        public Amount RealBatchTime => new Amount(_timeAccumulators.Sum(kv => kv.Value.GetValue(TimeUnits.Minute)), TimeUnits.Minute);
        public Amount StarvedTimeTransferingToWip => _timeAccumulators[MixerTimeType.StarvedByTransferToWIP];
        public Amount TheroicalTransferTimeToWip { get; set; } = new Amount(0, TimeUnits.Minute);
        public Amount BatchSize { get; set; } = new Amount(0, MassUnits.KiloGram);
        public Amount TotalStarvedTime => StarvedTimeInitBatch + StarvedTimeByFeeder +
    StarvedTimeByOperator +
    StarvedTimeByWashoutPump + StarvedTimeTransferingToWip;



    }
}
