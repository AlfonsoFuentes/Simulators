using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Simulator.Server.Databases.Contracts;
using Simulator.Server.ExtensionsMethods.Validations;
using Simulator.Shared.Enums.HCEnums.Enums;
using Simulator.Shared.Intefaces;
using Simulator.Shared.Models.HCs.BackBoneSteps;
using Simulator.Shared.Models.HCs.Materials;
using Simulator.Shared.Models.HCs.SKUs;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.AccessControl;
using static Simulator.Shared.StaticClasses.StaticClass;

namespace Simulator.Server.Databases.Entities.HC
{
    public class SKU : Entity, IMapper, IQueryHandler<SKU>, IValidationRule<SKU>, ICreator<SKU>
    {
       

        public FocusFactory FocusFactory { get; set; } = FocusFactory.None;
        public List<SKULine> SKULines { get; set; } = new();

        public string SkuCode { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public ProductCategory ProductCategory { get; set; } = ProductCategory.None;
        public Material Material { get; set; } = null!;
        public Guid MaterialId { get; set; }
        public PackageType PackageType { get; set; }
        public int EA_Case { get; set; }
        public double SizeValue { get; set; }
        public string SizeUnit { get; set; } = string.Empty;
        public double WeigthValue { get; set; }
        public string WeigthUnit { get; set; } = string.Empty;

        [ForeignKey("SKUId")]
        public List<PlannedSKU> PlannedSKUs { get; set; } = new();

        public static SKU Create(IDto dto)
        {
            if (dto is SKUDTO mappeddto)
            {
                var entity = new SKU
                {
                    Id = Guid.NewGuid(),

                };

               
                return entity;
            }
            return null!;
        }
        public void MapFromDto(IDto dto)
        {
            switch (dto)
            {
                case SKUDTO request:
                    {
                        FocusFactory = request.FocusFactory;
                        SkuCode = request.SkuCode;
                        SizeValue = request.SizeValue;
                        SizeUnit = request.SizeUnitName;
                        WeigthUnit = request.WeigthUnitName;
                        WeigthValue = request.WeigthValue;
                        EA_Case = request.EA_Case;
                        ProductCategory = request.ProductCategory;
                        MaterialId = request.BackBone!.Id;
                        PackageType = request.PackageType;
                        Name = request.Name;
                        FocusFactory = request.FocusFactory;

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
                _ when typeof(T) == typeof(SKUDTO) => (T)(object)new SKUDTO
                {
                    Id = Id,
                    Name = Name,
                    EA_Case = EA_Case,
                    ProductCategory = ProductCategory,
                    BackBone = Material == null ? null! : Material.MapToDto<MaterialDTO>(),
                    PackageType = PackageType,
                    SizeValue = SizeValue,
                    SizeUnitName = SizeUnit,
                    WeigthValue = WeigthValue,
                    WeigthUnitName = WeigthUnit,

                    SkuCode = SkuCode,
                    FocusFactory = FocusFactory,
                    Order = Order,

                },
               
              
                _ => default(T)!
            };
        }
        static Func<IQueryable<SKU>, IIncludableQueryable<SKU, object>> IQueryHandler<SKU>.GetIncludesBy(IDto dto)
        {
            if (dto is SKUDTO )
            {
                return x => x.Include(y => y.Material);
            }

            return null!;

        }
        static Expression<Func<SKU, object>> IQueryHandler<SKU>.GetOrderBy(IDto dto)
        {
            if (dto is SKUDTO)
            {
                return f => f.Name;
            }
          
            return null!;

        }
        static Expression<Func<SKU, bool>> IQueryHandler<SKU>.GetFilterBy(IDto dto)
        {
            return dto switch
            {
                SKUDTO material =>

                x =>  (material.FocusFactory != FocusFactory.None ? x.FocusFactory == material.FocusFactory : true),
               
                _ => null!
            };
        }
        static Expression<Func<SKU, bool>> IValidationRule<SKU>.GetExistCriteria(IDto dto, string validationKey)
        {
            return dto switch
            {
                SKUDTO mappedto => validationKey switch
                {
                    nameof(SKUDTO.SkuCode) => mappedto.BuildStringCriteria<SKU, SKUDTO>(
                        nameof(SKUDTO.SkuCode),
                        nameof(SKU.SkuCode)),
                    nameof(SKUDTO.Name) => mappedto.BuildStringCriteria<SKU, SKUDTO>(
                        nameof(SKUDTO.Name),
                        nameof(SKU.Name)),
                   // nameof(SKUDTO.CommonName) => mappedto.BuildStringCriteria<SKU, SKUDTO>(
                   //nameof(SKUDTO.CommonName),
                   //nameof(SKU.CommonName)),
                    _ => x => false
                },



                _ => x => false
            };
        }
        static Expression<Func<SKU, bool>> IValidationRule<SKU>.GetIdCriteria(IDto dto)
        {
            return dto switch
            {


                _ => x => true // DTO no reconocido → sin filtro
            };
        }

    }

}

