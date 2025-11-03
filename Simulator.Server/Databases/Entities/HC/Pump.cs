using DocumentFormat.OpenXml.Spreadsheet;
using Simulator.Server.Databases.Contracts;
using Simulator.Server.ExtensionsMethods.Validations;
using Simulator.Shared.Enums.HCEnums.Enums;
using Simulator.Shared.Intefaces;
using Simulator.Shared.Models.HCs.Pumps;

namespace Simulator.Server.Databases.Entities.HC
{
    public class Pump : BaseEquipment, IMapper, IQueryHandler<Pump>, IValidationRule<Pump>, ICreator<Pump>
    {
        public static Pump Create(Guid mainId) => new()
        {
            Id = Guid.NewGuid(),
            MainProcessId = mainId,

        };
        public bool IsForWashing { get; set; }
        public double FlowValue { get; set; }
        public string FlowUnit { get; set; } = string.Empty;
        public static Pump Create(IDto dto)
        {
            if (dto is PumpDTO mappeddto)
            {
                var entity = new Pump
                {
                    Id = Guid.NewGuid(),
                    MainProcessId = mappeddto.MainProcessId,

                };
                entity.AddInletConnector(mappeddto.InletConnectors);
                entity.AddOutletConnector(mappeddto.OutletConnectors);
                entity.AddPlannedDowntime(mappeddto.PlannedDownTimes);


                return entity;
            }
            return null!;
        }
        public override void MapFromDto(IDto dto)
        {
            switch (dto)
            {
                case PumpDTO request:
                    {
                        FlowValue = request.FlowValue;
                        FlowUnit = request.FlowUnitName;
                        IsForWashing = request.IsForWashing;
                        Name = request.Name;
                        ProccesEquipmentType = ProccesEquipmentType.Pump;
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
                _ when typeof(T) == typeof(PumpDTO) => (T)(object)new PumpDTO
                {
                    Id = Id,
                    MainProcessId = MainProcessId,
                    FlowValue = FlowValue,
                    FlowUnitName =      FlowUnit,
                    IsForWashing = IsForWashing,
                    EquipmentType = ProccesEquipmentType,
                    Name = Name,
                    FocusFactory = FocusFactory,


                    Order = Order,

                },


                _ => default(T)!
            };
        }
        static Func<IQueryable<Pump>, IIncludableQueryable<Pump, object>> IQueryHandler<Pump>.GetIncludesBy(IDto dto)
        {
            if (dto is PumpDTO)
            {
                //return x => x.Include(y => y.Material);
            }

            return null!;

        }
        static Expression<Func<Pump, object>> IQueryHandler<Pump>.GetOrderBy(IDto dto)
        {
            if (dto is PumpDTO)
            {
                return f => f.Name;
            }

            return null!;

        }
        static Expression<Func<Pump, bool>> IQueryHandler<Pump>.GetFilterBy(IDto dto)
        {
            return dto switch
            {
                PumpDTO mappeddto =>

                 x => x.MainProcessId == mappeddto.MainProcessId,

                _ => null!
            };
        }
        static Expression<Func<Pump, bool>> IValidationRule<Pump>.GetExistCriteria(IDto dto, string validationKey)
        {
            return dto switch
            {
                PumpDTO mappeddto => validationKey switch
                {
                    nameof(PumpDTO.Name) => mappeddto.BuildStringCriteria<Pump, PumpDTO>(
                         nameof(PumpDTO.Name),
                         nameof(Pump.Name)),
                    _ => x => false
                },
                _ => x => false
            };
        }
        static Expression<Func<Pump, bool>> IValidationRule<Pump>.GetIdCriteria(IDto dto)
        {
            return dto switch
            {
                PumpDTO mappeddto => x => x.MainProcessId == mappeddto.MainProcessId,

                _ => x => true // DTO no reconocido → sin filtro
            };
        }

    }

}

