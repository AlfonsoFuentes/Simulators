using Simulator.Shared.NuevaSimlationconQwen.Equipments;
using Simulator.Shared.NuevaSimlationconQwen.Materials;
using System.Collections.ObjectModel;

namespace Simulator.Shared.NuevaSimlationconQwen
{
    public enum ManufacturingReportType
    {
        MaterialConnected,
        MaterialNotConnected,
        ProductCanProduce,
        ProductCannotProduce,
        // ... otros tipos en el futuro
    }
    public class ManufacturingReport
    {
        public string EquipmentName => Equipment?.Name ?? string.Empty;
        public Guid EquipmentId => Equipment?.Id ?? Guid.Empty;
        public Guid MaterialId => Material?.Id ?? Guid.Empty;
        public string MaterialName => Material?.CommonName ?? string.Empty;
        public IMaterial? Material { get; set; } = null!;
        public IEquipment? Equipment { get; set; } = null!;
        public Amount Capacity { get; set; } = new Amount(0, MassUnits.KiloGram);
        public Amount BatchTime { get; set; } = new Amount(0, TimeUnits.Minute);
        public Amount TransferTime { get; set; } = new Amount(0, TimeUnits.Minute);
        public Amount TotalTime => BatchTime + TransferTime;

    }
    public class ManufacturingAnalysisResult
    {
        private readonly Dictionary<ManufacturingReportType, List<ManufacturingReport>> _reports;

        public ManufacturingAnalysisResult()
        {
            _reports = new Dictionary<ManufacturingReportType, List<ManufacturingReport>>
        {
            { ManufacturingReportType.MaterialConnected, new List<ManufacturingReport>() },
            { ManufacturingReportType.MaterialNotConnected, new List<ManufacturingReport>() },
            { ManufacturingReportType.ProductCanProduce, new List<ManufacturingReport>() },
            { ManufacturingReportType.ProductCannotProduce, new List<ManufacturingReport>() }
        };
        }
        public IReadOnlyDictionary<ManufacturingReportType, IReadOnlyList<ManufacturingReport>> Reports
        {
            get
            {
                var readOnlyReports = _reports.ToDictionary(
                    x => x.Key,
                    x => (IReadOnlyList<ManufacturingReport>)x.Value.AsReadOnly()
                );
                return new ReadOnlyDictionary<ManufacturingReportType, IReadOnlyList<ManufacturingReport>>(readOnlyReports);
            }
        }


        public void AddReport(ManufacturingReportType type, IMaterial material, IEquipment equipment)
        {
            if (material == null || equipment == null) return;

            // Evitar duplicados
            if (_reports[type].Any(x => x.MaterialId == material.Id && x.EquipmentId == equipment.Id))
                return;

            _reports[type].Add(new ManufacturingReport
            {
                Material = material,
                Equipment = equipment

            });
        }
        public void AddReport(ManufacturingReportType type, IMaterial material, IEquipment equipment, Amount Capacity, Amount BatchTime, Amount TransferTime)
        {
            if (material == null || equipment == null) return;

            // Evitar duplicados
            if (_reports[type].Any(x => x.MaterialId == material.Id && x.EquipmentId == equipment.Id))
                return;

            _reports[type].Add(new ManufacturingReport
            {
                Material = material,
                Equipment = equipment,
                Capacity = Capacity,
                TransferTime = TransferTime,
                BatchTime = BatchTime

            });
        }

        // Métodos de conveniencia (opcionales)
        public IReadOnlyList<ManufacturingReport> GetConnectedMaterials() => _reports[ManufacturingReportType.MaterialConnected];
        public IReadOnlyList<ManufacturingReport> GetNotConnectedMaterials() => _reports[ManufacturingReportType.MaterialNotConnected];
        public IReadOnlyList<ManufacturingReport> GetProductsThatCanProduce() => _reports[ManufacturingReportType.ProductCanProduce];
        public IReadOnlyList<ManufacturingReport> GetProductsThatCannotProduce() => _reports[ManufacturingReportType.ProductCannotProduce];
    }
}
