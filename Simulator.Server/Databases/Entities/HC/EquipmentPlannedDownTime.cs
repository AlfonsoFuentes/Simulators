using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Spreadsheet;
using Simulator.Server.Databases.Contracts;
using Simulator.Server.ExtensionsMethods.Validations;
using Simulator.Shared.Enums.HCEnums.Enums;
using Simulator.Shared.Intefaces;
using Simulator.Shared.Models.HCs.ContinuousSystems;
using Simulator.Shared.Models.HCs.EquipmentPlannedDownTimes;

namespace Simulator.Server.Databases.Entities.HC
{
    public class EquipmentPlannedDownTime : Entity, IMapper, IQueryHandler<EquipmentPlannedDownTime>, IValidationRule<EquipmentPlannedDownTime>, ICreator<EquipmentPlannedDownTime>
    {


        public string Name { get; set; } = string.Empty;
        public static EquipmentPlannedDownTime Create(Guid _BaseEquipmentId) =>
            new()
            {
                Id = Guid.NewGuid(),
                BaseEquipmentId = _BaseEquipmentId
            };

        public Guid BaseEquipmentId { get; set; }
        public BaseEquipment BaseEquipment { get; set; } = null!;

        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public static EquipmentPlannedDownTime Create(IDto dto)
        {
            if (dto is EquipmentPlannedDownTimeDTO mappeddto)
            {
                var entity = new EquipmentPlannedDownTime
                {
                    Id = Guid.NewGuid(),
                    BaseEquipmentId = mappeddto.BaseEquipmentId,

                };




                return entity;
            }
            return null!;
        }
        public void MapFromDto(IDto dto)
        {
            switch (dto)
            {
                case EquipmentPlannedDownTimeDTO request:
                    {
                        StartTime = request.StartTime;
                        EndTime = request.EndTime;



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
                _ when typeof(T) == typeof(EquipmentPlannedDownTimeDTO) => (T)(object)new EquipmentPlannedDownTimeDTO
                {
                    Id = Id,
                    StartTime = StartTime,
                    EndTime = EndTime,
                    Order = Order,

                },


                _ => default(T)!
            };
        }
        static Func<IQueryable<EquipmentPlannedDownTime>, IIncludableQueryable<EquipmentPlannedDownTime, object>> IQueryHandler<EquipmentPlannedDownTime>.GetIncludesBy(IDto dto)
        {
            if (dto is EquipmentPlannedDownTimeDTO request)
            {
                if (request.MainProcessId != Guid.Empty)
                {
                    return x => x.Include(y => y.BaseEquipment);
                }

            }

            return null!;

        }
        static Expression<Func<EquipmentPlannedDownTime, object>> IQueryHandler<EquipmentPlannedDownTime>.GetOrderBy(IDto dto)
        {
            if (dto is EquipmentPlannedDownTimeDTO)
            {
                return f => f.StartTime!.Value;
            }

            return null!;

        }
        static Expression<Func<EquipmentPlannedDownTime, bool>> IQueryHandler<EquipmentPlannedDownTime>.GetFilterBy(IDto dto)
        {
            return dto switch
            {
                EquipmentPlannedDownTimeDTO mappeddto => getExpresion(mappeddto),

                _ => null!
            };
        }
        static Expression<Func<EquipmentPlannedDownTime, bool>> getExpresion(EquipmentPlannedDownTimeDTO dto)
        {
            if (dto.MainProcessId != Guid.Empty)
            {
                return x => x.BaseEquipment != null ? x.BaseEquipment.MainProcessId == dto.MainProcessId : false;
            }
            if (dto.BaseEquipmentId != Guid.Empty)
            {
                return x => x.BaseEquipmentId == dto.BaseEquipmentId;
            }


            return null!;
        }
        static Expression<Func<EquipmentPlannedDownTime, bool>> IValidationRule<EquipmentPlannedDownTime>.GetExistCriteria(IDto dto, string validationKey)
        {
            return dto switch
            {
                //EquipmentPlannedDownTimeDTO mappeddto => validationKey switch
                //{
                //    nameof(EquipmentPlannedDownTimeDTO.Name) => mappeddto.BuildStringCriteria<EquipmentPlannedDownTime, EquipmentPlannedDownTimeDTO>(
                //         nameof(EquipmentPlannedDownTimeDTO.Name),
                //         nameof(EquipmentPlannedDownTime.Name)),
                //    _ => x => false
                //},
                _ => x => false
            };
        }
        static Expression<Func<EquipmentPlannedDownTime, bool>> IValidationRule<EquipmentPlannedDownTime>.GetIdCriteria(IDto dto)
        {
            return dto switch
            {
                //EquipmentPlannedDownTimeDTO mappeddto => x => x.MainProcessId == mappeddto.MainProcessId,

                _ => x => true // DTO no reconocido → sin filtro
            };
        }
    }

}

