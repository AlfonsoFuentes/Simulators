using Caldera;

namespace Simulator.Client.Steam;
public partial class WaterPage
{
    [CascadingParameter]
    public MassEnergyBalance balance { get; set; } = new();
}
