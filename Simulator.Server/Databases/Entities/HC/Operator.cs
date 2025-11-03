using Simulator.Server.Databases.Contracts;
using Simulator.Server.ExtensionsMethods.Validations;
using Simulator.Shared.Enums.HCEnums.Enums;
using Simulator.Shared.Intefaces;
using Simulator.Shared.Models.HCs.Operators;

namespace Simulator.Server.Databases.Entities.HC
{
    public class Operator : BaseEquipment, IMapper, IQueryHandler<Operator>, IValidationRule<Operator>, ICreator<Operator>
    {
        public static Operator Create(Guid mainId) => new() { Id = Guid.NewGuid(), MainProcessId = mainId };
        public static Operator Create(IDto dto)
        {
            if (dto is OperatorDTO mappeddto)
            {
                var entity = new Operator
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
                case OperatorDTO request:
                    {
                       
                        ProccesEquipmentType = ProccesEquipmentType.Operator;
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
                _ when typeof(T) == typeof(OperatorDTO) => (T)(object)new OperatorDTO
                {
                    Id = Id,
                    MainProcessId = MainProcessId,
                   
                    EquipmentType = ProccesEquipmentType,
                    Name = Name,
                    FocusFactory = FocusFactory,


                    Order = Order,

                },


                _ => default(T)!
            };
        }
        static Func<IQueryable<Operator>, IIncludableQueryable<Operator, object>> IQueryHandler<Operator>.GetIncludesBy(IDto dto)
        {
            if (dto is OperatorDTO)
            {
                //return x => x.Include(y => y.Material);
            }

            return null!;

        }
        static Expression<Func<Operator, object>> IQueryHandler<Operator>.GetOrderBy(IDto dto)
        {
            if (dto is OperatorDTO)
            {
                return f => f.Name;
            }

            return null!;

        }
        static Expression<Func<Operator, bool>> IQueryHandler<Operator>.GetFilterBy(IDto dto)
        {
            return dto switch
            {
                OperatorDTO mappeddto =>

                x => x.MainProcessId == mappeddto.MainProcessId,

                _ => null!
            };
        }
        static Expression<Func<Operator, bool>> IValidationRule<Operator>.GetExistCriteria(IDto dto, string validationKey)
        {
            return dto switch
            {
                OperatorDTO mappeddto => validationKey switch
                {
                    nameof(OperatorDTO.Name) => mappeddto.BuildStringCriteria<Operator, OperatorDTO>(
                         nameof(OperatorDTO.Name),
                         nameof(Operator.Name)),
                    _ => x => false
                },
                _ => x => false
            };
        }
        static Expression<Func<Operator, bool>> IValidationRule<Operator>.GetIdCriteria(IDto dto)
        {
            return dto switch
            {
                OperatorDTO mappeddto => x => x.MainProcessId == mappeddto.MainProcessId,

                _ => x => true // DTO no reconocido → sin filtro
            };
        }

    }

}

