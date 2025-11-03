using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Simulator.Server.Databases.Contracts;
using Simulator.Server.ExtensionsMethods.Validations;
using Simulator.Shared.Enums.HCEnums.Enums;
using Simulator.Shared.Intefaces;
using Simulator.Shared.Models.HCs.Lines;
using Simulator.Shared.Models.HCs.SKUs;
using System.ComponentModel.DataAnnotations.Schema;

namespace Simulator.Server.Databases.Entities.HC
{
    public class Line : BaseEquipment, IMapper, IQueryHandler<Line>, IValidationRule<Line>, ICreator<Line>
    {
        public List<SKULine> SKULines { get; set; } = new();

        public PackageType PackageType { get; set; } = PackageType.None;

        public static Line Create(Guid mainId) => new()
        {
            Id = Guid.NewGuid(),
            MainProcessId = mainId,


        };
        public double TimeToReviewAUValue { get; set; }
        public string TimeToReviewAUUnit { get; set; } = string.Empty;


        [ForeignKey("LineId")]
        public List<LinePlanned> LinePlanneds { get; set; } = new();

        public static Line Create(IDto dto)
        {
            if (dto is LineDTO mappeddto)
            {
                var entity = new Line
                {
                    Id = Guid.NewGuid(),
                    MainProcessId = mappeddto.MainProcessId,

                };
                entity.AddInletConnector(mappeddto.InletConnectors);             
                entity.AddPlannedDowntime(mappeddto.PlannedDownTimes);
                entity.AddLineSKUs(mappeddto.LineSKUs);
        


                return entity;
            }
            return null!;
        }
        public override void MapFromDto(IDto dto)
        {
            switch (dto)
            {
                case LineDTO request:
                    {
                        TimeToReviewAUValue = request.TimeToReviewAUValue;
                        TimeToReviewAUUnit = request.TimeToReviewAUUnitName;
                        PackageType = request.PackageType;
                        ProccesEquipmentType = ProccesEquipmentType.Line;
                        Name = request.Name;
                        FocusFactory = request.FocusFactory;
                 
                    }
                    break;

                default:
                    break;

            }
        }
        public override T MapToDto<T>()
        {
            return typeof(T) switch
            {
                _ when typeof(T) == typeof(LineDTO) => (T)(object)new LineDTO
                {
                    Id = Id,
                    MainProcessId = MainProcessId,
                    TimeToReviewAUValue = TimeToReviewAUValue,
                    TimeToReviewAUUnitName = TimeToReviewAUUnit,
                    PackageType = PackageType,
                    EquipmentType = ProccesEquipmentType,
                    Name = Name,
                    FocusFactory = FocusFactory,


                    Order = Order,

                },


                _ => default(T)!
            };
        }
        static Func<IQueryable<Line>, IIncludableQueryable<Line, object>> IQueryHandler<Line>.GetIncludesBy(IDto dto)
        {
            if (dto is LineDTO)
            {
                //return x => x.Include(y => y.Material);
            }

            return null!;

        }
        static Expression<Func<Line, object>> IQueryHandler<Line>.GetOrderBy(IDto dto)
        {
            if (dto is LineDTO)
            {
                return f => f.Name;
            }

            return null!;

        }
        static Expression<Func<Line, bool>> IQueryHandler<Line>.GetFilterBy(IDto dto)
        {
            return dto switch
            {
                LineDTO mappeddto =>

                x => x.MainProcessId == mappeddto.MainProcessId,

                _ => null!
            };
        }
        static Expression<Func<Line, bool>> IValidationRule<Line>.GetExistCriteria(IDto dto, string validationKey)
        {
            return dto switch
            {
                LineDTO mappeddto => validationKey switch
                {
                    nameof(LineDTO.Name) => mappeddto.BuildStringCriteria<Line, LineDTO>(
                         nameof(LineDTO.Name),
                         nameof(Line.Name)),
                    _ => x => false
                },
                _ => x => false
            };
        }
        static Expression<Func<Line, bool>> IValidationRule<Line>.GetIdCriteria(IDto dto)
        {
            return dto switch
            {
                LineDTO mappeddto => x => x.MainProcessId == mappeddto.MainProcessId,

                _ => x => true // DTO no reconocido → sin filtro
            };
        }
    }

}

