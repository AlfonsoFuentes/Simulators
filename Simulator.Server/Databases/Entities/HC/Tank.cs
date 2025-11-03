using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Simulator.Server.Databases.Contracts;
using Simulator.Server.ExtensionsMethods.Validations;
using Simulator.Shared.Enums.HCEnums.Enums;
using Simulator.Shared.Intefaces;
using Simulator.Shared.Models.HCs.Tanks;
using System.ComponentModel.DataAnnotations.Schema;

namespace Simulator.Server.Databases.Entities.HC
{
    public class Tank : BaseEquipment, IMapper, IQueryHandler<Tank>, IValidationRule<Tank>, ICreator<Tank>
    {
        //public static Tank Create(Guid mainId) => new() { Id = Guid.NewGuid(), MainProcessId = mainId };


        public double CapacityValue { get; set; }
        public string CapacityUnit { get; set; } = string.Empty;
        public double MaxLevelValue { get; set; }
        public string MaxLevelUnit { get; set; } = string.Empty;
        public double MinLevelValue { get; set; }
        public string MinLevelUnit { get; set; } = string.Empty;
        public double LoLoLevelValue { get; set; }
        public string LoLoLevelUnit { get; set; } = string.Empty;
        public double InitialLevelValue { get; set; }
        public string InitialLevelUnit { get; set; } = string.Empty;

        public FluidToStorage FluidStorage { get; set; } = FluidToStorage.None;

        public bool IsStorageForOneFluid { get; set; } = false;

        public TankCalculationType TankCalculationType { get; set; } = TankCalculationType.None;

        [ForeignKey("ProducingToId")]
        public List<MixerPlanned> MixerPlanneds { get; set; } = new List<MixerPlanned>();
        public static Tank Create(IDto dto)
        {
            if (dto is TankDTO mappeddto)
            {
                var entity = new Tank
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
                case TankDTO request:
                    {
                        CapacityValue = request.CapacityValue;
                        CapacityUnit = request.CapacityUnitName;
                        Name = request.Name;
                        MaxLevelUnit = request.MaxLevelUnitName;
                        MaxLevelValue = request.MaxLevelValue;
                        MinLevelValue = request.MinLevelValue;
                        MinLevelUnit = request.MinLevelUnitName;
                        LoLoLevelUnit = request.LoLoLevelUnitName;
                        LoLoLevelValue = request.LoLoLevelValue;
                        IsStorageForOneFluid = request.IsStorageForOneFluid;
                        TankCalculationType = request.TankCalculationType;
                        FluidStorage = request.FluidStorage;
                        InitialLevelValue = request.InitialLevelValue;
                        InitialLevelUnit = request.InitialLevelUnitName;
                        FocusFactory = request.FocusFactory;
                        ProccesEquipmentType = ProccesEquipmentType.Tank;

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
                _ when typeof(T) == typeof(TankDTO) => (T)(object)new TankDTO
                {
                    Id = Id,
                    MainProcessId = MainProcessId,
                 
                    CapacityValue = CapacityValue,
                    CapacityUnitName = CapacityUnit,
                    MaxLevelValue = MaxLevelValue,
                    MaxLevelUnitName = MaxLevelUnit,
                    MinLevelUnitName = MinLevelUnit,
                    MinLevelValue = MinLevelValue,
                    LoLoLevelUnitName = LoLoLevelUnit,
                    LoLoLevelValue = LoLoLevelValue,
                    IsStorageForOneFluid = IsStorageForOneFluid,
                    FluidStorage = FluidStorage,
                    TankCalculationType = TankCalculationType,
                    InitialLevelUnitName = InitialLevelUnit,
                    InitialLevelValue = InitialLevelValue,
                    Name = Name,

                    FocusFactory = FocusFactory,
                    Order = Order,
                    EquipmentType = ProccesEquipmentType.Tank,

                },


                _ => default(T)!
            };
        }
        static Func<IQueryable<Tank>, IIncludableQueryable<Tank, object>> IQueryHandler<Tank>.GetIncludesBy(IDto dto)
        {
            if (dto is TankDTO)
            {
                //return x => x.Include(y => y.Material);
            }

            return null!;

        }
        static Expression<Func<Tank, object>> IQueryHandler<Tank>.GetOrderBy(IDto dto)
        {
            if (dto is TankDTO)
            {
                return f => f.Name;
            }

            return null!;

        }
        static Expression<Func<Tank, bool>> IQueryHandler<Tank>.GetFilterBy(IDto dto)
        {
            return dto switch
            {
                TankDTO mappeddto =>

                 x => x.MainProcessId == mappeddto.MainProcessId,

                _ => null!
            };
        }
        static Expression<Func<Tank, bool>> IValidationRule<Tank>.GetExistCriteria(IDto dto, string validationKey)
        {
            return dto switch
            {
                TankDTO mappeddto => validationKey switch
                {
                    nameof(TankDTO.Name) => mappeddto.BuildStringCriteria<Tank, TankDTO>(
                         nameof(TankDTO.Name),
                         nameof(Tank.Name)),
                    _ => x => false
                },
                _ => x => false
            };
        }
        static Expression<Func<Tank, bool>> IValidationRule<Tank>.GetIdCriteria(IDto dto)
        {
            return dto switch
            {
                TankDTO mappeddto => x => x.MainProcessId == mappeddto.MainProcessId,

                _ => x => true // DTO no reconocido → sin filtro
            };
        }

    }

}

