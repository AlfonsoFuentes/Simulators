namespace Simulator.Shared.Commons.FileResults.Generics.Reponses
{
    public interface IExportResponse
    {

    }
    public record ExportResponse<T>(T Data) where T : IExportResponse;

}
