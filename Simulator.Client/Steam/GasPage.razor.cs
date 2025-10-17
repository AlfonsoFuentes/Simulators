using Caldera;

namespace Simulator.Client.Steam;
public partial class GasPage
{
    [CascadingParameter]
    public MassEnergyBalance balance { get; set; } = new();

}
