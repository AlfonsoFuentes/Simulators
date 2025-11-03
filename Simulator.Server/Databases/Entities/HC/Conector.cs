using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using Simulator.Server.Databases.Contracts;
using Simulator.Server.ExtensionsMethods.Validations;
using Simulator.Shared.Enums.HCEnums.Enums;
using Simulator.Shared.Intefaces;
using Simulator.Shared.Models.HCs.BaseEquipments;
using Simulator.Shared.Models.HCs.Conectors;
using Simulator.Shared.Models.HCs.Lines;
using Simulator.Shared.Models.HCs.Washouts;

namespace Simulator.Server.Databases.Entities.HC
{
    public class Conector : Entity, IMapper, IQueryHandler<Conector>, IValidationRule<Conector>, ICreator<Conector>
    {

        public Guid MainProcessId { get; set; } = Guid.Empty;

        public Guid FromId { get; set; }
        public BaseEquipment From { get; set; } = null!;
        public Guid ToId { get; set; }
        public BaseEquipment To { get; set; } = null!;


        public static Conector CreateInlet(Guid ToId, Guid MainProcessId)
        {
            return new()
            {
                Id = Guid.NewGuid(),
                ToId = ToId,
                MainProcessId = MainProcessId
            };

        }
        public static Conector CreateOutlet(Guid FromId, Guid MainProcessId)
        {
            return new()
            {
                Id = Guid.NewGuid(),
                FromId = FromId,
                MainProcessId = MainProcessId
            };

        }
        public static Conector Create(IDto dto)
        {
            var entity = new Conector
            {
                Id = Guid.NewGuid(),


            };
            if (dto is InletConnectorDTO inmappeddto)
            {
                entity.MainProcessId = inmappeddto.MainProcessId;

                return entity;
            }
            if (dto is OutletConnectorDTO outmappeddto)
            {
                entity.MainProcessId = outmappeddto.MainProcessId;

                return entity;
            }
            return null!;
        }
        public void MapFromDto(IDto dto)
        {
            switch (dto)
            {
                case InletConnectorDTO request:
                    {
                        FromId = request.FromId;

                    }
                    break;
                case OutletConnectorDTO request:
                    {
                        ToId = request.ToId;

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
                _ when typeof(T) == typeof(InletConnectorDTO) => (T)(object)new InletConnectorDTO
                {
                    Id = Id,
                    MainProcessId = MainProcessId,

                    From = From == null ? null! : new BaseEquipmentDTO()
                    {
                        Id = FromId,
                        EquipmentType = From.ProccesEquipmentType,
                        Name = From.Name,
                        FocusFactory = From.FocusFactory
                    },
                    To = To == null ? null! : new BaseEquipmentDTO()
                    {
                        Id = ToId,
                        EquipmentType = To.ProccesEquipmentType,
                        Name = To.Name,
                        FocusFactory = To.FocusFactory

                    },
                    ToId = ToId,
                    FromId = FromId,

                    Order = Order,



                },
                _ when typeof(T) == typeof(OutletConnectorDTO) => (T)(object)new OutletConnectorDTO
                {
                    Id = Id,
                    MainProcessId = MainProcessId,
                    ToId = ToId,
                    FromId = FromId,
                    From = From == null ? null! : new BaseEquipmentDTO()
                    {
                        Id = FromId,
                        EquipmentType = From.ProccesEquipmentType,
                        Name = From.Name,
                        FocusFactory = From.FocusFactory
                    },
                    To = To == null ? null! : new BaseEquipmentDTO()
                    {
                        Id = ToId,
                        EquipmentType = To.ProccesEquipmentType,
                        Name = To.Name,
                        FocusFactory = To.FocusFactory

                    },
                    Order = Order,



                },

                _ => default(T)!
            };
        }
        static Func<IQueryable<Conector>, IIncludableQueryable<Conector, object>> IQueryHandler<Conector>.GetIncludesBy(IDto dto)
        {
            return dto switch
            {
                InletConnectorDTO request => x => x.Include(x => x.From),
                OutletConnectorDTO request => x => x.Include(x => x.To),
                ConectorDTO request => x => x.Include(x => x.From).Include(x => x.To),

                _ => null!
            };

        }
        static Expression<Func<Conector, object>> IQueryHandler<Conector>.GetOrderBy(IDto dto)
        {
            if (dto is LineDTO)
            {
                //return f => f.Name;
            }

            return null!;

        }
        static Expression<Func<Conector, bool>> IQueryHandler<Conector>.GetFilterBy(IDto dto)
        {
            return dto switch
            {
                InletConnectorDTO request => x => x.ToId == request.ToId,
                OutletConnectorDTO request => x => x.FromId == request.FromId,
                ConectorDTO request => x => x.MainProcessId == request.MainProcessId,

                _ => null!
            };
        }
        static Expression<Func<Conector, bool>> IValidationRule<Conector>.GetExistCriteria(IDto dto, string validationKey)
        {
            return dto switch
            {
                InletConnectorDTO inletdto => validationKey switch
                {
                    InletConnectorDTO.ConnectorReview =>
                        x => x.ToId == inletdto.ToId &&
                             x.FromId == inletdto.FromId &&
                             (inletdto.IsCreated ? x.Id != inletdto.Id : true),
                    _ => x => false
                },
                OutletConnectorDTO ouletdto => validationKey switch
                {
                    OutletConnectorDTO.ConnectorReview =>
                        x => x.ToId == ouletdto.ToId &&
                             x.FromId == ouletdto.FromId &&
                             (ouletdto.IsCreated ? x.Id != ouletdto.Id : true),
                    _ => x => false
                },
                _ => x => false
            };
        }
        static Expression<Func<Conector, bool>> IValidationRule<Conector>.GetIdCriteria(IDto dto)
        {
            return dto switch
            {
               

                _ => x => true 
            };
        }
    }

}

