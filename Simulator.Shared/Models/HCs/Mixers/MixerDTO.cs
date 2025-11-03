using Simulator.Shared.Enums.HCEnums.Enums;
using Simulator.Shared.Models.HCs.BaseEquipments;
using Simulator.Shared.Models.HCs.Materials;
using System.Text.Json.Serialization;

namespace Simulator.Shared.Models.HCs.Mixers
{
    public class MixerDTO: BaseEquipmentDTO
    {
       
        public override ProccesEquipmentType EquipmentType { get; set; } = ProccesEquipmentType.Mixer;

    }
    //public class DeleteMixerRequest : DeleteMessageResponse, IRequest
    //{
    //    public string Name { get; set; } = string.Empty;
    //    public override string Legend => Name;

    //    public override string ClassName => StaticClass.Mixers.ClassName;

    //    public Guid Id { get; set; }

    //    public string EndPointName => StaticClass.Mixers.EndPoint.Delete;
    //}
    //public class GetMixerByIdRequest : GetByIdMessageResponse, IGetById
    //{

    //    public Guid Id { get; set; }
    //    public string EndPointName => StaticClass.Mixers.EndPoint.GetById;
    //    public override string ClassName => StaticClass.Mixers.ClassName;
    //}
    //public class MixerGetAll : IGetAll
    //{
    //    public string EndPointName => StaticClass.Mixers.EndPoint.GetAll;
    //    public Guid MainProcessId {  get; set; }
    //}
    //public class MixerResponseList : IResponseAll
    //{
    //    public List<MixerDTO> Items { get; set; } = new();
    //}
    //public class ValidateMixerNameRequest : ValidateMessageResponse, IRequest
    //{
    //    public Guid? Id { get; set; }
    //    public string Name { get; set; } = string.Empty;

    //    public string EndPointName => StaticClass.Mixers.EndPoint.Validate;

    //    public override string Legend => Name;

    //    public override string ClassName => StaticClass.Mixers.ClassName;
    //    public Guid MainProcessId { get; set; }
    //}
    //public class DeleteGroupMixerRequest : DeleteMessageResponse, IRequest
    //{

    //    public override string Legend => "Group of Mixer";

    //    public override string ClassName => StaticClass.Mixers.ClassName;

    //    public HashSet<MixerDTO> SelecteItems { get; set; } = null!;

    //    public string EndPointName => StaticClass.Mixers.EndPoint.DeleteGroup;
    //    public Guid MainProcessId {  get; set; }
    //}
    //public class ChangeMixerOrderDowmRequest : UpdateMessageResponse, IRequest
    //{

    //    public Guid? Id { get; set; }
    //    public string Name { get; set; } = string.Empty;
    //    public Guid ProductionLineAssignmentId { get; set; }
    //    public string EndPointName => StaticClass.Mixers.EndPoint.UpdateDown;
    //    public int Order { get; set; }
    //    public override string Legend => Name;

    //    public override string ClassName => StaticClass.Mixers.ClassName;
    //}
    //public class ChangeMixerOrderUpRequest : UpdateMessageResponse, IRequest
    //{
    //    public Guid ProductionLineAssignmentId { get; set; }
    //    public Guid Id { get; set; }
    //    public string Name { get; set; } = string.Empty;
    //    public int Order { get; set; }
    //    public string EndPointName => StaticClass.Mixers.EndPoint.UpdateUp;

    //    public override string Legend => Name;

    //    public override string ClassName => StaticClass.Mixers.ClassName;
    //}
    //public static class MixerMapper
    //{
    //    public static ChangeMixerOrderDowmRequest ToDown(this MixerDTO response)
    //    {
    //        return new()
    //        {
    //            Id = response.Id,
    //            Name = response.Name,
             
    //            Order = response.Order,


    //        };
    //    }
    //    public static ChangeMixerOrderUpRequest ToUp(this MixerDTO response)
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
