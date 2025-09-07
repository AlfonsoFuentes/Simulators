using Simulator.Shared.Enums.HCEnums.Enums;
using Simulator.Shared.Models.HCs.LinePlanneds;
using Simulator.Shared.Models.HCs.MainProcesss;
using Simulator.Shared.Models.HCs.MixerPlanneds;
using Simulator.Shared.Models.HCs.Tanks;
using System.Globalization;
using static Simulator.Shared.StaticClasses.StaticClass;

namespace Simulator.Shared.Models.HCs.SimulationPlanneds
{
    public class SimulationPlannedDTO : BaseResponse, IMessageResponse, IRequest
    {
        public string EndPointName => StaticClass.SimulationPlanneds.EndPoint.CreateUpdate;

        public string Legend => Name;

        public string ActionType => Id == Guid.Empty ? "created" : "updated";
        public string ClassName => StaticClass.SimulationPlanneds.ClassName;
        public string Succesfully => StaticClass.ResponseMessages.ReponseSuccesfullyMessage(Legend, ClassName, ActionType);
        public string Fail => StaticClass.ResponseMessages.ReponseFailMessage(Legend, ClassName, ActionType);
        public string NotFound => StaticClass.ResponseMessages.ReponseNotFound(ClassName);
        public Guid MainProcessId { get; set; }
        DateTime _InitDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 6, 0, 0);
        TimeSpan? _InitSpam = new TimeSpan(6, 0, 0);
        public TimeSpan? InitSpam
        {
            get => _InitSpam;
            set
            {
                _InitSpam = value;
                if (_InitSpam != null)
                {
                    _InitDate = new DateTime(_InitDate.Year, _InitDate.Month, _InitDate.Day, _InitSpam.Value.Hours, _InitSpam.Value.Minutes, _InitSpam.Value.Seconds);
                    EndDate = InitDate!.Value.AddHours(PlannedHours);
                }

            }
        }
        public DateTime? InitDate
        {
            get
            {
                return _InitDate;
            }
            set
            {
                _InitDate = value!.Value;
                if (_InitSpam != null)
                {
                    _InitDate = new DateTime(_InitDate.Year, _InitDate.Month, _InitDate.Day, _InitSpam.Value.Hours, _InitSpam.Value.Minutes, _InitSpam.Value.Seconds);
                }
                EndDate = InitDate!.Value.AddHours(PlannedHours);
            }
        }
        CultureInfo ci=new CultureInfo("en-US");
        public string InitDateString => InitDate == null ? string.Empty : InitDate.Value.ToString("f", ci);
        public string EndDateString => EndDate == null ? string.Empty : EndDate.Value.ToString("f", ci);
        public DateTime? EndDate { get; private set; }
        double PlannedHours;
        public double Hours
        {
            get
            {
                return PlannedHours;
            }
            set
            {
                PlannedHours = value;
                EndDate = InitDate!.Value.AddHours(PlannedHours);
            }
        }

       

    
        public List<LinePlannedDTO> PlannedLines { get; set; } = new();
        public List<LinePlannedDTO> OrderedPlannedLines => PlannedLines.OrderBy(x => x.PackageType).ThenBy(x => x.LineName).ToList();
        public List<MixerPlannedDTO> PlannedMixers { get; set; } = new();

        public List<MixerPlannedDTO> OrderedPlannedMixers => PlannedMixers.OrderBy(x => x.MixerName).ToList();
        public CurrentShift CurrentShift => CheckShift();
        CurrentShift CheckShift() =>
             InitDate!.Value.Hour switch
             {
                 >= 6 and < 14 => CurrentShift.Shift_1,
                 >= 14 and < 22 => CurrentShift.Shift_2,
                 _ => CurrentShift.Shift_3
             };
        public bool CheckPlannedShift(ShiftType ShiftType) => CurrentShift switch
        {

            CurrentShift.Shift_1 => ShiftType switch
            {
                ShiftType.Shift_1_2_3 => true,
                ShiftType.Shift_1_2 => true,
                ShiftType.Shift_1_3 => true,
                ShiftType.Shift_1 => true,
                _ => false,
            },
            CurrentShift.Shift_2 => ShiftType switch
            {
                ShiftType.Shift_1_2_3 => true,
                ShiftType.Shift_1_2 => true,
                ShiftType.Shift_2_3 => true,
                ShiftType.Shift_2 => true,
                _ => false,
            },
            CurrentShift.Shift_3 => ShiftType switch
            {
                ShiftType.Shift_1_2_3 => true,
                ShiftType.Shift_2_3 => true,
                ShiftType.Shift_1_3 => true,
                ShiftType.Shift_3 => true,
                _ => false
            },

            _ => false
        };
        //public MainProcessDTO MainProcess { get; private set; } = null!;
        //public List<TankDTO> WIPTanks { get; set; } = new();
        //public List<TankDTO> GetLinesWIPAssociatedtoMixers(MixerPlannedDTO plannedmixer)
        //{
        //    var outletpumps = MainProcess.Conectors.Where(x => x.FromId == plannedmixer.MixerId).Select(x => x.To).ToList();

        //    List<TankDTO> outletWIPs = new List<TankDTO>();

        //    foreach (var pump in outletpumps)
        //    {
        //        foreach (var wip in pump!.OutletConnections)
        //        {
        //            foreach (var pumpwip in wip.Equipment.OutletConnections)
        //            {
        //                foreach (var line in pumpwip.Equipment.OutletConnections)
        //                {
        //                    if (PlannedLines.Any(x => x.LineId == line.Equipment.Id && x.PlannedSKUDTOs.Count > 0
        //                    && CheckPlannedShift(x.ShiftType)))
        //                    {
        //                        var plannedlined = PlannedLines.First(x => x.LineId == line.Equipment.Id);
        //                        var wipdto = WIPTanks.FirstOrDefault(x => x.Id == wip.Equipment.Id);
        //                        outletWIPs.Add(wipdto!);
        //                        var plannedsku = plannedlined.PlannedSKUDTOs.MinBy(x => x.Order);

        //                        plannedmixer.BackBone = plannedsku!.SKU!.BackBone!;

        //                    }
        //                }

        //            }

        //        }
        //    }

        //    return outletWIPs;
        //}

    }
    public class DeleteSimulationPlannedRequest : DeleteMessageResponse, IRequest
    {
        public string Name { get; set; } = string.Empty;
        public override string Legend => Name;

        public override string ClassName => StaticClass.SimulationPlanneds.ClassName;

        public Guid Id { get; set; }

        public string EndPointName => StaticClass.SimulationPlanneds.EndPoint.Delete;
    }
    public class GetSimulationPlannedByIdRequest : GetByIdMessageResponse, IGetById
    {

        public Guid Id { get; set; }
        public string EndPointName => StaticClass.SimulationPlanneds.EndPoint.GetById;
        public override string ClassName => StaticClass.SimulationPlanneds.ClassName;
    }
    public class GetProcessByIdRequest : GetByIdMessageResponse, IGetById
    {
        public Guid Id { get; set; }    
       
        public Guid MainProcessId { get; set; }
        public string EndPointName => StaticClass.SimulationPlanneds.EndPoint.GetProcess;
        public override string ClassName => StaticClass.SimulationPlanneds.ClassName;
    }
    public class GetPlannedByIdRequest : GetByIdMessageResponse, IGetById
    {
        public Guid Id { get; set; }

        public Guid MainProcessId { get; set; }
        public string EndPointName => StaticClass.SimulationPlanneds.EndPoint.GetPlanned;
        public override string ClassName => StaticClass.SimulationPlanneds.ClassName;
    }
    public class SimulationPlannedGetAll : IGetAll
    {
        public string EndPointName => StaticClass.SimulationPlanneds.EndPoint.GetAll;
        public Guid MainProcessId { get; set; }
    }
    public class SimulationPlannedResponseList : IResponseAll
    {
        public List<SimulationPlannedDTO> Items { get; set; } = new();
    }
    public class ValidateSimulationPlannedNameRequest : ValidateMessageResponse, IRequest
    {
        public Guid? Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public string EndPointName => StaticClass.SimulationPlanneds.EndPoint.Validate;

        public override string Legend => Name;

        public override string ClassName => StaticClass.SimulationPlanneds.ClassName;
        public Guid MainProcessId { get; set; }
    }
    public class DeleteGroupSimulationPlannedRequest : DeleteMessageResponse, IRequest
    {

        public override string Legend => "Group of SimulationPlanned";

        public override string ClassName => StaticClass.SimulationPlanneds.ClassName;

        public HashSet<SimulationPlannedDTO> SelecteItems { get; set; } = null!;

        public string EndPointName => StaticClass.SimulationPlanneds.EndPoint.DeleteGroup;
        public Guid MainProcessId { get; set; }
    }
    public class ChangeSimulationPlannedOrderDowmRequest : UpdateMessageResponse, IRequest
    {

        public Guid? Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public Guid ProductionLineAssignmentId { get; set; }
        public string EndPointName => StaticClass.SimulationPlanneds.EndPoint.UpdateDown;
        public int Order { get; set; }
        public override string Legend => Name;

        public override string ClassName => StaticClass.SimulationPlanneds.ClassName;
    }
    public class ChangeSimulationPlannedOrderUpRequest : UpdateMessageResponse, IRequest
    {
        public Guid ProductionLineAssignmentId { get; set; }
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Order { get; set; }
        public string EndPointName => StaticClass.SimulationPlanneds.EndPoint.UpdateUp;

        public override string Legend => Name;

        public override string ClassName => StaticClass.SimulationPlanneds.ClassName;
    }
    public static class SimulationPlannedMapper
    {
        public static ChangeSimulationPlannedOrderDowmRequest ToDown(this SimulationPlannedDTO response)
        {
            return new()
            {
                Id = response.Id,
                Name = response.Name,

                Order = response.Order,


            };
        }
        public static ChangeSimulationPlannedOrderUpRequest ToUp(this SimulationPlannedDTO response)
        {
            return new()
            {

                Id = response.Id,
                Name = response.Name,
                Order = response.Order,
            };
        }

    }
}
