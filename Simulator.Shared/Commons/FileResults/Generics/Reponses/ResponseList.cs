namespace Simulator.Shared.Commons.FileResults.Generics.Reponses
{
    public class ResponseList<T> where T : IResponse
    {
        public List<T> Items { get; set; } = new();
    }
}
