using Simulator.Server.Databases.Contracts;
using Simulator.Server.ExtensionsMethods.Validations;
using Simulator.Shared.Enums.HCEnums.Enums;
using Simulator.Shared.Intefaces;
using Simulator.Shared.Models.HCs.StreamJoiners;

namespace Simulator.Server.Databases.Entities.HC
{
    public class StreamJoiner : BaseEquipment, IMapper, IQueryHandler<StreamJoiner>, IValidationRule<StreamJoiner>, ICreator<StreamJoiner>
    {
        public static StreamJoiner Create(Guid mainId) => new()
        {
            Id = Guid.NewGuid(),
            MainProcessId = mainId,

        };
        public static StreamJoiner Create(IDto dto)
        {
            if (dto is StreamJoinerDTO mappeddto)
            {
                var entity = new StreamJoiner
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
                case StreamJoinerDTO request:
                    {
                       
                        ProccesEquipmentType = ProccesEquipmentType.StreamJoiner;
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
                _ when typeof(T) == typeof(StreamJoinerDTO) => (T)(object)new StreamJoinerDTO
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
        static Func<IQueryable<StreamJoiner>, IIncludableQueryable<StreamJoiner, object>> IQueryHandler<StreamJoiner>.GetIncludesBy(IDto dto)
        {
            if (dto is StreamJoinerDTO)
            {
                //return x => x.Include(y => y.Material);
            }

            return null!;

        }
        static Expression<Func<StreamJoiner, object>> IQueryHandler<StreamJoiner>.GetOrderBy(IDto dto)
        {
            if (dto is StreamJoinerDTO)
            {
                return f => f.Name;
            }

            return null!;

        }
        static Expression<Func<StreamJoiner, bool>> IQueryHandler<StreamJoiner>.GetFilterBy(IDto dto)
        {
            return dto switch
            {
                StreamJoinerDTO mappeddto =>

                 x => x.MainProcessId == mappeddto.MainProcessId,

                _ => null!
            };
        }
        static Expression<Func<StreamJoiner, bool>> IValidationRule<StreamJoiner>.GetExistCriteria(IDto dto, string validationKey)
        {
            return dto switch
            {
                StreamJoinerDTO mappeddto => validationKey switch
                {
                    nameof(StreamJoinerDTO.Name) => mappeddto.BuildStringCriteria<StreamJoiner, StreamJoinerDTO>(
                         nameof(StreamJoinerDTO.Name),
                         nameof(StreamJoiner.Name)),
                    _ => x => false
                },
                _ => x => false
            };
        }
        static Expression<Func<StreamJoiner, bool>> IValidationRule<StreamJoiner>.GetIdCriteria(IDto dto)
        {
            return dto switch
            {
                StreamJoinerDTO mappeddto => x => x.MainProcessId == mappeddto.MainProcessId,

                _ => x => true // DTO no reconocido → sin filtro
            };
        }

    }

}

