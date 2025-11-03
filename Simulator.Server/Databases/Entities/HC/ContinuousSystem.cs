using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Vml.Office;
using Simulator.Server.Databases.Contracts;
using Simulator.Server.ExtensionsMethods.Validations;
using Simulator.Shared.Enums.HCEnums.Enums;
using Simulator.Shared.Intefaces;
using Simulator.Shared.Models.HCs.ContinuousSystems;

namespace Simulator.Server.Databases.Entities.HC
{
    public class ContinuousSystem : BaseEquipment, IMapper, IQueryHandler<ContinuousSystem>, IValidationRule<ContinuousSystem>, ICreator<ContinuousSystem>
    {

        public double FlowValue { get; set; }
        public string FlowUnit { get; set; } = string.Empty;
        public static ContinuousSystem Create(IDto dto)
        {
            if (dto is ContinuousSystemDTO mappeddto)
            {
                var entity = new ContinuousSystem
                {
                    Id = Guid.NewGuid(),
                    MainProcessId = mappeddto.MainProcessId,

                };
                entity.AddInletConnector(mappeddto.InletConnectors);
                entity.AddOutletConnector(mappeddto.OutletConnectors);
                entity.AddPlannedDowntime(mappeddto.PlannedDownTimes);
                entity.AddMaterialEquipment(mappeddto.MaterialEquipments);



                return entity;
            }
            return null!;
        }
        public override void MapFromDto(IDto dto)
        {
            switch (dto)
            {
                case ContinuousSystemDTO request:
                    {
                        FlowValue = request.FlowValue;
                        FlowUnit = request.FlowUnitName;
                        ProccesEquipmentType = ProccesEquipmentType.ContinuousSystem;
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
                _ when typeof(T) == typeof(ContinuousSystemDTO) => (T)(object)new ContinuousSystemDTO
                {
                    Id = Id,
                    MainProcessId = MainProcessId,
                    FlowValue = FlowValue,
                    FlowUnitName = FlowUnit,
                    EquipmentType = ProccesEquipmentType,
                    Name = Name,
                    FocusFactory = FocusFactory,



                    Order = Order,

                },


                _ => default(T)!
            };
        }
        static Func<IQueryable<ContinuousSystem>, IIncludableQueryable<ContinuousSystem, object>> IQueryHandler<ContinuousSystem>.GetIncludesBy(IDto dto)
        {
            if (dto is ContinuousSystemDTO)
            {
                //return x => x.Include(y => y.Material);
            }

            return null!;

        }
        static Expression<Func<ContinuousSystem, object>> IQueryHandler<ContinuousSystem>.GetOrderBy(IDto dto)
        {
            if (dto is ContinuousSystemDTO)
            {
                return f => f.Name;
            }

            return null!;

        }
        static Expression<Func<ContinuousSystem, bool>> IQueryHandler<ContinuousSystem>.GetFilterBy(IDto dto)
        {
            return dto switch
            {
                ContinuousSystemDTO mappeddto =>

                x => x.MainProcessId == mappeddto.MainProcessId,

                _ => null!
            };
        }
        static Expression<Func<ContinuousSystem, bool>> IValidationRule<ContinuousSystem>.GetExistCriteria(IDto dto, string validationKey)
        {
            return dto switch
            {
                ContinuousSystemDTO mappeddto => validationKey switch
                {
                    nameof(ContinuousSystemDTO.Name) => mappeddto.BuildStringCriteria<ContinuousSystem, ContinuousSystemDTO>(
                         nameof(ContinuousSystemDTO.Name),
                         nameof(ContinuousSystem.Name)),
                    _ => x => false
                },
                _ => x => false
            };
        }
        static Expression<Func<ContinuousSystem, bool>> IValidationRule<ContinuousSystem>.GetIdCriteria(IDto dto)
        {
            return dto switch
            {
                ContinuousSystemDTO mappeddto => x => x.MainProcessId == mappeddto.MainProcessId,

                _ => x => true // DTO no reconocido → sin filtro
            };
        }

    }

}

