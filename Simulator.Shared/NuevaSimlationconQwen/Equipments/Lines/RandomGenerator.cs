namespace Simulator.Shared.NuevaSimlationconQwen.Equipments.Lines
{
    public interface IRandomGenerator
    {
        int Next(int maxValue);
    }
    public class RandomGenerator : IRandomGenerator
    {
        private readonly Random _random = new Random();
        public int Next(int maxValue) => _random.Next(maxValue);
    }
}
