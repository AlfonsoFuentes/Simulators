using Simulator.Shared.Enums.HCEnums.Enums;
using Simulator.Shared.Models.HCs.Tanks;

namespace Simulator.Shared.NuevaSimlationconQwen.Equipments.Tanks
{
    public interface ITankFactory
    {
        bool CanCreate(FluidToStorage fluidType);
        ProcessBaseTank Create(TankDTO dto);
    }
    public class RawMaterialTankFactory : ITankFactory
    {
        public bool CanCreate(FluidToStorage fluidType) => fluidType == FluidToStorage.RawMaterial;

        public ProcessBaseTank Create(TankDTO dto)
        {
            return new ProcessRawMaterialTank
            {
                Id = dto.Id,
                Name = dto.Name,
                EquipmentType = ProccesEquipmentType.Tank,
                Capacity = dto.Capacity,
                HiLevel = dto.MaxLevel,
                LoLevel = dto.MinLevel,
                LoLolevel = dto.LoLoLevel,
                InitialLevel = dto.InitialLevel
            };
        }
    }
    public class ProcessWipTankForLineFactory : ITankFactory
    {
        public bool CanCreate(FluidToStorage fluidType) => fluidType == FluidToStorage.ProductBackBone;

        public ProcessBaseTank Create(TankDTO dto)
        {
            return new ProcessWipTankForLine
            {
                Id = dto.Id,
                Name = dto.Name,
                EquipmentType = ProccesEquipmentType.Tank,
                Capacity = dto.Capacity,
                HiLevel = dto.MaxLevel,
                LoLevel = dto.MinLevel,
                LoLolevel = dto.LoLoLevel,
                InitialLevel = dto.InitialLevel,
             
            };
        }
    }
    public class RecipedRawMaterialTankFactory : ITankFactory
    {
        public bool CanCreate(FluidToStorage fluidType) =>
           
            fluidType == FluidToStorage.RawMaterialBackBone;

        public ProcessBaseTank Create(TankDTO dto)
        {
            return new ProcessRecipedTank
            {
                Id = dto.Id,
                Name = dto.Name,
                EquipmentType = ProccesEquipmentType.Tank,
                Capacity = dto.Capacity,
                HiLevel = dto.MaxLevel,
                LoLevel = dto.MinLevel,
                LoLolevel = dto.LoLoLevel,
                InitialLevel = dto.InitialLevel
            };
        }
    }
    public class TankFactoryProvider
    {
        private readonly List<ITankFactory> _factories;

        public TankFactoryProvider()
        {
            _factories = new List<ITankFactory>
        {
            new RawMaterialTankFactory(),
            new RecipedRawMaterialTankFactory(),
            new ProcessWipTankForLineFactory(),
        };
        }

        public ProcessBaseTank CreateTank(TankDTO dto)
        {
            var factory = _factories.FirstOrDefault(f => f.CanCreate(dto.FluidStorage));
            if (factory == null)
                throw new InvalidOperationException($"No factory for fluid type: {dto.FluidStorage}");
            return factory.Create(dto);
        }

        // 👇 Nuevo método: crea todos los tanques de una vez
        public List<ProcessBaseTank> CreateTanks(List<TankDTO> dtos, SimulationMessageService messageService)
        {
            // Solo validamos duplicados (asumimos que otros datos ya fueron validados por FluentValidation)
            var duplicateGroups = dtos.GroupBy(d => d.Id)
                                      .Where(g => g.Count() > 1);

            foreach (var group in duplicateGroups)
            {
                var names = string.Join(", ", group.Select(d => $"\"{d.Name}\""));
                messageService.AddWarning($"Duplicate tanks (same ID): {names}. Only the first will be loaded.", "TankFactory");
            }

            // Filtrar duplicados (mantener el primero)
            var uniqueDtos = dtos.GroupBy(d => d.Id)
                                 .Select(g => g.First())
                                 .ToList();

            // Crear tanques (sin validaciones adicionales)
            return uniqueDtos.Select(CreateTank).ToList();
        }
    }

}
