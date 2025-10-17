using Simulator.Shared.NuevaSimlationconQwen.Equipments;
using Simulator.Shared.NuevaSimlationconQwen.Equipments.Pumps;
using Simulator.Shared.NuevaSimlationconQwen.Equipments.Tanks;

namespace Simulator.Shared.NuevaSimlationconQwen.StreanJoiners
{
    public class ProcessStreamJoiner : Equipment
    {
        private List<ProcessPump>? _inletPumps;
        private List<ProcessWipTankForLine>? _wipTanks;
        public List<ProcessPump> InletPumps => _inletPumps ??= InletEquipments.OfType<ProcessPump>().ToList();
        public List<ProcessWipTankForLine> WIPTanksAttached => _wipTanks ??= InletPumps.SelectMany(x => x.InletWipTanks).ToList();
    }
}
