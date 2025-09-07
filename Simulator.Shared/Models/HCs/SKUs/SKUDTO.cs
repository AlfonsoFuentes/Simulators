using Simulator.Shared.Enums.HCEnums.Enums;
using Simulator.Shared.Models.HCs.BackBoneSteps;
using Simulator.Shared.Models.HCs.Materials;
using System.Text.Json.Serialization;

namespace Simulator.Shared.Models.HCs.SKUs
{
    
    public class SKUDTO:BaseResponse, IMessageResponse, IRequest
    {
        public string EndPointName => StaticClass.HCSKUs.EndPoint.CreateUpdate;

        public string Legend => Name;

        public string ActionType => Id == Guid.Empty ? "created" : "updated";
        public string ClassName => StaticClass.HCSKUs.ClassName;
        public string Succesfully => StaticClass.ResponseMessages.ReponseSuccesfullyMessage(Legend, ClassName, ActionType);
        public string Fail => StaticClass.ResponseMessages.ReponseFailMessage(Legend, ClassName, ActionType);
        public string NotFound => StaticClass.ResponseMessages.ReponseNotFound(ClassName);
        public string SkuCode { get; set; } = string.Empty;

        public  ProductCategory ProductCategory { get; set; } = ProductCategory.None;
        public  MaterialDTO BackBone { get; set; } = null!;

        public string BackBoneCommonName => BackBone == null ? string.Empty : BackBone.CommonName;
        public string BackBoneM_Number => BackBone == null ? string.Empty : BackBone.M_Number;

        double _SizeValue;
        string _SizeUnitName = VolumeUnits.MilliLiter.Name;
        public double SizeValue
        {
            get => _SizeValue;
            set
            {
                _SizeValue = value;
                if (Size != null)
                    Size = new Amount(_SizeValue, _SizeUnitName);
            }
        }
        public string SizeUnitName
        {
            get => _SizeUnitName;
            set
            {
                _SizeUnitName = value;
                if (Size != null)
                    Size = new Amount(_SizeValue, _SizeUnitName);
            }
        }
        public void ChangeSize()
        {
            _SizeValue = Size.GetValue(Size.Unit);
            _SizeUnitName = Size.UnitName;
        }
        [JsonIgnore]
        public Amount Size { get; set; } = new(VolumeUnits.MilliLiter);
        double _WeigthValue;
        string _WeigthUnitName = MassUnits.Gram.Name;
        public double WeigthValue
        {
            get => _WeigthValue;
            set
            {
                _WeigthValue = value;
                if (Weigth != null)
                    Weigth = new Amount(_WeigthValue, _WeigthUnitName);
            }
        }
        public string WeigthUnitName
        {
            get => _WeigthUnitName;
            set
            {
                _WeigthUnitName = value;
                if (Weigth != null)
                    Weigth = new Amount(_WeigthValue, _WeigthUnitName);
            }
        }
        public void ChangeWeigth()
        {
            _WeigthValue = Weigth.GetValue(Weigth.Unit);
            _WeigthUnitName = Weigth.UnitName;
        }
        [JsonIgnore]
        public Amount Weigth { get; set; } = new(MassUnits.Gram);

        public int EA_Case { get; set; }

        public string SKUCodeName => $"{SkuCode}-{Name}";

        public PackageType PackageType { get; set; } = PackageType.None;

    }
    public class DeleteSKURequest : DeleteMessageResponse, IRequest
    {
        public string Name { get; set; } = string.Empty;
        public override string Legend => Name;

        public override string ClassName => StaticClass.HCSKUs.ClassName;

        public Guid Id { get; set; }

        public string EndPointName => StaticClass.HCSKUs.EndPoint.Delete;
    }
    public class GetSKUByIdRequest : GetByIdMessageResponse, IGetById
    {

        public Guid Id { get; set; }
        public string EndPointName => StaticClass.HCSKUs.EndPoint.GetById;
        public override string ClassName => StaticClass.HCSKUs.ClassName;
    }
    public class SKUGetAll : IGetAll
    {
        public string EndPointName => StaticClass.HCSKUs.EndPoint.GetAll;
 
    }
    public class SKUResponseList : IResponseAll
    {
        public List<SKUDTO> Items { get; set; } = new();
    }
    public class ValidateSKUNameRequest : ValidateMessageResponse, IRequest
    {
        public Guid? Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public string EndPointName => StaticClass.HCSKUs.EndPoint.Validate;

        public override string Legend => Name;

        public override string ClassName => StaticClass.HCSKUs.ClassName;
    }
    public class ValidateSKUCodeRequest : ValidateMessageResponse, IRequest
    {
        public Guid? Id { get; set; }
        public string SkuCode { get; set; } = string.Empty;

        public string EndPointName => StaticClass.HCSKUs.EndPoint.ValidateSKUCode;

        public override string Legend => SkuCode;

        public override string ClassName => StaticClass.HCSKUs.ClassName;
    }
    public class DeleteGroupSKURequest : DeleteMessageResponse, IRequest
    {

        public override string Legend => "Group of SKU";

        public override string ClassName => StaticClass.HCSKUs.ClassName;

        public HashSet<SKUDTO> SelecteItems { get; set; } = null!;

        public string EndPointName => StaticClass.HCSKUs.EndPoint.DeleteGroup;
    }
    public class ChangeSKUOrderDowmRequest : UpdateMessageResponse, IRequest
    {

        public Guid? Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public Guid ProductionLineAssignmentId { get; set; }
        public string EndPointName => StaticClass.HCSKUs.EndPoint.UpdateDown;
        public int Order { get; set; }
        public override string Legend => Name;

        public override string ClassName => StaticClass.HCSKUs.ClassName;
    }
    public class ChangeSKUOrderUpRequest : UpdateMessageResponse, IRequest
    {
        public Guid ProductionLineAssignmentId { get; set; }
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Order { get; set; }
        public string EndPointName => StaticClass.HCSKUs.EndPoint.UpdateUp;

        public override string Legend => Name;

        public override string ClassName => StaticClass.HCSKUs.ClassName;
    }
    public static class SKUMapper
    {
        public static ChangeSKUOrderDowmRequest ToDown(this SKUDTO response)
        {
            return new()
            {
                Id = response.Id,
                Name = response.Name,
             
                Order = response.Order,


            };
        }
        public static ChangeSKUOrderUpRequest ToUp(this SKUDTO response)
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
