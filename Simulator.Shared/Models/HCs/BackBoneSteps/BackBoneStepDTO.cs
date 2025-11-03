using Simulator.Shared.Enums.HCEnums.Enums;
using Simulator.Shared.Intefaces;
using Simulator.Shared.Models.HCs.BaseEquipments;
using Simulator.Shared.Models.HCs.Materials;
using System.Text.Json.Serialization;

namespace Simulator.Shared.Models.HCs.BackBoneSteps
{
    public class BackBoneStepDTO : Dto
    {

       
        public Guid MaterialId { get; set; } = Guid.Empty;
        public Guid? RawMaterialId => StepRawMaterial == null ? null : StepRawMaterial.Id;
        public RawMaterialDto StepRawMaterial { get; set; } = null!;

        

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
    
}
