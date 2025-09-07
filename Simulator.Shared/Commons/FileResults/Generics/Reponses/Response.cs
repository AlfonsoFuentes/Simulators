using Simulator.Shared.Commons.BaseResponses;

namespace Simulator.Shared.Commons.FileResults.Generics.Reponses
{
    public interface IResponseAll
    {

    }
    public interface IUpdateStateResponse
    {
        string EndPointName { get; }
    }
    public interface IResponse
    {
        Guid Id { get; set; }
        string Name { get; set; }
       
    }
    public class Response<T> : BaseResponse where T : IResponse
    {
        public T Value { get; set; } = default!;


    }
}
