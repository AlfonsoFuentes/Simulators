using Simulator.Shared.Enums.HCEnums.Enums;

namespace Simulator.Shared.Simulations
{
    public class WashoutSimulation
    {
        public ProductCategory ProductCategoryCurrent { get; set; } = ProductCategory.None;
        public ProductCategory ProductCategoryNext { get; set; } = ProductCategory.None;
        public Amount MixerWashoutTime { get; set; } = new(TimeUnits.Minute);
        public Amount LineWashoutTime { get; set; } = new(TimeUnits.Minute);
    }
}
