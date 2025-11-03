using DocumentFormat.OpenXml.Spreadsheet;
using Simulator.Server.Databases.Contracts;
using Simulator.Shared.Enums.HCEnums.Enums;
using Simulator.Shared.Intefaces;
using Simulator.Shared.Models.HCs.BackBoneSteps;
using Simulator.Shared.Models.HCs.BaseEquipments;
using Simulator.Shared.Models.HCs.Materials;
using Simulator.Shared.Models.HCs.MixerPlanneds;
using Simulator.Shared.Models.HCs.Mixers;

namespace Simulator.Server.Databases.Entities.HC
{
    public class MixerPlanned : Entity, IMapper, IQueryHandler<MixerPlanned>, IValidationRule<MixerPlanned>, ICreator<MixerPlanned>
    {


        public Guid MixerId { get; set; }
        public Mixer Mixer { get; private set; } = null!;

        public SimulationPlanned SimulationPlanned { get; set; } = null!;
        public Guid SimulationPlannedId { get; set; }
        public CurrentMixerState CurrentMixerState { get; set; } = CurrentMixerState.None;
        //public static MixerPlanned Create(Guid SimulationPlannedId)
        //{
        //    var retorno = new MixerPlanned
        //    {
        //        Id = Guid.NewGuid(),
        //        SimulationPlannedId = SimulationPlannedId
        //    };

        //    return retorno;
        //}
        public double MixerLevelValue { get; set; }
        public string MixerLevelUnit { get; set; } = string.Empty;

        public double MixerCapacityValue { get; set; }
        public string MixerCapacityUnit { get; set; } = string.Empty;

        public Guid? BackBoneStepId { get; set; }
        public BackBoneStep? BackBoneStep { get; set; } = null!;


        public Guid? BackBoneId { get; set; }
        public Material? BackBone { get; set; } = null!;

        public Guid? ProducingToId { get; set; }
        public Tank? ProducingTo { get; set; } = null!;

        public static MixerPlanned Create(IDto dto)
        {
            if (dto is MixerPlannedDTO mappeddto)
            {
                var entity = new MixerPlanned
                {
                    Id = Guid.NewGuid(),
                    SimulationPlannedId = mappeddto.SimulationPlannedId,


                };




                return entity;
            }
            return null!;
        }
        public  void MapFromDto(IDto dto)
        {
            switch (dto)
            {
                case MixerPlannedDTO request:
                    {
                        BackBoneStepId = request.BackBoneStep == null ? null : request.BackBoneStep.Id;
                        BackBoneId = request.BackBone.Id;
                        MixerLevelUnit = request.MixerLevelUnitName;
                        MixerLevelValue = request.MixerLevelValue;
                        ProducingToId = request.ProducingTo == null ? null : request.ProducingTo.Id;
                        MixerId = request.MixerId;
                        CurrentMixerState = request.CurrentMixerState;
                        MixerCapacityValue = request.CapacityValue;
                        MixerCapacityUnit = request.CapacityUnitName;

                    }
                    break;

                default:
                    break;

            }
        }
        public  T MapToDto<T>()        where T : IDto,new()
        {
            return typeof(T) switch
            {
                _ when typeof(T) == typeof(MixerPlannedDTO) => (T)(object)new MixerPlannedDTO
                {
                    MixerDTO = Mixer == null ? null! : Mixer.MapToDto<MixerDTO>(),
                    MixerLevelValue =       MixerLevelValue,
                    MixerLevelUnitName = MixerLevelUnit,
                    CurrentMixerState = CurrentMixerState,
                    CapacityValue = MixerCapacityValue,
                    CapacityUnitName = MixerCapacityUnit,
                    ProducingTo = ProducingTo == null ? null! :     ProducingTo!.MapToDto<BaseEquipmentDTO>(),
                    BackBone = BackBone == null ? null! : BackBone.MapToDto<BackBoneDto>(),
                    BackBoneStep = BackBoneStep == null ? null! : BackBoneStep.MapToDto<BackBoneStepDTO>(),
                    Order = Order,
                    SimulationPlannedId = SimulationPlannedId,
                    MainProcesId = SimulationPlanned == null ? Guid.Empty : SimulationPlanned.MainProcessId,

                },


                _ => default(T)!
            };
        }
        static Func<IQueryable<MixerPlanned>, IIncludableQueryable<MixerPlanned, object>> IQueryHandler<MixerPlanned>.GetIncludesBy(IDto dto)
        {
            if (dto is MixerPlannedDTO)
            {
                return x => x
                    .Include(x => x.SimulationPlanned)
                    .Include(y => y.Mixer!)
                    .Include(x => x.BackBone!)
                    .Include(x => x.BackBoneStep!)
                    .Include(x => x.ProducingTo!);
            }

            return null!;

        }
        static Expression<Func<MixerPlanned, object>> IQueryHandler<MixerPlanned>.GetOrderBy(IDto dto)
        {
            if (dto is MixerPlannedDTO)
            {
                return f => f.Order;
            }

            return null!;

        }
        static Expression<Func<MixerPlanned, bool>> IQueryHandler<MixerPlanned>.GetFilterBy(IDto dto)
        {
            return dto switch
            {
                MixerPlannedDTO mappeddto =>

                x => x.SimulationPlannedId == mappeddto.SimulationPlannedId,

                _ => null!
            };
        }
        static Expression<Func<MixerPlanned, bool>> IValidationRule<MixerPlanned>.GetExistCriteria(IDto dto, string validationKey)
        {
            return dto switch
            {
                MixerPlannedDTO mappeddto => validationKey switch
                {
                    //nameof(MixerPlannedDTO.Name) => mappeddto.BuildStringCriteria<MixerPlanned, MixerPlannedDTO>(
                    //     nameof(MixerPlannedDTO.Name),
                    //     nameof(MixerPlanned.Name)),
                    _ => x => false
                },
                _ => x => false
            };
        }
        static Expression<Func<MixerPlanned, bool>> IValidationRule<MixerPlanned>.GetIdCriteria(IDto dto)
        {
            return dto switch
            {
                //MixerPlannedDTO mappeddto => x => x.MainProcessId == mappeddto.MainProcessId,

                _ => x => true // DTO no reconocido → sin filtro
            };
        }

    }

}

