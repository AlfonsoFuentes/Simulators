namespace Simulator.Shared.NuevaSimlationconQwen
{
    public class SimulationMessage
    {
        public DateTime Timestamp { get; set; }
        public string Level { get; set; } = string.Empty; // "Warning", "Error", "Info"
        public string Message { get; set; } = string.Empty;
        public string Source { get; set; } = string.Empty; // "TankFactory", "MixerFactory", etc.
    }
    public class SimulationMessageService
    {
        private readonly List<SimulationMessage> _messages = new();

        public void AddWarning(string message, string source = "General")
        {
            _messages.Add(new SimulationMessage
            {
                Timestamp = DateTime.Now,
                Level = "Warning",
                Message = message,
                Source = source
            });
        }

        public IReadOnlyList<SimulationMessage> GetMessages() => _messages.AsReadOnly();
    }
}
