using System.Collections.Generic;

namespace Simulator.Shared.Commons
{
    public interface IResult
    {
        List<string> Messages { get; set; }

        string Message {  get; }
        bool Succeeded { get; set; }
    }

    public interface IResult<out T> : IResult
    {
        T Data { get; }
    }
}