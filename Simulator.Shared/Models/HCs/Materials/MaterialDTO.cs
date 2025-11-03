using Simulator.Shared.Enums.HCEnums.Enums;
using Simulator.Shared.Intefaces;
using Simulator.Shared.Models.HCs.BackBoneSteps;

namespace Simulator.Shared.Models.HCs.Materials
{
    

    public class RawMaterialDto: MaterialDTO
    {
       
    }

    public class ProductBackBoneDto: MaterialDTO
    {

    }
    public class BackBoneDto : MaterialDTO
    {
       
    }
    public class CompletedMaterialDTO : MaterialDTO
    {

    }


    public class MaterialDTO : Dto 
    {
        public FocusFactory FocusFactory { get; set; } = FocusFactory.None;
       
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
      
        public double SumOfPercentage { get; set; } = 0;


    }
   
}
