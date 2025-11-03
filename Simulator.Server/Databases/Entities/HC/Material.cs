using Simulator.Server.Databases.Contracts;
using Simulator.Server.Databases.Entities.Equilibrio;
using Simulator.Server.ExtensionsMethods.Validations;
using Simulator.Shared.Enums.HCEnums.Enums;
using Simulator.Shared.Intefaces;
using Simulator.Shared.Models.HCs.BackBoneSteps;
using Simulator.Shared.Models.HCs.Materials;
using Simulator.Shared.NewModels.Compounds;
using System.ComponentModel.DataAnnotations.Schema;
using static MudBlazor.TimeSeriesChartSeries;

namespace Simulator.Server.Databases.Entities.HC
{
    public class Material : Entity, IMapper, IQueryHandler<Material>, IValidationRule<Material>, ICreator<Material>
    {


        public MaterialType MaterialType { get; set; }

        public string M_Number { get; set; } = string.Empty;
        public string SAPName { get; set; } = string.Empty;
        public string CommonName { get; set; } = string.Empty;
        public bool IsForWashing { get; set; } = false;
        public MaterialPhysicState PhysicalState { get; set; } = MaterialPhysicState.None;
        public ProductCategory ProductCategory { get; set; } = ProductCategory.None;
        public FocusFactory FocusFactory { get; set; } = FocusFactory.None;

        public List<BackBoneStep> BackBoneSteps { get; private set; } = new();
        [ForeignKey("RawMaterialId")]
        public List<BackBoneStep> RawMaterials { get; private set; } = new();
        [ForeignKey("MaterialId")]
        public List<SKU> SKUs { get; private set; } = new();
        public BackBoneStep AddBakBoneStep(BackBoneStepDTO stepdto)
        {
            var lastorder = BackBoneSteps.Count == 0 ? 1 : BackBoneSteps.OrderBy(x => x.Order).Last().Order + 1;
            BackBoneStep backBoneStep = new BackBoneStep()
            {
                Id = Guid.NewGuid(),
                MaterialId = Id,
                Order = lastorder,
                BackBoneStepType = stepdto.BackBoneStepType,
                RawMaterialId = stepdto.RawMaterialId,
                Percentage = stepdto.Percentage,
                TimeValue = stepdto.TimeValue,
                TimeUnitName = stepdto.TimeUnitName,
            };
            BackBoneSteps.Add(backBoneStep);
            return backBoneStep;
        }
        public List<MaterialEquipment> ProcessEquipments { get; set; } = new();
        [ForeignKey("BackBoneId")]
        public List<MixerPlanned> MixerPlanneds { get; set; } = new List<MixerPlanned>();


        private double CalculateSumOfPercentage()
        {
            return BackBoneSteps == null || BackBoneSteps.Count == 0 ? 0 :
                Math.Round(BackBoneSteps.Where(x => x.BackBoneStepType == BackBoneStepType.Add).Sum(x => x.Percentage), 2);
        }

        public static Material Create(IDto dto)
        {
            if (dto is MaterialDTO materialdto)
            {
                var material = new Material
                {
                    Id = Guid.NewGuid(),

                };

                foreach (var stepdto in materialdto.BackBoneSteps.OrderBy(x => x.Order))
                {
                    var step = material.AddBakBoneStep(stepdto);

                }
                return material;
            }
            return null!;
        }
        public void MapFromDto(IDto dto)
        {
            switch (dto)
            {
                case MaterialDTO properdto:
                    {
                        FocusFactory = properdto.FocusFactory;
                        M_Number = properdto.M_Number;
                        SAPName = properdto.SAPName;
                        PhysicalState = properdto.PhysicalState;
                        ProductCategory = properdto.ProductCategory;
                        IsForWashing = properdto.IsForWashing;
                        MaterialType = properdto.MaterialType;
                        CommonName = properdto.CommonName;
                    }
                    break;

                default:
                    break;

            }
        }
        public T MapToDto<T>() where T : IDto, new()
        {
            return typeof(T) switch
            {
                _ when typeof(T) == typeof(MaterialDTO) => (T)(object)new MaterialDTO
                {
                    Id = Id,
                    FocusFactory = FocusFactory,
                    M_Number = M_Number,
                    SAPName = SAPName,
                    PhysicalState = PhysicalState,
                    ProductCategory = ProductCategory,
                    IsForWashing = IsForWashing,
                    MaterialType = MaterialType,
                    CommonName = CommonName,
                    SumOfPercentage = CalculateSumOfPercentage()
                },
                _ when typeof(T) == typeof(RawMaterialDto) => (T)(object)new RawMaterialDto
                {
                    Id = Id,
                    FocusFactory = FocusFactory,
                    M_Number = M_Number,
                    SAPName = SAPName,
                    PhysicalState = PhysicalState,
                    ProductCategory = ProductCategory,
                    IsForWashing = IsForWashing,
                    MaterialType = MaterialType,
                    CommonName = CommonName,

                },
                _ when typeof(T) == typeof(BackBoneDto) => (T)(object)new BackBoneDto
                {
                    Id = Id,
                    FocusFactory = FocusFactory,
                    M_Number = M_Number,
                    SAPName = SAPName,
                    PhysicalState = PhysicalState,
                    ProductCategory = ProductCategory,
                    IsForWashing = IsForWashing,
                    MaterialType = MaterialType,
                    CommonName = CommonName,
                    SumOfPercentage = CalculateSumOfPercentage(),
                    BackBoneSteps = BackBoneSteps == null || BackBoneSteps.Count == 0 ? new() : BackBoneSteps.Select(x => x.MapToDto<BackBoneStepDTO>()).ToList(),

                },
                _ when typeof(T) == typeof(ProductBackBoneDto) => (T)(object)new ProductBackBoneDto
                {
                    Id = Id,
                    FocusFactory = FocusFactory,
                    M_Number = M_Number,
                    SAPName = SAPName,
                    PhysicalState = PhysicalState,
                    ProductCategory = ProductCategory,
                    IsForWashing = IsForWashing,
                    MaterialType = MaterialType,
                    CommonName = CommonName,
                    SumOfPercentage = CalculateSumOfPercentage(),
                    BackBoneSteps = BackBoneSteps == null || BackBoneSteps.Count == 0 ? new() : BackBoneSteps.Select(x => x.MapToDto<BackBoneStepDTO>()).ToList(),
                },
                _ => default(T)!
            };
        }
        static Func<IQueryable<Material>, IIncludableQueryable<Material, object>> IQueryHandler<Material>.GetIncludesBy(IDto dto)
        {
            if (dto is RawMaterialDto || dto is ProductBackBoneDto || dto is BackBoneDto||dto is CompletedMaterialDTO)
            {
                return x => x
                 .Include(y => y.BackBoneSteps).ThenInclude(x => x.RawMaterial!);
            }

            return null!;

        }
        static Expression<Func<Material, object>> IQueryHandler<Material>.GetOrderBy(IDto dto)
        {
            if (dto is RawMaterialDto)
            {
                return f => f.CommonName;
            }
            else if (dto is ProductBackBoneDto)
            {
                return f => f.CommonName;
            }
            else if (dto is BackBoneDto)
            {
                return f => f.CommonName;
            }
            else if (dto is MaterialDTO)
            {
                return f => f.MaterialType;
            }
            return null!;

        }
        static Expression<Func<Material, bool>> IQueryHandler<Material>.GetFilterBy(IDto dto)
        {
            if (dto is RawMaterialDto rawmaterial)
            {
                return x => (x.MaterialType == MaterialType.RawMaterial || x.MaterialType == MaterialType.RawMaterialBackBone)
                && (rawmaterial.FocusFactory != FocusFactory.None ? x.FocusFactory == rawmaterial.FocusFactory : true);
            }
            else if (dto is ProductBackBoneDto productbackbone)
            {
                return x => (x.MaterialType == MaterialType.ProductBackBone)
                   && (productbackbone.FocusFactory != FocusFactory.None ? x.FocusFactory == productbackbone.FocusFactory : true);

            }
            else if (dto is BackBoneDto backbone)
            {
                return x => (x.MaterialType == MaterialType.ProductBackBone || x.MaterialType == MaterialType.RawMaterialBackBone)
                  && (backbone.FocusFactory != FocusFactory.None ? x.FocusFactory == backbone.FocusFactory : true);
            }
            else if (dto is MaterialDTO material)
            {
                return x => material.FocusFactory != FocusFactory.None ? x.FocusFactory == material.FocusFactory : true;
            }

            return null!;

        }
        static Expression<Func<Material, bool>> IValidationRule<Material>.GetExistCriteria(IDto dto, string validationKey)
        {
            return dto switch
            {
                MaterialDTO materialDto => validationKey switch
                {
                    nameof(MaterialDTO.SAPName) => materialDto.BuildStringCriteria<Material, MaterialDTO>(
                        nameof(MaterialDTO.SAPName),
                        nameof(Material.SAPName)),
                    nameof(MaterialDTO.M_Number) => materialDto.BuildStringCriteria<Material, MaterialDTO>(
                        nameof(MaterialDTO.M_Number),
                        nameof(Material.M_Number)),
                    nameof(MaterialDTO.CommonName) => materialDto.BuildStringCriteria<Material, MaterialDTO>(
                   nameof(MaterialDTO.CommonName),
                   nameof(Material.CommonName)),
                    _ => x => false
                },



                _ => x => false
            };
        }
        static Expression<Func<Material, bool>> IValidationRule<Material>.GetIdCriteria(IDto dto)
        {
            return dto switch
            {


                _ => x => true // DTO no reconocido → sin filtro
            };
        }

    }

}

