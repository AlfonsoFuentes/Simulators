using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Simulator.Server.Databases.Contracts;
using Simulator.Shared.Enums.HCEnums.Enums;
using Simulator.Shared.Intefaces;
using Simulator.Shared.Models.HCs.LinePlanneds;
using Simulator.Shared.Models.HCs.Lines;
using Simulator.Shared.Models.HCs.PreferedMixers;
using System.ComponentModel.DataAnnotations.Schema;

namespace Simulator.Server.Databases.Entities.HC
{
    public class LinePlanned : Entity, IMapper, IQueryHandler<LinePlanned>, IValidationRule<LinePlanned>, ICreator<LinePlanned>
    {

        public Guid LineId { get; set; }
        public Line Line { get; set; } = null!;
        public ShiftType ShiftType { get; set; }

        public SimulationPlanned HCSimulationPlanned { get; set; } = null!;
        public Guid SimulationPlannedId { get; set; }

        public static LinePlanned Create(Guid SimulationId)
        {
            LinePlanned retorno = new() { Id = Guid.NewGuid() };
            retorno.SimulationPlannedId = SimulationId;

            return retorno;
        }
        [ForeignKey("LinePlannedId")]
        public List<PlannedSKU> SKUPlanneds { get; private set; } = new List<PlannedSKU>();


        public double WIPLevelValue { get; set; }
        public string WIPLevelUnit { get; set; } = string.Empty;

        [ForeignKey("LinePlannedId")]
        public List<PreferedMixer> PreferedMixers { get; private set; } = new List<PreferedMixer>();
        public static LinePlanned Create(IDto dto)
        {
            if (dto is LinePlannedDTO mappeddto)
            {
                var entity = new LinePlanned
                {
                    Id = Guid.NewGuid(),
                   SimulationPlannedId = mappeddto.SimulationPlannedId,

                };
                 foreach(var plannedsku in mappeddto.PlannedSKUDTOs)
                 {
                    PlannedSKU rowplanned = PlannedSKU.Create(plannedsku);
                    rowplanned.LinePlannedId = entity.Id;
                    rowplanned.MapFromDto(plannedsku);
                    entity.SKUPlanneds.Add(rowplanned);
                }
                foreach (var preferedmixer in mappeddto.PreferedMixerDTOs)
                {
                    PreferedMixer rowpreferred = PreferedMixer.Create(preferedmixer);
                    rowpreferred.LinePlannedId = entity.Id;
                    rowpreferred.MapFromDto(preferedmixer);
                    entity.PreferedMixers.Add(rowpreferred);
                }

                    return entity;
            }
            return null!;
        }
        public  void MapFromDto(IDto dto)
        {
            switch (dto)
            {
                case LinePlannedDTO request:
                    {
                        ShiftType = request.ShiftType;
                        WIPLevelValue = request.WIPLevelValue;
                        WIPLevelUnit = request.WIPLevelUnitName;
         

                    }
                    break;

                default:
                    break;

            }
        }
        public  T MapToDto<T>() where T : IDto,new()
        {
            return typeof(T) switch
            {
                _ when typeof(T) == typeof(LinePlannedDTO) => (T)(object)new LinePlannedDTO
                {
                    Id = Id,
                    MainProcesId =  HCSimulationPlanned == null ? Guid.Empty : HCSimulationPlanned.MainProcessId,
       
                    LineDTO = Line == null ? null! : Line.MapToDto<LineDTO>(),
                    WIPLevelValue = WIPLevelValue,
                    WIPLevelUnitName = WIPLevelUnit,
                    ShiftType = ShiftType,
                    SimulationPlannedId = SimulationPlannedId,
                    Order = Order,
                    PreferedMixerDTOs = PreferedMixers == null || PreferedMixers.Count == 0 ? new() : PreferedMixers.Select(x => x.MapToDto<PreferedMixerDTO>()).ToList(),



                },


                _ => default(T)!
            };
        }
        static Func<IQueryable<LinePlanned>, IIncludableQueryable<LinePlanned, object>> IQueryHandler<LinePlanned>.GetIncludesBy(IDto dto)
        {
            if (dto is LinePlannedDTO)
            {
                return x => x
                   .Include(y => y.Line)
                   .Include(x => x.HCSimulationPlanned)
                   .Include(x => x.PreferedMixers).ThenInclude(x => x.Mixer);
            }

            return null!;

        }
        static Expression<Func<LinePlanned, object>> IQueryHandler<LinePlanned>.GetOrderBy(IDto dto)
        {
            if (dto is LinePlannedDTO)
            {
                return f => f.Order;
            }

            return null!;

        }
        static Expression<Func<LinePlanned, bool>> IQueryHandler<LinePlanned>.GetFilterBy(IDto dto)
        {
            return dto switch
            {
                LinePlannedDTO mappeddto =>

                x => x.SimulationPlannedId == mappeddto.SimulationPlannedId,

                _ => null!
            };
        }
        static Expression<Func<LinePlanned, bool>> IValidationRule<LinePlanned>.GetExistCriteria(IDto dto, string validationKey)
        {
            return dto switch
            {
                LinePlannedDTO mappeddto => validationKey switch
                {
                    //nameof(LinePlannedDTO.Name) => mappeddto.BuildStringCriteria<LinePlanned, LinePlannedDTO>(
                    //     nameof(LinePlannedDTO.Name),
                    //     nameof(LinePlanned.Name)),
                    _ => x => false
                },
                _ => x => false
            };
        }
        static Expression<Func<LinePlanned, bool>> IValidationRule<LinePlanned>.GetIdCriteria(IDto dto)
        {
            return dto switch
            {
                //LinePlannedDTO mappeddto => x => x.MainProcessId == mappeddto.MainProcessId,

                _ => x => true // DTO no reconocido → sin filtro
            };
        }
    }

}

