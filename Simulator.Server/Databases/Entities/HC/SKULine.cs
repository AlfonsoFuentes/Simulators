using Simulator.Server.Databases.Contracts;
using Simulator.Shared.Intefaces;
using Simulator.Shared.Models.HCs.SKULines;
using Simulator.Shared.Models.HCs.SKUs;

namespace Simulator.Server.Databases.Entities.HC
{
    public class SKULine : Entity, IMapper, IQueryHandler<SKULine>, IValidationRule<SKULine>, ICreator<SKULine>
    {
        public Guid SKUId { get; set; }
        public SKU SKU { get; set; } = null!;

        public Guid LineId { get; set; }
        public Line Line { get; set; } = null!;
        public double LineSpeedValue { get; set; }
        public string LineSpeedUnit { get; set; } = string.Empty;
        public double Case_Shift { get; set; }
        public static SKULine Create(Guid LineId)
        {
            return new SKULine()
            {
                Id = Guid.NewGuid(),
                LineId = LineId,
            };
        }
        public static SKULine Create(IDto dto)
        {
            if (dto is SKULineDTO mappeddto)
            {
                var entity = new SKULine
                {
                    Id = Guid.NewGuid(),
                   LineId = mappeddto.LineId,

                };
           



                return entity;
            }
            return null!;
        }
        public void MapFromDto(IDto dto)
        {
            switch (dto)
            {
                case SKULineDTO request:
                    {
                        LineSpeedValue = request.LineSpeedValue;
                        LineSpeedUnit = request.LineSpeedUnitName;
                        SKUId = request.SKUId;
                        Case_Shift = request.Case_Shift;

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
                _ when typeof(T) == typeof(SKULineDTO) => (T)(object)new SKULineDTO
                {
                    Id = Id,
                    LineSpeedUnitName = LineSpeedUnit,
                    LineSpeedValue = LineSpeedValue,
                    SKU = SKU == null ? null! : SKU.MapToDto<SKUDTO>(),
                    LineId = LineId,
                    Case_Shift = Case_Shift,
                    Order = Order,


                },


                _ => default(T)!
            };
        }
        static Func<IQueryable<SKULine>, IIncludableQueryable<SKULine, object>> IQueryHandler<SKULine>.GetIncludesBy(IDto dto)
        {
            if (dto is SKULineDTO)
            {
                return x => x.Include(y => y.SKU).ThenInclude(x => x.Material);
            }

            return null!;

        }
        static Expression<Func<SKULine, object>> IQueryHandler<SKULine>.GetOrderBy(IDto dto)
        {
            if (dto is SKULineDTO)
            {
                //return f => f.Name;
            }

            return null!;

        }
        static Expression<Func<SKULine, bool>> IQueryHandler<SKULine>.GetFilterBy(IDto dto)
        {
            return dto switch
            {
                SKULineDTO request =>

                x => x.LineId == request.LineId,

                _ => null!
            };
        }
        static Expression<Func<SKULine, bool>> IValidationRule<SKULine>.GetExistCriteria(IDto dto, string validationKey)
        {
            return dto switch
            {
                SKULineDTO skulinedto => validationKey switch
                {
                    SKULineDTO.ValidationSKUId =>
                        x => x.SKUId == skulinedto.SKUId &&
                             (skulinedto.IsCreated ? x.Id != skulinedto.Id : true),
                    _ => x => false
                },

               
                _ => x => false
            };
        }
        static Expression<Func<SKULine, bool>> IValidationRule<SKULine>.GetIdCriteria(IDto dto)
        {
            return dto switch
            {
                SKULineDTO mappeddto => x => x.LineId == mappeddto.LineId,

                _ => x => true // DTO no reconocido → sin filtro
            };
        }
    }

}

