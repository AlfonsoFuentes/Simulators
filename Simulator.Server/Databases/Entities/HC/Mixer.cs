using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Simulator.Server.Databases.Contracts;
using Simulator.Server.ExtensionsMethods.Validations;
using Simulator.Shared.Enums.HCEnums.Enums;
using Simulator.Shared.Intefaces;
using Simulator.Shared.Models.HCs.Mixers;
using System.ComponentModel.DataAnnotations.Schema;

namespace Simulator.Server.Databases.Entities.HC
{
    public class Mixer : BaseEquipment, IMapper, IQueryHandler<Mixer>, IValidationRule<Mixer>, ICreator<Mixer>
    {
        public static Mixer Create(Guid mainId) => new()
        {
            Id = Guid.NewGuid(),
            MainProcessId = mainId,

        };
        [ForeignKey("MixerId")]
        public List<MixerPlanned> MixerPlanneds { get; set; } = new();

        public LinePlanned? LinePlanned { get; set; } = null!;
        public Guid? LinePlannedId { get; set; } = null!;

        [ForeignKey("MixerId")]
        public List<PreferedMixer> PreferedMixers { get; set; } = new();

        public static Mixer Create(IDto dto)
        {
            if (dto is MixerDTO mappeddto)
            {
                var entity = new Mixer
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
                case MixerDTO request:
                    {

                        ProccesEquipmentType = ProccesEquipmentType.Mixer;
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
                _ when typeof(T) == typeof(MixerDTO) => (T)(object)new MixerDTO
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
        static Func<IQueryable<Mixer>, IIncludableQueryable<Mixer, object>> IQueryHandler<Mixer>.GetIncludesBy(IDto dto)
        {
            if (dto is MixerDTO)
            {
                //return x => x.Include(y => y.Material);
            }

            return null!;

        }
        static Expression<Func<Mixer, object>> IQueryHandler<Mixer>.GetOrderBy(IDto dto)
        {
            if (dto is MixerDTO)
            {
                return f => f.Name;
            }

            return null!;

        }
        static Expression<Func<Mixer, bool>> IQueryHandler<Mixer>.GetFilterBy(IDto dto)
        {
            return dto switch
            {
                MixerDTO mappeddto =>

                 x => x.MainProcessId == mappeddto.MainProcessId,

                _ => null!
            };
        }
        static Expression<Func<Mixer, bool>> IValidationRule<Mixer>.GetExistCriteria(IDto dto, string validationKey)
        {
            return dto switch
            {
                MixerDTO mappeddto => validationKey switch
                {
                    nameof(MixerDTO.Name) => mappeddto.BuildStringCriteria<Mixer, MixerDTO>(
                         nameof(MixerDTO.Name),
                         nameof(Mixer.Name)),
                    _ => x => false
                },
                _ => x => false
            };
        }
        static Expression<Func<Mixer, bool>> IValidationRule<Mixer>.GetIdCriteria(IDto dto)
        {
            return dto switch
            {
                MixerDTO mappeddto => x => x.MainProcessId == mappeddto.MainProcessId,

                _ => x => true // DTO no reconocido → sin filtro
            };
        }
    }

}

