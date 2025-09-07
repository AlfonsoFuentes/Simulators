using Simulator.Shared.Commons.FileResults.Generics.Request;

namespace Simulator.Shared.Commons.FileResults.Generics.Records
{
    public interface IGetById : IRequest
    {
        Guid Id { get; }

    }
    public interface IGetAll : IRequest
    {
       
    }
}
