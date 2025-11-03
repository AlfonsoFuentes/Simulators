using DocumentFormat.OpenXml.Spreadsheet;
using Simulator.Server.Databases.Contracts;
using Simulator.Shared.Enums.HCEnums.Enums;
using Simulator.Shared.Intefaces;
using Simulator.Shared.Models.HCs.PlannedSKUs;
using Simulator.Shared.Models.HCs.SKUs;

namespace Simulator.Server.Databases.Entities.HC
{
    public class PlannedSKU : Entity, IMapper, IQueryHandler<PlannedSKU>, IValidationRule<PlannedSKU>, ICreator<PlannedSKU>
    {


        public int PlannedCases { get; set; }

        public Guid SKUId { get; set; }
        public SKU SKU { get; set; } = null!;

        public double TimeToChangeSKUValue { get; set; }
        public string TimeToChangeSKUUnit { get; set; } = string.Empty;

        public double LineSpeedValue { get; set; }
        public string LineSpeedUnit { get; set; } = string.Empty;


        public static PlannedSKU Create(Guid LinePlannedId) =>
            new() { Id = Guid.NewGuid(), LinePlannedId = LinePlannedId };

        public LinePlanned LinePlanned { get; set; } = null!;
        public Guid LinePlannedId { get; set; }
        public static PlannedSKU Create(IDto dto)
        {
            if (dto is PlannedSKUDTO mappeddto)
            {
                var entity = new PlannedSKU
                {
                    Id = Guid.NewGuid(),
                    LinePlannedId = mappeddto.LinePlannedId,

                };




                return entity;
            }
            return null!;
        }
        public void MapFromDto(IDto dto)
        {
            switch (dto)
            {
                case PlannedSKUDTO request:
                    {
                        SKUId = request.SKU!.Id;

                        TimeToChangeSKUValue = request.TimeToChangeSKUValue;
                        TimeToChangeSKUUnit = request.TimeToChangeSKUUnitName;
                        PlannedCases = request.PlannedCases;

                        LineSpeedUnit = request.LineSpeedUnitName;
                        LineSpeedValue = request.LineSpeedValue;

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
                _ when typeof(T) == typeof(PlannedSKUDTO) => (T)(object)getPlannedSkudto(),


                _ => default(T)!
            };
        }
        PlannedSKUDTO getPlannedSkudto()
        {
            PlannedSKUDTO result = new();
            result.PlannedCases = PlannedCases;
            //Case_Shift = row.SKU.SKULines.Case_Shift,
            result.TimeToChangeSKUUnitName = TimeToChangeSKUUnit;
            result.TimeToChangeSKUValue = TimeToChangeSKUValue;
            result.LinePlannedId = LinePlannedId;
            result.SKU = SKU == null ? null! : SKU.MapToDto<SKUDTO>();
            result.LineId = LinePlanned == null ? Guid.Empty : LinePlanned.LineId;

            result.LineSpeedUnitName = LineSpeedUnit;
            result.LineSpeedValue = LineSpeedValue;



            result.Order = Order;
            var skulines = SKU == null ? null! : SKU.SKULines;
            if (skulines != null && skulines.Any())
            {
                var case_shift = skulines.FirstOrDefault(x => x.LineId == result.LineId);
                result.Case_Shift = case_shift == null ? 0 : case_shift.Case_Shift;
            }
            return result;
        }
        static Func<IQueryable<PlannedSKU>, IIncludableQueryable<PlannedSKU, object>> IQueryHandler<PlannedSKU>.GetIncludesBy(IDto dto)
        {
            if (dto is PlannedSKUDTO)
            {
                return x => x

                   .Include(x => x.LinePlanned)
                   .Include(y => y.SKU).ThenInclude(x => x.SKULines);
            }

            return null!;

        }
        static Expression<Func<PlannedSKU, object>> IQueryHandler<PlannedSKU>.GetOrderBy(IDto dto)
        {
            if (dto is PlannedSKUDTO)
            {
                return f => f.Order;
            }

            return null!;

        }
        static Expression<Func<PlannedSKU, bool>> IQueryHandler<PlannedSKU>.GetFilterBy(IDto dto)
        {
            return dto switch
            {
                PlannedSKUDTO mappeddto =>

               x => x.LinePlannedId == mappeddto.LinePlannedId,

                _ => null!
            };
        }
        static Expression<Func<PlannedSKU, bool>> IValidationRule<PlannedSKU>.GetExistCriteria(IDto dto, string validationKey)
        {
            return dto switch
            {
                PlannedSKUDTO mappeddto => validationKey switch
                {
                    //nameof(PlannedSKUDTO.Name) => mappeddto.BuildStringCriteria<PlannedSKU, PlannedSKUDTO>(
                    //     nameof(PlannedSKUDTO.Name),
                    //     nameof(PlannedSKU.Name)),
                    _ => x => false
                },
                _ => x => false
            };
        }
        static Expression<Func<PlannedSKU, bool>> IValidationRule<PlannedSKU>.GetIdCriteria(IDto dto)
        {
            return dto switch
            {
                PlannedSKUDTO mappeddto => x => x.LinePlannedId == mappeddto.LinePlannedId,

                _ => x => true // DTO no reconocido → sin filtro
            };
        }
    }

}

