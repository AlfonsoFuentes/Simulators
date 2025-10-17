using Caldera;

namespace Simulator.Client.Steam;
public partial class SummaryPage
{
    [CascadingParameter]
    public MassEnergyBalance balance { get; set; } = new();
}
