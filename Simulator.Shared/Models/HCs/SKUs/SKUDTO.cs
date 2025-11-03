using Simulator.Shared.Enums.HCEnums.Enums;
using Simulator.Shared.Intefaces;
using Simulator.Shared.Models.HCs.BackBoneSteps;
using Simulator.Shared.Models.HCs.Materials;
using System.Text.Json.Serialization;

namespace Simulator.Shared.Models.HCs.SKUs
{

    public class SKUDTO : Dto
    {
        public string Name { get; set; } = string.Empty;
        public string SkuCode { get; set; } = string.Empty;

        public ProductCategory ProductCategory { get; set; } = ProductCategory.None;
        public MaterialDTO BackBone { get; set; } = null!;
        public FocusFactory FocusFactory { get; set; } = FocusFactory.None;
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
   
}
