using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using Simulator.Server.Databases.Contracts;
using Simulator.Shared.Enums.HCEnums.Enums;
using Simulator.Shared.Intefaces;
using Simulator.Shared.Models.HCs.Mixers;
using Simulator.Shared.Models.HCs.PreferedMixers;

namespace Simulator.Server.Databases.Entities.HC
{
    public class PreferedMixer : Entity, IMapper, IQueryHandler<PreferedMixer>, IValidationRule<PreferedMixer>, ICreator<PreferedMixer>
    {

        public Guid MixerId { get; set; }
        public Mixer Mixer { get; set; } = null!;

        public static PreferedMixer Create(Guid LinePlannedId) =>
            new() { Id = Guid.NewGuid(), LinePlannedId = LinePlannedId };

        public LinePlanned LinePlanned { get; set; } = null!;
        public Guid LinePlannedId { get; set; }
        public static PreferedMixer Create(IDto dto)
        {
            if (dto is PreferedMixerDTO mappeddto)
            {
                var entity = new PreferedMixer
                {
                    Id = Guid.NewGuid(),
                    LinePlannedId = mappeddto.LinePlannedId,

                };




                return entity;
            }
            return null!;
        }
        public void MapFromDto(IDto dto)
        {
            switch (dto)
            {
                case PreferedMixerDTO request:
                    {
                        MixerId = request.MixerId;

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
                _ when typeof(T) == typeof(PreferedMixerDTO) => (T)(object)new PreferedMixerDTO
                {
                    Id = Id,

                    Mixer = Mixer == null ? null! : Mixer.MapToDto<MixerDTO>(),


                    Order = Order,

                },


                _ => default(T)!
            };
        }
        static Func<IQueryable<PreferedMixer>, IIncludableQueryable<PreferedMixer, object>> IQueryHandler<PreferedMixer>.GetIncludesBy(IDto dto)
        {
            if (dto is PreferedMixerDTO)
            {
                return x => x
                    .Include(x => x.LinePlanned)
                    .Include(x => x.Mixer);
            }

            return null!;

        }
        static Expression<Func<PreferedMixer, object>> IQueryHandler<PreferedMixer>.GetOrderBy(IDto dto)
        {
            if (dto is PreferedMixerDTO)
            {
                return f => f.Order;
            }

            return null!;

        }
        static Expression<Func<PreferedMixer, bool>> IQueryHandler<PreferedMixer>.GetFilterBy(IDto dto)
        {
            return dto switch
            {
                PreferedMixerDTO mappeddto =>

                x => x.LinePlannedId == mappeddto.LinePlannedId,

                _ => null!
            };
        }
        static Expression<Func<PreferedMixer, bool>> IValidationRule<PreferedMixer>.GetExistCriteria(IDto dto, string validationKey)
        {
            return dto switch
            {
                PreferedMixerDTO mappeddto => validationKey switch
                {
                    //nameof(PreferedMixerDTO.Name) => mappeddto.BuildStringCriteria<PreferedMixer, PreferedMixerDTO>(
                    //     nameof(PreferedMixerDTO.Name),
                    //     nameof(PreferedMixer.Name)),
                    _ => x => false
                },
                _ => x => false
            };
        }
        static Expression<Func<PreferedMixer, bool>> IValidationRule<PreferedMixer>.GetIdCriteria(IDto dto)
        {
            return dto switch
            {
                PreferedMixerDTO mappeddto => x => x.LinePlannedId == mappeddto.LinePlannedId,

                _ => x => true // DTO no reconocido → sin filtro
            };
        }
    }

}

