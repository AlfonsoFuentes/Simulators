namespace Simulator.Shared.NuevaSimlationconQwen.Reports
{
    public enum ReportColumn
    {
        Column1_OperatorsAndRawMaterialTanks,
        Column2_SkidsAndMixers,
        Column3_WipTanks,
        Column4_Lines
    }

    public enum ReportPriorityInColumn
    {
        High, // ← Arriba en la columna
        Low   // ← Abajo en la columna
    }
    public interface ILiveReportable
    {
        ReportColumn ReportColumn { get; }
        ReportPriorityInColumn ReportPriority { get; }
        List<LiveReportItem> GetLiveReportItems();

    }
    public class LiveReportItem
    {
        public string Label { get; set; } = string.Empty; // ← Leyenda (ej: "State", "Level")
        public string Value { get; set; } = string.Empty;// ← Valor actual como string (con unidades)
        public ReportStyle Style { get; set; } = new ReportStyle(); // ← Estilo para la UI
    }
    public class ReportStyle
    {
        public string Color { get; set; } = "Black";
        public string FontEmphasis { get; set; } = "Normal";
        public string BackgroundColor { get; set; } = "Transparent";
    }
}
