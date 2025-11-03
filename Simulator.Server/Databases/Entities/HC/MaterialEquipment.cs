using DocumentFormat.OpenXml.Spreadsheet;
using Simulator.Server.Databases.Contracts;
using Simulator.Shared.Enums.HCEnums.Enums;
using Simulator.Shared.Intefaces;
using Simulator.Shared.Models.HCs.BackBoneSteps;
using Simulator.Shared.Models.HCs.MaterialEquipments;
using Simulator.Shared.Models.HCs.Materials;
using Simulator.Shared.Models.HCs.Washouts;
using Simulator.Shared.NuevaSimlationconQwen.Equipments;
using System.Security.AccessControl;

namespace Simulator.Server.Databases.Entities.HC
{
    public class MaterialEquipment : Entity, IMapper, IQueryHandler<MaterialEquipment>, IValidationRule<MaterialEquipment>, ICreator<MaterialEquipment>
    {

        public Guid MainProcessId { get; set; } = Guid.Empty;

        public Guid MaterialId { get; set; }
        public Material Material { get; private set; } = null!;

        public BaseEquipment ProccesEquipment { get; private set; } = null!;
        public Guid ProccesEquipmentId { get; set; }
        public double CapacityValue { get; set; }
        public string CapacityUnit { get; set; } = string.Empty;

        public bool IsMixer { get; set; } = false;

        public static MaterialEquipment Create(IDto dto)
        {
            if (dto is MaterialEquipmentDTO materialdto)
            {
                var material = new MaterialEquipment
                {
                    Id = Guid.NewGuid(),
                    ProccesEquipmentId = materialdto.ProccesEquipmentId,
                    MainProcessId = materialdto.MainProcessId,
                };


                return material;
            }
            return null!;
        }
        public void MapFromDto(IDto dto)
        {
            switch (dto)
            {
                case MaterialEquipmentDTO request:
                    {
                        MaterialId = request.MaterialId;
                        CapacityValue = request.CapacityValue;
                        CapacityUnit = request.CapacityUnitName;
                        IsMixer = request.IsMixer;
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
                _ when typeof(T) == typeof(MaterialEquipmentDTO) => (T)(object)new MaterialEquipmentDTO
                {
                    Id = Id,
                    Material = Material == null ? null! : (Material.MapToDto<MaterialDTO>())!,
                    ProccesEquipmentId = ProccesEquipmentId,
                    CapacityValue = CapacityValue,
                    CapacityUnitName = CapacityUnit,
                    IsMixer = IsMixer,
                    Order = Order,
                    MainProcessId = MainProcessId,
                },

                _ => default(T)!
            };
        }
        static Func<IQueryable<MaterialEquipment>, IIncludableQueryable<MaterialEquipment, object>> IQueryHandler<MaterialEquipment>.GetIncludesBy(IDto dto)
        {
            if (dto is MaterialEquipmentDTO)
            {
                return query => query
                    .Include(me => me.Material)
                    .Include(me => me.ProccesEquipment);
            }

            return null!;

        }
        static Expression<Func<MaterialEquipment, object>> IQueryHandler<MaterialEquipment>.GetOrderBy(IDto dto)
        {
            //if (dto is RawMaterialDto)
            //{

            //}
            return null!;

        }
        static Expression<Func<MaterialEquipment, bool>> IQueryHandler<MaterialEquipment>.GetFilterBy(IDto dto)
        {
            if (dto is MaterialEquipmentDTO material)
            {
                return x => x.MainProcessId == material.MainProcessId ;
            }
            return null!;

        }
        static Expression<Func<MaterialEquipment, bool>> IValidationRule<MaterialEquipment>.GetExistCriteria(IDto dto, string validationKey)
        {
            return dto switch
            {

                MaterialEquipmentDTO materialDto => validationKey switch
                {
                    MaterialEquipmentDTO.MaterialEquipmentCombination =>
                        x => x.MaterialId == materialDto.MaterialId &&
                             x.ProccesEquipmentId == materialDto.ProccesEquipmentId &&
                             (materialDto.IsCreated ? x.Id != materialDto.Id : true),
                    _ => x => false
                },



                _ => x => false
            };
        }
        static Expression<Func<MaterialEquipment, bool>> IValidationRule<MaterialEquipment>.GetIdCriteria(IDto dto)
        {
            return dto switch
            {


                _ => x => true // DTO no reconocido → sin filtro
            };
        }
    }

}

