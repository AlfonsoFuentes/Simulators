using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Simulator.Server.Databases.Contracts;
using Simulator.Shared.Enums.HCEnums.Enums;
using Simulator.Shared.Intefaces;
using Simulator.Shared.Models.HCs.BaseEquipments;
using Simulator.Shared.Models.HCs.Conectors;
using System.ComponentModel.DataAnnotations.Schema;

namespace Simulator.Server.Databases.Entities.HC
{
    public abstract class BaseEquipment : Entity, IMapper     ,IQueryHandler<BaseEquipment>
    {
        public FocusFactory FocusFactory { get; set; } = FocusFactory.None;
        public ProccesEquipmentType ProccesEquipmentType { get; set; } = ProccesEquipmentType.None;
        public int X { get; set; }
        public int Y { get; set; }
        public string Name { get; set; } = string.Empty;

        public ProcessFlowDiagram MainProcess { get; set; } = null!;
        public Guid MainProcessId { get; set; } = Guid.Empty;
        public List<EquipmentPlannedDownTime> PlannedDownTimes { get; private set; } = new();
        public List<MaterialEquipment> Materials { get; set; } = new();

        [ForeignKey("FromId")]
        public List<Conector> Froms { get; set; } = new();
        [ForeignKey("ToId")]
        public List<Conector> Tos { get; set; } = new();

        public static Expression<Func<BaseEquipment, bool>> GetFilterBy(IDto dto)
        {
           return x => x.MainProcessId == ((BaseEquipmentDTO)dto).MainProcessId;
        }

        public static Func<IQueryable<BaseEquipment>, IIncludableQueryable<BaseEquipment, object>> GetIncludesBy(IDto dto)
        {
           return null!;
        }

        public static Expression<Func<BaseEquipment, object>> GetOrderBy(IDto dto)
        {
            return x => x.Name;
        }

        public virtual void MapFromDto(IDto dto)
        {

        }

        public virtual T MapToDto<T>() where T : IDto, new()
        {
            if (typeof(T) == typeof(BaseEquipmentDTO))
            {
                return (T)(object)new BaseEquipmentDTO()
                {
                    Id = Id,
                    Name = Name,
                    EquipmentType = ProccesEquipmentType,
                };
            }
            return default(T)!;
        }
    }

}

