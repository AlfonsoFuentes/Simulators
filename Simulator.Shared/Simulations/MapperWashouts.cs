using Simulator.Shared.Models.HCs.Washouts;

namespace Simulator.Shared.Simulations
{
    public static class MapperWashouts
    {
        public static void MapWashouts(this NewSimulation simulation, List<WashoutDTO> WashouTimes)
        {
            simulation.WashouTimes = WashouTimes.Select(x => x.MapWashout()).ToList();
        }
        public static WashoutSimulation MapWashout(this WashoutDTO washout)
        {
            return new()
            {
                LineWashoutTime = washout.LineWashoutTime,
                MixerWashoutTime = washout.MixerWashoutTime,
                ProductCategoryCurrent = washout.ProductCategoryCurrent,
                ProductCategoryNext = washout.ProductCategoryNext,
            };
        }
    }
}
