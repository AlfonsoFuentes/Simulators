using Simulator.Shared.Commons;
using Simulator.Shared.Commons.FileResults;
using System.Data;

namespace Simulator.Server.Interfaces.Storage
{
    public interface IExcelService
    {
        Task<IResult<FileResult>> ExportAsync<TData>(IQueryable<TData> data, string sheetName = "Sheet1");
        Task<string> ExportAsync<TData>(IEnumerable<TData> data
            , Dictionary<string, Func<TData, object>> mappers
            , string sheetName = "Sheet1");

        Task<IResult<IEnumerable<TEntity>>> ImportAsync<TEntity>(Stream data
            , Dictionary<string, Func<DataRow, TEntity, object>> mappers
            , string sheetName = "Sheet1");
    }

}