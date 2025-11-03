using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Simulator.Server.Databases.Contracts;
using Simulator.Shared.Enums.HCEnums.Enums;
using Simulator.Shared.Intefaces;
using Simulator.Shared.Models.HCs.BackBoneSteps;
using Simulator.Shared.Models.HCs.Materials;
using System.ComponentModel.DataAnnotations.Schema;

namespace Simulator.Server.Databases.Entities.HC
{
    public class BackBoneStep : Entity, IMapper, IQueryHandler<BackBoneStep>, IValidationRule<BackBoneStep>, ICreator<BackBoneStep>
    {

        public Material HCMaterial { get; set; } = null!;
        public Guid MaterialId { get; set; }


        public Material? RawMaterial { get; set; } = null!;
        public Guid? RawMaterialId { get; set; }

        public BackBoneStepType BackBoneStepType { get; set; } = BackBoneStepType.None;
        public double Percentage { get; set; }
        public double TimeValue { get; set; }
        public string TimeUnitName { get; set; } = string.Empty;

       
        public static BackBoneStep Create(IDto dto)
        {
            if (dto is BackBoneStepDTO stepdto)
            {
                var result = new BackBoneStep()
                {
                    Id = Guid.NewGuid(),
                };
                result.MapFromDto(stepdto);
                return result;
            }
            return null!;
        }


        public void MapFromDto(IDto dto)
        {
            switch (dto)
            {
                case BackBoneStepDTO request:
                    {
                        BackBoneStepType = request.BackBoneStepType;
                        RawMaterialId = request.RawMaterialId;
                        Percentage = request.Percentage;
                        TimeValue = request.TimeValue;
                        TimeUnitName = request.TimeUnitName;
                        MaterialId = request.MaterialId;
                    }
                    break;

                default:
                    break;

            }

        }

        public static Expression<Func<BackBoneStep, bool>> GetFilterBy(IDto dto)
        {
            if (dto is BackBoneStepDTO request)
            {
                return x => x.MaterialId == request.MaterialId;
            }

            return null!;
        }

        public static Func<IQueryable<BackBoneStep>, IIncludableQueryable<BackBoneStep, object>> GetIncludesBy(IDto dto)
        {
            if (dto is BackBoneStepDTO)
            {
                return f => f.Include(x => x.RawMaterial!);
            }

            return null!;
        }

        public static Expression<Func<BackBoneStep, object>> GetOrderBy(IDto dto)
        {
            if (dto is BackBoneStepDTO)
            {
                return f => f.Order;
            }

            return null!;
        }

        public static Expression<Func<BackBoneStep, bool>> GetIdCriteria(IDto dto)
        {
            if (dto is BackBoneStepDTO request)
            {
                return x => x.MaterialId == request.MaterialId;
            }

            return null!;
        }

        public static Expression<Func<BackBoneStep, bool>> GetExistCriteria(IDto dto, string validationKey)
        {
            throw new NotImplementedException();
        }



        public T MapToDto<T>() where T : IDto, new()
        {
            return typeof(T) switch
            {
                _ when typeof(T) == typeof(BackBoneStepDTO) => (T)(object)new BackBoneStepDTO
                {
                    Id = Id,

                    TimeValue = TimeValue,
                    TimeUnitName = TimeUnitName,
                    Percentage = Percentage,
                    StepRawMaterial = RawMaterial == null ? null! : RawMaterial.MapToDto<RawMaterialDto>(),
                    MaterialId = MaterialId,
                    BackBoneStepType = BackBoneStepType,
                    Order = Order,
                },

                _ => default(T)!
            };
        }

        [ForeignKey("BackBoneStepId")]
        public List<MixerPlanned> MixerPlanneds { get; set; } = new List<MixerPlanned>();

    }

}

