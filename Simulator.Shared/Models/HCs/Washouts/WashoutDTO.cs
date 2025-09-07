using Simulator.Shared.Enums.HCEnums.Enums;
using System.Text.Json.Serialization;

namespace Simulator.Shared.Models.HCs.Washouts
{
    public class WashoutDTO:BaseResponse, IMessageResponse, IRequest
    {
        public string EndPointName => StaticClass.Washouts.EndPoint.CreateUpdate;

        public string Legend => Name;

        public string ActionType => Id == Guid.Empty ? "created" : "updated";
        public string ClassName => StaticClass.Washouts.ClassName;
        public string Succesfully => StaticClass.ResponseMessages.ReponseSuccesfullyMessage(Legend, ClassName, ActionType);
        public string Fail => StaticClass.ResponseMessages.ReponseFailMessage(Legend, ClassName, ActionType);
        public string NotFound => StaticClass.ResponseMessages.ReponseNotFound(ClassName);
        public ProductCategory ProductCategoryCurrent { get; set; } = ProductCategory.None;
        public ProductCategory ProductCategoryNext { get; set; } = ProductCategory.None;
        double _MixerWashoutValue;
        string _MixerWashoutUnitName = TimeUnits.Minute.Name;
        public double MixerWashoutValue
        {
            get => _MixerWashoutValue;
            set
            {
                _MixerWashoutValue = value;
                if (MixerWashoutTime != null)
                    MixerWashoutTime=new Amount(_MixerWashoutValue, _MixerWashoutUnitName);
            }
        }
        public string MixerWashoutUnitName
        {
            get => _MixerWashoutUnitName;
            set
            {
                _MixerWashoutUnitName = value;
                if (MixerWashoutTime != null)
                    MixerWashoutTime=new Amount(_MixerWashoutValue, _MixerWashoutUnitName);
            }
        }
        public void ChangeMixerWashout()
        {
            _MixerWashoutValue = MixerWashoutTime.GetValue(MixerWashoutTime.Unit);
            _MixerWashoutUnitName = MixerWashoutTime.UnitName;
        }
        [JsonIgnore]
        public Amount MixerWashoutTime { get; set; } = new(TimeUnits.Minute);
        double _LineWashoutValue;
        string _LineWashoutUnitName = TimeUnits.Minute.Name;
        public double LineWashoutValue
        {
            get => _LineWashoutValue;
            set
            {
                _LineWashoutValue = value;
                if (LineWashoutTime != null)
                    LineWashoutTime=new Amount(_LineWashoutValue, _LineWashoutUnitName);
            }
        }
        public string LineWashoutUnitName
        {
            get => _LineWashoutUnitName;
            set
            {
                _LineWashoutUnitName = value;
                if (LineWashoutTime != null)
                    LineWashoutTime=new Amount(_LineWashoutValue, _LineWashoutUnitName);
            }
        }
        public void ChangeLineWashout()
        {
            _LineWashoutValue = LineWashoutTime.GetValue(LineWashoutTime.Unit);
            _LineWashoutUnitName = LineWashoutTime.UnitName;
        }
        [JsonIgnore]
        public Amount LineWashoutTime { get; set; } = new(TimeUnits.Minute);

    }
    public class DeleteWashoutRequest : DeleteMessageResponse, IRequest
    {
        public string Name { get; set; } = string.Empty;
        public override string Legend => Name;

        public override string ClassName => StaticClass.Washouts.ClassName;

        public Guid Id { get; set; }

        public string EndPointName => StaticClass.Washouts.EndPoint.Delete;
    }
    public class GetWashoutByIdRequest : GetByIdMessageResponse, IGetById
    {

        public Guid Id { get; set; }
        public string EndPointName => StaticClass.Washouts.EndPoint.GetById;
        public override string ClassName => StaticClass.Washouts.ClassName;
    }
    public class WashoutGetAll : IGetAll
    {
        public string EndPointName => StaticClass.Washouts.EndPoint.GetAll;
    }
    public class WashoutResponseList : IResponseAll
    {
        public List<WashoutDTO> Items { get; set; } = new();
    }
    public class ValidateWashoutRequest : ValidateMessageResponse, IRequest
    {
        public Guid? Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public string EndPointName => StaticClass.Washouts.EndPoint.Validate;

        public override string Legend => Name;

        public override string ClassName => StaticClass.Washouts.ClassName;
        public ProductCategory Current {  get; set; }
        public ProductCategory Next { get; set; }

    }
    public class DeleteGroupWashoutRequest : DeleteMessageResponse, IRequest
    {

        public override string Legend => "Group of Washout";

        public override string ClassName => StaticClass.Washouts.ClassName;

        public HashSet<WashoutDTO> SelecteItems { get; set; } = null!;

        public string EndPointName => StaticClass.Washouts.EndPoint.DeleteGroup;
    }
    public class ChangeWashoutOrderDowmRequest : UpdateMessageResponse, IRequest
    {

        public Guid? Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public Guid ProductionLineAssignmentId { get; set; }
        public string EndPointName => StaticClass.Washouts.EndPoint.UpdateDown;
        public int Order { get; set; }
        public override string Legend => Name;

        public override string ClassName => StaticClass.Washouts.ClassName;
    }
    public class ChangeWashoutOrderUpRequest : UpdateMessageResponse, IRequest
    {
        public Guid ProductionLineAssignmentId { get; set; }
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Order { get; set; }
        public string EndPointName => StaticClass.Washouts.EndPoint.UpdateUp;

        public override string Legend => Name;

        public override string ClassName => StaticClass.Washouts.ClassName;
    }
    public static class WashoutMapper
    {
        public static ChangeWashoutOrderDowmRequest ToDown(this WashoutDTO response)
        {
            return new()
            {
                Id = response.Id,
                Name = response.Name,
             
                Order = response.Order,


            };
        }
        public static ChangeWashoutOrderUpRequest ToUp(this WashoutDTO response)
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
