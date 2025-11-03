using Simulator.Shared.Intefaces;
using System.Text.Json.Serialization;

namespace Simulator.Shared.Models.HCs.EquipmentPlannedDownTimes
{
    public class EquipmentPlannedDownTimeDTO :Dto
    {
       
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }

        public string StartTimeString => StartTime == null ? string.Empty : StartTime.Value.ToString();
        public string EndTimeString => EndTime == null ? string.Empty : EndTime.Value.ToString();
        public Guid MainProcessId { get; set; }
        public Guid BaseEquipmentId { get; set; }
        [JsonIgnore]
        public Amount SpanTime => EndTime == null || StartTime == null ? new Amount(TimeUnits.Second) : new((EndTime!.Value - StartTime!.Value).TotalSeconds, TimeUnits.Second);
        public double SpaValue=> SpanTime.Value;
        public bool Between(DateTime current)
        {
            var retorno = TimeOnly.FromDateTime(current).IsBetween(
                TimeOnly.FromTimeSpan(StartTime!.Value),
                TimeOnly.FromTimeSpan(EndTime!.Value));
            if (retorno)
            {

            }

            return retorno;
        }

    }
    //public class DeleteEquipmentPlannedDownTimeRequest : DeleteMessageResponse, IRequest
    //{
    //    public string Name { get; set; } = string.Empty;
    //    public override string Legend => Name;

    //    public override string ClassName => StaticClass.EquipmentPlannedDownTimes.ClassName;

    //    public Guid Id { get; set; }

    //    public string EndPointName => StaticClass.EquipmentPlannedDownTimes.EndPoint.Delete;
    //}
    //public class GetEquipmentPlannedDownTimeByIdRequest : GetByIdMessageResponse, IGetById
    //{

    //    public Guid Id { get; set; }
    //    public string EndPointName => StaticClass.EquipmentPlannedDownTimes.EndPoint.GetById;
    //    public override string ClassName => StaticClass.EquipmentPlannedDownTimes.ClassName;
    //}
    //public class EquipmentPlannedDownTimeGetAll : IGetAll
    //{
    //    public string EndPointName => StaticClass.EquipmentPlannedDownTimes.EndPoint.GetAll;
    //    public Guid EquipmentId { get; set; }
    //}
    //public class EquipmentPlannedDownTimeResponseList : IResponseAll
    //{
    //    public List<EquipmentPlannedDownTimeDTO> Items { get; set; } = new();
    //}
    ////public class ValidateEquipmentPlannedDownTimeNameRequest : ValidateMessageResponse, IRequest
    ////{
    ////    public Guid? Id { get; set; }
    ////    public string Name { get; set; } = string.Empty;

    ////    public string EndPointName => StaticClass.EquipmentPlannedDownTimes.EndPoint.Validate;

    ////    public override string Legend => Name;

    ////    public override string ClassName => StaticClass.EquipmentPlannedDownTimes.ClassName;
    ////}
    //public class DeleteGroupEquipmentPlannedDownTimeRequest : DeleteMessageResponse, IRequest
    //{

    //    public override string Legend => "Group of EquipmentPlannedDownTime";

    //    public override string ClassName => StaticClass.EquipmentPlannedDownTimes.ClassName;

    //    public HashSet<EquipmentPlannedDownTimeDTO> SelecteItems { get; set; } = null!;

    //    public string EndPointName => StaticClass.EquipmentPlannedDownTimes.EndPoint.DeleteGroup;
    //    public Guid EquipmentId { get; set; }
    //}
    //public class ChangeEquipmentPlannedDownTimeOrderDowmRequest : UpdateMessageResponse, IRequest
    //{

    //    public Guid? Id { get; set; }
    //    public string Name { get; set; } = string.Empty;
    //    public Guid ProductionLineAssignmentId { get; set; }
    //    public string EndPointName => StaticClass.EquipmentPlannedDownTimes.EndPoint.UpdateDown;
    //    public int Order { get; set; }
    //    public override string Legend => Name;

    //    public override string ClassName => StaticClass.EquipmentPlannedDownTimes.ClassName;
    //}
    //public class ChangeEquipmentPlannedDownTimeOrderUpRequest : UpdateMessageResponse, IRequest
    //{
    //    public Guid ProductionLineAssignmentId { get; set; }
    //    public Guid Id { get; set; }
    //    public string Name { get; set; } = string.Empty;
    //    public int Order { get; set; }
    //    public string EndPointName => StaticClass.EquipmentPlannedDownTimes.EndPoint.UpdateUp;

    //    public override string Legend => Name;

    //    public override string ClassName => StaticClass.EquipmentPlannedDownTimes.ClassName;
    //}
    //public static class EquipmentPlannedDownTimeMapper
    //{
    //    public static ChangeEquipmentPlannedDownTimeOrderDowmRequest ToDown(this EquipmentPlannedDownTimeDTO response)
    //    {
    //        return new()
    //        {
    //            Id = response.Id,
    //            Name = response.Name,

    //            Order = response.Order,


    //        };
    //    }
    //    public static ChangeEquipmentPlannedDownTimeOrderUpRequest ToUp(this EquipmentPlannedDownTimeDTO response)
    //    {
    //        return new()
    //        {

    //            Id = response.Id,
    //            Name = response.Name,
    //            Order = response.Order,
    //        };
    //    }

    //}
}
