using Simulator.Shared.Enums.HCEnums.Enums;
using Simulator.Shared.Models.HCs.BaseEquipments;
using Simulator.Shared.Models.HCs.Materials;
using System.Text.Json.Serialization;

namespace Simulator.Shared.Models.HCs.BackBoneSteps
{
    public class BackBoneStepDTO : BaseDTO, IMessageResponse, IRequest
    {

        public string EndPointName => StaticClass.BackBoneSteps.EndPoint.CreateUpdate;

        public string Legend => Name;

        public string ActionType => Id == Guid.Empty ? "created" : "updated";
        public string ClassName => StaticClass.BackBoneSteps.ClassName;
        public string Succesfully => StaticClass.ResponseMessages.ReponseSuccesfullyMessage(Legend, ClassName, ActionType);
        public string Fail => StaticClass.ResponseMessages.ReponseFailMessage(Legend, ClassName, ActionType);
        public string NotFound => StaticClass.ResponseMessages.ReponseNotFound(ClassName);
        public Guid MaterialId { get; set; } = Guid.Empty;
        public Guid? RawMaterialId => StepRawMaterial == null ? null : StepRawMaterial.Id;
        public MaterialDTO StepRawMaterial { get; set; } = null!;


        public BackBoneStepType BackBoneStepType { get; set; } = BackBoneStepType.None;
        public double Percentage { get; set; }
        double _TimeValue;
        string _TimeUnitName = TimeUnits.Minute.Name;
        public double TimeValue
        {
            get => _TimeValue;
            set
            {
                _TimeValue = value;
                if (Time != null)
                    Time=new Amount(_TimeValue, _TimeUnitName);
            }
        }
        public string TimeUnitName
        {
            get => _TimeUnitName;
            set
            {
                _TimeUnitName = value;
                if (Time != null)
                    Time=new Amount(_TimeValue, _TimeUnitName);
            }
        }
        public void ChangeTime()
        {
            _TimeValue = Time.GetValue(Time.Unit);
            _TimeUnitName = Time.UnitName;
        }
        [JsonIgnore]
        public Amount Time { get; set; } = new(TimeUnits.Minute);
       
        public string TimeString => BackBoneStepType != BackBoneStepType.Add ? Time.ToString() : string.Empty;
        public string PercentageString => BackBoneStepType == BackBoneStepType.Add ? Percentage.ToString("0.00") : string.Empty;
        public string StepRawMaterialString => StepRawMaterial == null ? "" : $"{StepRawMaterial.M_NumberCommonName}";

        public string StepName => BackBoneStepType == BackBoneStepType.Add ?
            $"{Order} - {BackBoneStepType} {StepRawMaterialString} {Percentage}%" : BackBoneStepType == BackBoneStepType.Washout ?
            $"{Order} - {BackBoneStepType} {StepRawMaterialString}" :
            $"{Order} - {BackBoneStepType} {Time.ToString()}";

    }
    public class DeleteBackBoneStepRequest : DeleteMessageResponse, IRequest
    {
        public string Name { get; set; } = string.Empty;
        public override string Legend => Name;

        public override string ClassName => StaticClass.BackBoneSteps.ClassName;

        public Guid Id { get; set; }

        public string EndPointName => StaticClass.BackBoneSteps.EndPoint.Delete;
    }
    public class GetBackBoneStepByIdRequest : GetByIdMessageResponse, IGetById
    {

        public Guid Id { get; set; }
        public string EndPointName => StaticClass.BackBoneSteps.EndPoint.GetById;
        public override string ClassName => StaticClass.BackBoneSteps.ClassName;
    }
    public class BackBoneStepGetAll : IGetAll
    {
        public string EndPointName => StaticClass.BackBoneSteps.EndPoint.GetAll;
        public Guid MaterialId {  get; set; }
    }
    public class BackBoneStepResponseList : IResponseAll
    {
        public List<BackBoneStepDTO> Items { get; set; } = new();
    }
    public class ValidateBackBoneStepNameRequest : ValidateMessageResponse, IRequest
    {
        public Guid? Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public string EndPointName => StaticClass.BackBoneSteps.EndPoint.Validate;

        public override string Legend => Name;

        public override string ClassName => StaticClass.BackBoneSteps.ClassName;
    }
    public class DeleteGroupBackBoneStepRequest : DeleteMessageResponse, IRequest
    {

        public override string Legend => "Group of BackBoneStep";

        public override string ClassName => StaticClass.BackBoneSteps.ClassName;

        public HashSet<BackBoneStepDTO> SelecteItems { get; set; } = null!;

        public string EndPointName => StaticClass.BackBoneSteps.EndPoint.DeleteGroup;
        public Guid MaterialId { get; set; }
    }
    public class ChangeBackBoneStepOrderDowmRequest : UpdateMessageResponse, IRequest
    {

        public Guid? Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public Guid MaterialId { get; set; }
        public string EndPointName => StaticClass.BackBoneSteps.EndPoint.UpdateDown;
        public int Order { get; set; }
        public override string Legend => Name;

        public override string ClassName => StaticClass.BackBoneSteps.ClassName;
    }
    public class ChangeBackBoneStepOrderUpRequest : UpdateMessageResponse, IRequest
    {
        public Guid MaterialId { get; set; }
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Order { get; set; }
        public string EndPointName => StaticClass.BackBoneSteps.EndPoint.UpdateUp;

        public override string Legend => Name;

        public override string ClassName => StaticClass.BackBoneSteps.ClassName;
    }
    public static class BackBoneStepMapper
    {
        public static ChangeBackBoneStepOrderDowmRequest ToDown(this BackBoneStepDTO response)
        {
            return new()
            {
                Id = response.Id,
                Name = response.Name,
                MaterialId = response.MaterialId,

                Order = response.Order,


            };
        }
        public static ChangeBackBoneStepOrderUpRequest ToUp(this BackBoneStepDTO response)
        {
            return new()
            {

                Id = response.Id,
                Name = response.Name,
                Order = response.Order,
                MaterialId = response.MaterialId,
            };
        }

    }
}
