using Simulator.Shared.Enums.HCEnums.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulator.Shared.NuevaSimlationconQwen
{
    public class WashoutTime
    {
        public ProductCategory ProductCategoryCurrent { get; set; } = ProductCategory.None;
        public ProductCategory ProductCategoryNext { get; set; } = ProductCategory.None;
        public Amount MixerWashoutTime {  get; set; }=new Amount(0,TimeUnits.Minute);
        public Amount LineWashoutTime { get; set; } = new Amount(0, TimeUnits.Minute);
    }
}
