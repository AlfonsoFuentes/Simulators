using Simulator.Shared.Commons.FileResults.ExportFiles;

namespace Simulator.Shared.Commons.FileResults.Generics.Exports
{
    public record GetAllExport<T>(ExportFileType FileType, List<T> query) where T : class;
}
