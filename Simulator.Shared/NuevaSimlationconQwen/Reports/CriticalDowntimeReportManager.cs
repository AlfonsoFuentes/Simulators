using Simulator.Shared.Enums.HCEnums.Enums;
using Simulator.Shared.NuevaSimlationconQwen.Equipments;

namespace Simulator.Shared.NuevaSimlationconQwen.Reports
{
    public class CriticalDowntimeReportManager
    {
        private readonly GeneralSimulation _simulation = null!;
        public CriticalDowntimeReportManager(GeneralSimulation simulation)
        {
            _simulation = simulation;
        }
        private readonly List<CriticalDowntimeReport> _reports = new();
        private readonly Dictionary<Guid, CriticalDowntimeReport> _activeReports = new();

        public void StartReport(IEquipment generator, IEquipment source, string reason, string description)
        {
            var report = new CriticalDowntimeReport(_simulation,generator, source, reason, description);
            _activeReports[report.Id] = report; // ← Usa Id como clave
            _reports.Add(report);
            generator.ActiveDowntimeReportId= report.Id;
        }

        public void EndReport(Guid reportId)
        {
            if (_activeReports.TryGetValue(reportId, out var report))
            {
                report.End(_simulation.CurrentDate);
                _activeReports.Remove(reportId);
            }
        }

        public List<CriticalDowntimeReport> GetActiveReports() => _activeReports.Values.ToList();
        public List<CriticalDowntimeReport> GetAllReports() => _reports;
        public List<CriticalDowntimeReport> GetReportsByEquipmentType(string equipmentType) =>
            _reports.Where(r => r.Generator.EquipmentType.ToString() == equipmentType).ToList();

        // ✅ Agrupado por tipo de equipo, ordenado por nombre de equipo
        // ✅ Agrupado por tipo de equipo, ordenado por nombre
        public Dictionary<string, List<CriticalDowntimeReport>> GetReportsGroupedByEquipmentType()
        {
            return _reports
                .GroupBy(r => GetEquipmentCategory(r.Generator.EquipmentType))
                .OrderBy(g => GetCategoryOrder(g.Key)) // ← Orden: Lines, Mixers, Tanks, Pumps
                .ToDictionary(
                    g => g.Key,
                    g => g.OrderBy(r => r.Generator.Name).ToList() // ← Ordenado por nombre
                );
        }

        private string GetEquipmentCategory(ProccesEquipmentType equipmentType) => equipmentType switch
        {
            ProccesEquipmentType.Line => "Lines",
            ProccesEquipmentType.Mixer or ProccesEquipmentType.ContinuousSystem => "Mixers & Skids",
            ProccesEquipmentType.Tank => "Tanks",
            ProccesEquipmentType.Pump => "Pumps",
            ProccesEquipmentType.Operator => "Operators",
            _ => "Other"
        };

        private int GetCategoryOrder(string category) => category switch
        {
            "Lines" => 1,
            "Mixers & Skids" => 2,
            "Tanks" => 3,
            "Pumps" => 4,
            "Operators" => 5,
            _ => 6
        };
    }
}
