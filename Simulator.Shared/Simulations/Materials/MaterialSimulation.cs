using Simulator.Shared.Enums.HCEnums.Enums;
using Simulator.Shared.Simulations.Mixers;
using Simulator.Shared.Simulations.Operators;
using Simulator.Shared.Simulations.Pumps;
using Simulator.Shared.Simulations.Skids;
using Simulator.Shared.Simulations.Tanks;

namespace Simulator.Shared.Simulations.Materials
{
    public class MaterialSimulation
    {
        protected NewSimulation Simulation { get; set; } = null!;
        public MaterialSimulation(NewSimulation simulation)
        {
            Simulation = simulation;
        }
        MaterialSimulation currentMaterial => Simulation.MaterialSimulations.FirstOrDefault(x => x.Id == Id)!;
        public override string ToString() => $"{M_Number} {CommonName}";

        public MaterialType MaterialType { get; set; }
        public string SAPName { get; set; } = string.Empty;
        public string CommonName { get; set; } = string.Empty;
        public Guid Id { get; set; }
        public string M_Number { get; set; } = string.Empty;
        public MaterialType Type { get; set; } = MaterialType.None;
        public MaterialPhysicState PhysicalState { get; set; } = MaterialPhysicState.None;
        public string M_NumberCommonName => $"{M_Number} {SAPName}";
        public ProductCategory ProductCategory { get; set; } = ProductCategory.None;
        public bool IsForWashing { get; set; } = false;
        public List<NewBaseEquipment> ProcessEquipments { get; private set; } = new();
        List<BasePump> Pumps => currentMaterial == null ? new() : currentMaterial.ProcessEquipments.OfType<BasePump>().ToList();
        List<BaseOperator> Operators => currentMaterial == null ? new() : currentMaterial.ProcessEquipments.OfType<BaseOperator>().ToList();
        public List<BaseMixer> Mixers => currentMaterial == null ? new() : currentMaterial.ProcessEquipments.OfType<BaseMixer>().ToList();
        public List<BaseSKID> SKIDs => currentMaterial == null ? new() : currentMaterial.ProcessEquipments.OfType<BaseSKID>().ToList();
        public List<BaseTank> Tanks => currentMaterial == null ? new() : currentMaterial.ProcessEquipments.OfType<BaseTank>().ToList();

        bool IsTankAvailables => Tanks.Count == 0 ? false : Tanks.Any(x => x.ProcessOutletEquipments.Any(x => x.ProcessOutletEquipments.Count == 0));
        bool IsTanksHasThisMaterial => Tanks.Count != 0;
        WIPForProductBackBone WIPForProductBackBone => !IsTanksHasThisMaterial ? null! : (WIPForProductBackBone)Tanks.First()!;


        public WIPForProductBackBone GetTankAvailable(NewBaseEquipment equipment)
        {
            if (!IsTanksHasThisMaterial) return null!;

            if (IsTankAvailables)
            {
                if (WIPForProductBackBone.IsTankLoLevel)
                {
                    WIPForProductBackBone.PutEquipmentInQueue(equipment);

                    return null!;
                }
                if (!WIPForProductBackBone.IsEquipmentHasQueue)
                    return WIPForProductBackBone;

                if (WIPForProductBackBone.GetFirstQueue.Id == equipment.Id)
                {
                    WIPForProductBackBone.RemoveEquipmentFromQueue();
                    return WIPForProductBackBone;
                }

            }

            WIPForProductBackBone.PutEquipmentInQueue(equipment);


            return null!;
        }
        public List<BaseTank> TanksNoAvailables => Tanks.Where(x => x.ConnectedOutletEquipments.Any(x => x.ConnectedOutletEquipments.Count > 0)).ToList();


        public List<BaseMixer> MixerAvailables => Mixers.Where(x => x.ProcessOutletEquipments.Any(x => x.ProcessOutletEquipments.Count == 0)).ToList();

        public bool IsMixerAvailableForWIPTanks(BaseTank tank)
        {
            if (MixerAvailables.Count == 0) return false;

            var result = MixerAvailables.Any(x =>
            x.ConnectedOutletEquipments.Any(x =>
                          x.ConnectedOutletEquipments.Any(x => x.Id == tank.Id)));

            return result;
        }



        public List<BasePump> RawMaterialPumps => Pumps.Where(x => x.IsForWashing == false).ToList();
        public List<BasePump> WashoutPumps => Pumps.Where(x => x.IsForWashing == true).ToList();

        public bool IsSingleRawMaterialPump => RawMaterialPumps.Count == 1;
        public bool IsSingleRawMaterialPumpAvailable => RawMaterialPumps.Count == 1 ? RawMaterialPumps.Any(x => x.ProcessOutletEquipments.Count == 0) : false;
        public BasePump MaterialPump => IsSingleRawMaterialPumpAvailable ? RawMaterialPumps.First() : null!;
        public bool IsSingleWashoutPump => WashoutPumps.Count == 1;

        public BasePump? GetRawMaterialPumpByDestinationId(Guid DestinationId)
        {
            return  Pumps.FirstOrDefault(x => x.ConnectedOutletEquipments.Any(x => x.Id == DestinationId));
            
        }
        public BaseOperator? GetRawMaterialOperatorByDestinationId(Guid DestinationId)
        {
            return Operators.FirstOrDefault(x => x.ConnectedOutletEquipments.Any(x => x.Id == DestinationId));

        }

        public BasePump WashoutPump => IsSingleWashoutPump ? WashoutPumps[0] : null!;
        public void AddProcessEquipment(NewBaseEquipment equip)
        {
            if (!ProcessEquipments.Contains(equip))
            {
                ProcessEquipments.Add(equip);
            }
        }



        public BaseMixer GetMixerAvailableForWIP(BaseTank tank)
        {
            var mixer = MixerAvailables.FirstOrDefault(x => x.ConnectedOutletEquipments.Any(x => x.ConnectedOutletEquipments.Any(x => x.Id == tank.Id)));

            return mixer!;

        }
        public BaseMixer GetMixerForWIP(BaseTank tank)
        {
            var mixer = Mixers.FirstOrDefault(x => x.ConnectedOutletEquipments.Any(x => x.ConnectedOutletEquipments.Any(x => x.Id == tank.Id)));

            return mixer!;

        }
    }
}
