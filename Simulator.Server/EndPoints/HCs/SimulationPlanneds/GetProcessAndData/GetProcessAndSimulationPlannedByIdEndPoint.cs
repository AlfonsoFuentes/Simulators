using Simulator.Server.Databases.Entities.HC;
using Simulator.Shared.Models.HCs.SimulationPlanneds;
using Simulator.Shared.Simulations;
namespace Simulator.Server.EndPoints.HCs.SimulationPlanneds.GetProcessAndData
{
    public static class GetProcessAndSimulationPlannedByIdEndPoint
    {
        public class EndPoint : IEndPoint
        {
            public void MapEndPoint(IEndpointRouteBuilder app)
            {
                app.MapPost(StaticClass.SimulationPlanneds.EndPoint.GetProcess, async (GetProcessByIdRequest request, IQueryRepository Repository) =>
                {

                    NewSimulationDTO response = new NewSimulationDTO();
                    await response.ReadSimulationMaterials(Repository);
                    await response.ReadSkuSimulation(Repository);
                    await response.ReadWashoutTime(Repository);
                    await response.ReadLines(request.MainProcessId, Repository);
                    await response.ReadTanks(request.MainProcessId, Repository);
                    await response.ReadMixers(request.MainProcessId, Repository);
                    await response.ReadPumps(request.MainProcessId, Repository);
                    await response.ReadSkids(request.MainProcessId, Repository);
                    await response.ReadOperators(request.MainProcessId, Repository);
                    await response.ReadMaterialEquipments(request.MainProcessId, Repository);
                    await response.ReadConnectors(request.MainProcessId, Repository);
                    await response.ReadSkuLinesSimulation(Repository);
                    await response.ReadPlannedDowntimes(Repository);

                    return Result.Success(response);

                });
            }

        }

    }

    public static class GetPlannedByIdEndPoint
    {
        public class EndPoint : IEndPoint
        {
            public void MapEndPoint(IEndpointRouteBuilder app)
            {
                app.MapPost(StaticClass.SimulationPlanneds.EndPoint.GetPlanned, async (GetPlannedByIdRequest request, IQueryRepository Repository) =>
                {

                    SimulationPlannedDTO response = new SimulationPlannedDTO();
                    response.Id = request.Id;

                    await response.ReadPlannedLines(Repository);
                    await response.ReadPlannedMixers(Repository);
                    return Result.Success(response);

                });
            }

        }
        public static async Task ReadPlanned(this SimulationPlannedDTO request, IQueryRepository Repository)
        {
            Expression<Func<SimulationPlanned, bool>> Criteria = x => x.Id == request.Id;

            string CacheKey = StaticClass.SimulationPlanneds.Cache.GetById(request.Id);
            var row = await Repository.GetAsync(Cache: CacheKey, Criteria: Criteria/*, Includes: includes*/);
            if (row != null)
            {
                request = row.Map();


            }

        }

    }
}
