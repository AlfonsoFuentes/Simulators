using Simulator.Shared.Intefaces;
using Simulator.Shared.Models.HCs.BaseEquipments;
using Simulator.Shared.Models.HCs.Mixers;

namespace Simulator.Shared.Models.HCs.PreferedMixers
{
    public class PreferedMixerDTO: Dto
    {
        public string Name { get; set; } = string.Empty;
        public Guid LinePlannedId {  get; set; }
        
        public MixerDTO? Mixer { get; set; } = null!;
        public Guid MixerId => Mixer == null ? Guid.Empty : Mixer.Id;
        public string MixerName => Mixer == null ? "" : Mixer.Name;
        

    }
    //public class DeletePreferedMixerRequest : DeleteMessageResponse, IRequest
    //{
    //    public string Name { get; set; } = string.Empty;
    //    public override string Legend => Name;

    //    public override string ClassName => StaticClass.PreferedMixers.ClassName;

    //    public Guid Id { get; set; }

    //    public string EndPointName => StaticClass.PreferedMixers.EndPoint.Delete;
    //}
    //public class GetPreferedMixerByIdRequest : GetByIdMessageResponse, IGetById
    //{

    //    public Guid Id { get; set; }
    //    public string EndPointName => StaticClass.PreferedMixers.EndPoint.GetById;
    //    public override string ClassName => StaticClass.PreferedMixers.ClassName;
    //}
    //public class PreferedMixerGetAll : IGetAll
    //{
    //    public string EndPointName => StaticClass.PreferedMixers.EndPoint.GetAll;
    //    public Guid LinePlannedId {  get; set; }
    //}
    //public class PreferedMixerResponseList : IResponseAll
    //{
    //    public List<PreferedMixerDTO> Items { get; set; } = new();
    //}
    //public class ValidatePreferedMixerNameRequest : ValidateMessageResponse, IRequest
    //{
    //    public Guid? Id { get; set; }
    //    public string Name { get; set; } = string.Empty;

    //    public string EndPointName => StaticClass.PreferedMixers.EndPoint.Validate;

    //    public override string Legend => Name;

    //    public override string ClassName => StaticClass.PreferedMixers.ClassName;
    //}
    //public class DeleteGroupPreferedMixerRequest : DeleteMessageResponse, IRequest
    //{

    //    public override string Legend => "Group of PreferedMixer";

    //    public override string ClassName => StaticClass.PreferedMixers.ClassName;

    //    public HashSet<PreferedMixerDTO> SelecteItems { get; set; } = null!;

    //    public string EndPointName => StaticClass.PreferedMixers.EndPoint.DeleteGroup;
    //    public Guid LinePlannedId { get; set; }
    //}
    //public class ChangePreferedMixerOrderDowmRequest : UpdateMessageResponse, IRequest
    //{

    //    public Guid? Id { get; set; }
    //    public string Name { get; set; } = string.Empty;
    //    public Guid LinePlannedId { get; set; }
    //    public string EndPointName => StaticClass.PreferedMixers.EndPoint.UpdateDown;
    //    public int Order { get; set; }
    //    public override string Legend => Name;

    //    public override string ClassName => StaticClass.PreferedMixers.ClassName;
    //}
    //public class ChangePreferedMixerOrderUpRequest : UpdateMessageResponse, IRequest
    //{
    //    public Guid LinePlannedId { get; set; }
    //    public Guid Id { get; set; }
    //    public string Name { get; set; } = string.Empty;
    //    public int Order { get; set; }
    //    public string EndPointName => StaticClass.PreferedMixers.EndPoint.UpdateUp;

    //    public override string Legend => Name;

    //    public override string ClassName => StaticClass.PreferedMixers.ClassName;
    //}
    //public static class PreferedMixerMapper
    //{
    //    public static ChangePreferedMixerOrderDowmRequest ToDown(this PreferedMixerDTO response)
    //    {
    //        return new()
    //        {
    //            Id = response.Id,
    //            Name = response.Name,
             
    //            Order = response.Order,
    //            LinePlannedId = response.LinePlannedId, 


    //        };
    //    }
    //    public static ChangePreferedMixerOrderUpRequest ToUp(this PreferedMixerDTO response)
    //    {
    //        return new()
    //        {
          
    //            Id = response.Id,
    //            Name = response.Name,
    //            Order = response.Order,
    //            LinePlannedId = response.LinePlannedId,
    //        };
    //    }

    //}
}
