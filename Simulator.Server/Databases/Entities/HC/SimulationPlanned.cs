using DocumentFormat.OpenXml.Spreadsheet;
using Simulator.Server.Databases.Contracts;
using Simulator.Server.ExtensionsMethods.Validations;
using Simulator.Shared.Enums.HCEnums.Enums;
using Simulator.Shared.Intefaces;
using Simulator.Shared.Models.HCs.SimulationPlanneds;

namespace Simulator.Server.Databases.Entities.HC
{
    public class SimulationPlanned : Entity, IMapper, IQueryHandler<SimulationPlanned>, IValidationRule<SimulationPlanned>, ICreator<SimulationPlanned>
    {

        public string Name { get; set; } = string.Empty;
        public DateTime? InitDate { get; set; }
        public TimeSpan? InitSpam { get; set; }
        public DateTime? EndDate { get; set; }
        public double PlannedHours { get; set; }
        public ProcessFlowDiagram MainProcess { get; set; } = null!;
        public Guid MainProcessId { get; set; }
        public List<LinePlanned> LinePlanneds { get; private set; } = new();

        public List<MixerPlanned> MixerPlanneds { get; private set; } = new();


        public bool OperatorHasNotRestrictionToInitBatch { get; set; }
        public double MaxRestrictionTimeValue { get; set; }
        public string MaxRestrictionTimeUnit { get; set; } = string.Empty;
        public static SimulationPlanned Create(IDto dto)
        {
            if (dto is SimulationPlannedDTO mappeddto)
            {
                var entity = new SimulationPlanned
                {
                    Id = Guid.NewGuid(),
                    MainProcessId = mappeddto.MainProcessId,

                };
                entity.MapFromDto(mappeddto);
                foreach (var plannedline in mappeddto.PlannedLines)
                {
                    LinePlanned rowplanned = LinePlanned.Create(plannedline);
                    rowplanned.SimulationPlannedId = entity.Id;
                    rowplanned.MapFromDto(dto);
                    entity.LinePlanneds.Add(rowplanned);
                    
                }
                foreach (var plannedMixer in mappeddto.PlannedMixers)
                {
                    MixerPlanned rowplanned = MixerPlanned.Create(plannedMixer);
                    rowplanned.SimulationPlannedId = entity.Id;
                    rowplanned.MapFromDto(plannedMixer);
                    entity.MixerPlanneds.Add(rowplanned);
                }


                return entity;
            }
            return null!;
        }
        public void MapFromDto(IDto dto)
        {
            switch (dto)
            {
                case SimulationPlannedDTO request:
                    {
                        Name = request.Name;
                        InitDate = request.InitDate;
                        PlannedHours = request.Hours;
                        EndDate = request.EndDate;
                        InitSpam = request.InitSpam;
                        MaxRestrictionTimeValue = request.MaxRestrictionTimeValue;
                        MaxRestrictionTimeUnit = request.MaxRestrictionTimeUnit;
                        OperatorHasNotRestrictionToInitBatch = request.OperatorHasNotRestrictionToInitBatch;

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
                _ when typeof(T) == typeof(SimulationPlannedDTO) => (T)(object)new SimulationPlannedDTO
                {
                    Id = Id,

                    MainProcessId = MainProcessId,
                    Hours = PlannedHours,
                    InitDate = InitDate,
                    InitSpam = InitSpam,
                    Name = Name,
                    MaxRestrictionTimeValue = MaxRestrictionTimeValue,
                    MaxRestrictionTimeUnit = MaxRestrictionTimeUnit,
                    OperatorHasNotRestrictionToInitBatch = OperatorHasNotRestrictionToInitBatch,




                    Order = Order,

                },


                _ => default(T)!
            };
        }
        static Func<IQueryable<SimulationPlanned>, IIncludableQueryable<SimulationPlanned, object>> IQueryHandler<SimulationPlanned>.GetIncludesBy(IDto dto)
        {
            if (dto is SimulationPlannedDTO)
            {
                //return x => x.Include(y => y.Material);
            }

            return null!;

        }
        static Expression<Func<SimulationPlanned, object>> IQueryHandler<SimulationPlanned>.GetOrderBy(IDto dto)
        {
            if (dto is SimulationPlannedDTO)
            {
                return f => f.Name;
            }

            return null!;

        }
        static Expression<Func<SimulationPlanned, bool>> IQueryHandler<SimulationPlanned>.GetFilterBy(IDto dto)
        {
            return dto switch
            {
                SimulationPlannedDTO mappeddto =>

                x => x.MainProcessId == mappeddto.MainProcessId,

                _ => null!
            };
        }
        static Expression<Func<SimulationPlanned, bool>> IValidationRule<SimulationPlanned>.GetExistCriteria(IDto dto, string validationKey)
        {
            return dto switch
            {
                SimulationPlannedDTO mappeddto => validationKey switch
                {
                    nameof(SimulationPlannedDTO.Name) => mappeddto.BuildStringCriteria<SimulationPlanned, SimulationPlannedDTO>(
                         nameof(SimulationPlannedDTO.Name),
                         nameof(SimulationPlanned.Name)),
                    _ => x => false
                },
                _ => x => false
            };
        }
        static Expression<Func<SimulationPlanned, bool>> IValidationRule<SimulationPlanned>.GetIdCriteria(IDto dto)
        {
            return dto switch
            {
                SimulationPlannedDTO mappeddto => x => x.MainProcessId == mappeddto.MainProcessId,

                _ => x => true // DTO no reconocido → sin filtro
            };
        }
    }

}

