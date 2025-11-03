using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using Simulator.Server.Databases.Contracts;
using Simulator.Shared.Enums.HCEnums.Enums;
using Simulator.Shared.Intefaces;
using Simulator.Shared.Models.HCs.Materials;
using Simulator.Shared.Models.HCs.Washouts;

namespace Simulator.Server.Databases.Entities.HC
{
    public class Washout : Entity, IMapper, IQueryHandler<Washout>, IValidationRule<Washout>, ICreator<Washout>
    {
        public static Washout Create() => new()
        {
            Id = Guid.NewGuid(),


        };

        public FocusFactory FocusFactory { get; set; } = FocusFactory.None;
        public ProductCategory ProductCategoryCurrent { get; set; } = ProductCategory.None;
        public ProductCategory ProductCategoryNext { get; set; } = ProductCategory.None;
        public double MixerWashoutTimeValue { get; set; }
        public string MixerWashoutTimeUnit { get; set; } = string.Empty;

        public double LineWashoutTimeValue { get; set; }
        public string LineWashoutTimeUnit { get; set; } = string.Empty;
        public static Washout Create(IDto dto)
        {
            if (dto is WashoutDTO mappeddto)
            {
                var entity = new Washout
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
                case WashoutDTO request:
                    {
                        ProductCategoryNext = request.ProductCategoryNext;
                        ProductCategoryCurrent = request.ProductCategoryCurrent;
                        LineWashoutTimeUnit = request.LineWashoutUnitName;
                        LineWashoutTimeValue = request.LineWashoutValue;
                        MixerWashoutTimeUnit = request.MixerWashoutUnitName;
                        MixerWashoutTimeValue = request.MixerWashoutValue;
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
                _ when typeof(T) == typeof(WashoutDTO) => (T)(object)new WashoutDTO
                {
                    Id = Id,
                    LineWashoutValue = LineWashoutTimeValue,
                    LineWashoutUnitName = LineWashoutTimeUnit,
                    MixerWashoutValue = MixerWashoutTimeValue,
                    MixerWashoutUnitName = MixerWashoutTimeUnit,
                    ProductCategoryCurrent = ProductCategoryCurrent,
                    ProductCategoryNext = ProductCategoryNext,
                    FocusFactory = FocusFactory,

                    Order = Order,

                },


                _ => default(T)!
            };
        }
        static Func<IQueryable<Washout>, IIncludableQueryable<Washout, object>> IQueryHandler<Washout>.GetIncludesBy(IDto dto)
        {
            if (dto is WashoutDTO)
            {
                //return x => x.Include(y => y.Material);
            }

            return null!;

        }
        static Expression<Func<Washout, object>> IQueryHandler<Washout>.GetOrderBy(IDto dto)
        {
            if (dto is WashoutDTO)
            {
                return f => f.ProductCategoryCurrent;
            }

            return null!;

        }
        static Expression<Func<Washout, bool>> IQueryHandler<Washout>.GetFilterBy(IDto dto)
        {
            return dto switch
            {
                WashoutDTO material =>

                x => (material.FocusFactory != FocusFactory.None ? x.FocusFactory == material.FocusFactory : true),

                _ => null!
            };
        }
        static Expression<Func<Washout, bool>> IValidationRule<Washout>.GetExistCriteria(IDto dto, string validationKey)
        {
            return dto switch
            {
                WashoutDTO washoutDto => validationKey switch
                {
                    WashoutDTO.ProductCategoryCombination =>
                        x => x.ProductCategoryCurrent == washoutDto.ProductCategoryCurrent &&
                             x.ProductCategoryNext == washoutDto.ProductCategoryNext &&
                             (washoutDto.IsCreated ? x.Id != washoutDto.Id : true),
                    _ => x => false
                },
                _ => x => false
            };
        }
        static Expression<Func<Washout, bool>> IValidationRule<Washout>.GetIdCriteria(IDto dto)
        {
            return dto switch
            {


                _ => x => true // DTO no reconocido → sin filtro
            };
        }
    }

}

