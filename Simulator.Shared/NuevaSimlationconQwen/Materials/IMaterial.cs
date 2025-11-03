using Simulator.Shared.Enums.HCEnums.Enums;
using Simulator.Shared.Models.HCs.Materials;
using Simulator.Shared.NuevaSimlationconQwen.Equipments;

namespace Simulator.Shared.NuevaSimlationconQwen.Materials
{
    public interface IMaterial
    {
        Guid Id { get; set; }
        MaterialType MaterialType { get; set; }
        string SAPName { get; set; }
        string CommonName { get; set; }
        string M_Number { get; set; }
        ProductCategory ProductCategory { get; set; }
        List<IEquipment> Equipments { get; set; }
        void AddEquipment(IEquipment equipment);

    }
    public class Material : IMaterial
    {
        public override string ToString()
        {
            return $"{CommonName}";
        }
        public Guid Id { get; set; }
        public MaterialType MaterialType { get; set; } = MaterialType.None;
        public string SAPName { get; set; } = string.Empty;
        public string CommonName { get; set; } = string.Empty;
        public string M_Number { get; set; } = string.Empty;
        public ProductCategory ProductCategory { get; set; } = ProductCategory.None;
        public List<IEquipment> Equipments { get; set; } = new();
        public void AddEquipment(IEquipment equipment)
        {
            if (!Equipments.Any(x => x.Id == equipment.Id))
                Equipments.Add(equipment);
        }


    }
    public interface IMaterialFactory
    {
        bool CanCreate(MaterialType materialType);
        IMaterial Create(MaterialDTO dto);
    }
    public class RecipedMaterialFactory : IMaterialFactory
    {
        public bool CanCreate(MaterialType materialType) =>
            materialType == MaterialType.RawMaterialBackBone ||
            materialType == MaterialType.ProductBackBone;

        public IMaterial Create(MaterialDTO dto)
        {
            var material = new RecipedMaterial
            {
                Id = dto.Id,
                CommonName = dto.CommonName,
                M_Number = dto.M_Number,
                SAPName = dto.SAPName,
                MaterialType = dto.MaterialType,
                ProductCategory = dto.ProductCategory
            };

            // 👇 Aquí agregamos los pasos de receta (igual que en tu código "fea")
            foreach (var step in dto.BackBoneSteps.OrderBy(x => x.Order))
            {
                material.RecipeSteps.Enqueue(new RecipeStep
                {
                    BackBoneStepType = step.BackBoneStepType,
                    Id = step.Id,
                    StepNumber = step.Order,
                    RawMaterialId = step.StepRawMaterial?.Id, // ← Usando null-conditional operator
                    Time = step.Time,
                    Percentage = step.Percentage
                });
            }

            return material;
        }
    }
    public class RawMaterialFactory : IMaterialFactory
    {
        public bool CanCreate(MaterialType materialType) =>
            materialType == MaterialType.RawMaterial;

        public IMaterial Create(MaterialDTO dto)
        {
            return new Material
            {
                Id = dto.Id,
                CommonName = dto.CommonName,
                M_Number = dto.M_Number,
                SAPName = dto.SAPName,
                MaterialType = dto.MaterialType,
                ProductCategory = dto.ProductCategory
            };
        }
    }
    public class MaterialFactoryProvider
    {
        private readonly List<IMaterialFactory> _factories;

        public MaterialFactoryProvider()
        {
            _factories = new List<IMaterialFactory>
        {
            new RecipedMaterialFactory(),
            new RawMaterialFactory()
        };
        }

        public IMaterial CreateMaterial(MaterialDTO dto)
        {
            var factory = _factories.FirstOrDefault(f => f.CanCreate(dto.MaterialType));
            if (factory == null)
                throw new InvalidOperationException($"No factory for material type: {dto.MaterialType}");

            return factory.Create(dto);
        }

        public List<IMaterial> CreateMaterials(List<MaterialDTO> dtos, SimulationMessageService messageService)
        {
            // Validar duplicados (igual que en tanques)
            var duplicateGroups = dtos.GroupBy(d => d.Id)
                                      .Where(g => g.Count() > 1);

            foreach (var group in duplicateGroups)
            {
                var names = string.Join(", ", group.Select(d => $"\"{d.SAPName}\""));
                messageService.AddWarning($"Duplicate materials (same ID): {names}. Only the first will be loaded.", "MaterialFactory");
            }

            var uniqueDtos = dtos.GroupBy(d => d.Id)
                                 .Select(g => g.First())
                                 .ToList();

            return uniqueDtos.Select(CreateMaterial).ToList();
        }
    }
}
