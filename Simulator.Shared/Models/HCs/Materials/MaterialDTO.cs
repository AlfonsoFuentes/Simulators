using Simulator.Shared.Enums.HCEnums.Enums;
using Simulator.Shared.Models.HCs.BackBoneSteps;
using Simulator.Shared.Models.HCs.BaseEquipments;
using static Simulator.Shared.StaticClasses.StaticClass;

namespace Simulator.Shared.Models.HCs.Materials
{

    
    public class RawMaterialGetAll : IGetAll
    {
        public string EndPointName => StaticClass.Materials.EndPoint.GetAllRawMaterial;
    }
    public class RawMaterialSimpleGetAll : IGetAll
    {
        public string EndPointName => StaticClass.Materials.EndPoint.GetAllRawMaterialSimple;
    }
    public class ProductBackBoneGetAll : IGetAll
    {
        public string EndPointName => StaticClass.Materials.EndPoint.GetAllProductBackBone;
    }
    public class RawMaterialBackBoneGetAll : IGetAll
    {
        public string EndPointName => StaticClass.Materials.EndPoint.GetAllRawMaterialBackBone;
    }
    public class BackBoneGetAll : IGetAll
    {
        public string EndPointName => StaticClass.Materials.EndPoint.GetAllBackBone;
    }
    
    public class MaterialDTO : BaseDTO, IMessageResponse, IRequest
    {
        public string EndPointName => StaticClass.Materials.EndPoint.CreateUpdate;

        public string Legend => Name;

        public string ActionType => Id == Guid.Empty ? "created" : "updated";
        public string ClassName => StaticClass.Materials.ClassName;
        public string Succesfully => StaticClass.ResponseMessages.ReponseSuccesfullyMessage(Legend, ClassName, ActionType);
        public string Fail => StaticClass.ResponseMessages.ReponseFailMessage(Legend, ClassName, ActionType);
        public string NotFound => StaticClass.ResponseMessages.ReponseNotFound(ClassName);
        public string M_Number { get; set; } = string.Empty;
        public string SAPName { get; set; } = string.Empty;
        public string CommonName { get; set; } = string.Empty;
        public virtual MaterialType MaterialType { get; set; }
        

        public MaterialPhysicState PhysicalState { get; set; } = MaterialPhysicState.None;
        public ProductCategory ProductCategory { get; set; } = ProductCategory.None;
        public string M_NumberCommonName => $"{M_Number} {SAPName}";
        public string PhysicalStateString => PhysicalState.ToString();
        public List<BackBoneStepDTO> BackBoneSteps { get; set; } = new();
        public bool IsForWashing { get; set; } = false;
        public int GetLasOrderSteps()
        {
            return BackBoneSteps.Count == 0 ? 1 : BackBoneSteps.MaxBy(x => x.Order)!.Order;
        }
        public double SumOfPercentage { get; set; } = 0;
        
        public List<BaseEquipmentDTO> ProcessEquipments { get; set; } = new();

    }
    public class DeleteMaterialRequest : DeleteMessageResponse, IRequest
    {
        public string Name { get; set; } = string.Empty;
        public override string Legend => Name;

        public override string ClassName => StaticClass.Materials.ClassName;

        public Guid Id { get; set; }

        public string EndPointName => StaticClass.Materials.EndPoint.Delete;
    }
    public class GetMaterialByIdRequest : GetByIdMessageResponse, IGetById
    {

        public Guid Id { get; set; }
        public string EndPointName => StaticClass.Materials.EndPoint.GetById;
        public override string ClassName => StaticClass.Materials.ClassName;
    }
    public class MaterialGetAll : IGetAll
    {
        public string EndPointName => StaticClass.Materials.EndPoint.GetAllMaterial;
        public MaterialType MaterialType { get; set; }= MaterialType.None;
    }
    
    public class MaterialResponseList : IResponseAll
    {
        public List<MaterialDTO> Items { get; set; } = new();
    }
    public class ValidateMaterialNameRequest : ValidateMessageResponse, IRequest
    {
        public Guid? Id { get; set; }
        public string SapName { get; set; } = string.Empty;

        public string EndPointName => StaticClass.Materials.EndPoint.ValidateSAPName;

        public override string Legend => SapName;

        public override string ClassName => StaticClass.Materials.ClassName;
    }
    public class ValidateMaterialMNumberRequest : ValidateMessageResponse, IRequest
    {
        public Guid? Id { get; set; }
        public string MNumber { get; set; } = string.Empty;

        public string EndPointName => StaticClass.Materials.EndPoint.ValidateMNumber;

        public override string Legend => MNumber;

        public override string ClassName => StaticClass.Materials.ClassName;
    }
    public class ValidateMaterialCommonNameRequest : ValidateMessageResponse, IRequest
    {
        public Guid? Id { get; set; }
        public string CommonName { get; set; } = string.Empty;

        public string EndPointName => StaticClass.Materials.EndPoint.ValidateCommonName;

        public override string Legend => CommonName;

        public override string ClassName => StaticClass.Materials.ClassName;
    }
    public class DeleteGroupMaterialRequest : DeleteMessageResponse, IRequest
    {

        public override string Legend => "Group of Material";

        public override string ClassName => StaticClass.Materials.ClassName;

        public HashSet<MaterialDTO> SelecteItems { get; set; } = null!;

        public string EndPointName => StaticClass.Materials.EndPoint.DeleteGroup;
    }
    public class ChangeMaterialOrderDowmRequest : UpdateMessageResponse, IRequest
    {

        public Guid? Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public Guid ProductionLineAssignmentId { get; set; }
        public string EndPointName => StaticClass.Materials.EndPoint.UpdateDown;
        public int Order { get; set; }
        public override string Legend => Name;

        public override string ClassName => StaticClass.Materials.ClassName;
    }
    public class ChangeMaterialOrderUpRequest : UpdateMessageResponse, IRequest
    {
        public Guid ProductionLineAssignmentId { get; set; }
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Order { get; set; }
        public string EndPointName => StaticClass.Materials.EndPoint.UpdateUp;

        public override string Legend => Name;

        public override string ClassName => StaticClass.Materials.ClassName;
    }
    public static class MaterialMapper
    {
        public static ChangeMaterialOrderDowmRequest ToDown(this MaterialDTO response)
        {
            return new()
            {
                Id = response.Id,
                Name = response.Name,

                Order = response.Order,


            };
        }
        public static ChangeMaterialOrderUpRequest ToUp(this MaterialDTO response)
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
