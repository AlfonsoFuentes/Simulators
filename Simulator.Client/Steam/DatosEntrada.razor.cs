using Caldera;

namespace Simulator.Client.Steam;
public partial class DatosEntrada
{
    [CascadingParameter]
    public MassEnergyBalance balance { get; set; } = new();
}
