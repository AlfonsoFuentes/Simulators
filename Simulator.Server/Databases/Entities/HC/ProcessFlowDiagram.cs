using Simulator.Server.Databases.Contracts;
using Simulator.Server.ExtensionsMethods.Validations;
using Simulator.Shared.Enums.HCEnums.Enums;
using Simulator.Shared.Intefaces;
using Simulator.Shared.Models.HCs.MainProcesss;
using Simulator.Shared.Simulations;

namespace Simulator.Server.Databases.Entities.HC
{
    public class ProcessFlowDiagram : Entity, IMapper, IQueryHandler<ProcessFlowDiagram>, IValidationRule<ProcessFlowDiagram>, ICreator<ProcessFlowDiagram>
    {
        public static ProcessFlowDiagram Create() => new() { Id = Guid.NewGuid() };

        public string Name { get; set; } = string.Empty;
        public List<BaseEquipment> ProccesEquipments { get; set; } = new List<BaseEquipment>();
        public List<SimulationPlanned> SimulationPlanneds { get; set; } = new();
        public FocusFactory FocusFactory { get; set; } = FocusFactory.None;
        public static ProcessFlowDiagram Create(IDto dto)
        {
            if (dto is ProcessFlowDiagramDTO mappeddto)
            {
                var entity = new ProcessFlowDiagram
                {
                    Id = Guid.NewGuid(),

                };


                return entity;
            }
            return null!;
        }
        public void MapFromDto(IDto dto)
        {
            switch (dto)
            {
                case ProcessFlowDiagramDTO request:
                    {
                        Name = request.Name;

                        FocusFactory = request.FocusFactory;

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
                _ when typeof(T) == typeof(ProcessFlowDiagramDTO) => (T)(object)new ProcessFlowDiagramDTO
                {
                    Id = Id,
                    FocusFactory = FocusFactory,
                    Name = Name,

                    Order = Order,

                },


                _ => default(T)!
            };
        }
        static Func<IQueryable<ProcessFlowDiagram>, IIncludableQueryable<ProcessFlowDiagram, object>> IQueryHandler<ProcessFlowDiagram>.GetIncludesBy(IDto dto)
        {
            if (dto is NewSimulationDTO)
            {
                return x => x.Include(y => y.ProccesEquipments);
            }

            return null!;

        }
        static Expression<Func<ProcessFlowDiagram, object>> IQueryHandler<ProcessFlowDiagram>.GetOrderBy(IDto dto)
        {
            if (dto is ProcessFlowDiagramDTO)
            {
                return f => f.Name;
            }

            return null!;

        }
        static Expression<Func<ProcessFlowDiagram, bool>> IQueryHandler<ProcessFlowDiagram>.GetFilterBy(IDto dto)
        {
            return dto switch
            {
                //ProcessFlowDiagramDTO material =>

                //x => (material.FocusFactory != FocusFactory.None ? x.FocusFactory == material.FocusFactory : true),

                _ => null!
            };
        }
        static Expression<Func<ProcessFlowDiagram, bool>> IValidationRule<ProcessFlowDiagram>.GetExistCriteria(IDto dto, string validationKey)
        {
            return dto switch
            {
                ProcessFlowDiagramDTO mappedto => validationKey switch
                {

                    nameof(ProcessFlowDiagramDTO.Name) => mappedto.BuildStringCriteria<ProcessFlowDiagram, ProcessFlowDiagramDTO>(
                        nameof(ProcessFlowDiagramDTO.Name),
                        nameof(ProcessFlowDiagram.Name)),
                    // nameof(SKUDTO.CommonName) => mappedto.BuildStringCriteria<SKU, SKUDTO>(
                    //nameof(SKUDTO.CommonName),
                    //nameof(SKU.CommonName)),
                    _ => x => false
                },
                _ => x => false
            };
        }
        static Expression<Func<ProcessFlowDiagram, bool>> IValidationRule<ProcessFlowDiagram>.GetIdCriteria(IDto dto)
        {
            return dto switch
            {


                _ => x => true // DTO no reconocido → sin filtro
            };
        }

    }

}

