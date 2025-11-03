using Simulator.Server.Databases.Entities.HC;
using Simulator.Shared.Models.HCs.ContinuousSystems;
using Simulator.Shared.Models.HCs.Lines;
using Simulator.Shared.Models.HCs.Mixers;
using Simulator.Shared.Models.HCs.Operators;
using Simulator.Shared.Models.HCs.Pumps;
using Simulator.Shared.Models.HCs.SimulationPlanneds;
using Simulator.Shared.Models.HCs.StreamJoiners;
using Simulator.Shared.Models.HCs.Tanks;
using Simulator.Shared.Simulations;
namespace Simulator.Server.EndPoints.HCs.SimulationPlanneds.GetProcessAndData
{
    public static class GetSimulationByIdEndPoint
    {
        public class EndPoint : IEndPoint
        {
            public void MapEndPoint(IEndpointRouteBuilder app)
            {
                app.MapPost("api/getbyid/NewSimulationDTO", async (NewSimulationDTO request,  IServerCrudService service) =>
                {

                    var response = request;
                  
                    await response.ReadSimulationMaterials(service);
                    await response.ReadSkuSimulation(service);
                    await response.ReadWashoutTime(service);

                    await response.ReadCompleteEquipments(service);
                    //await response.ReadLines(service);
                    //await response.ReadStreamJoiners(service);
                    //await response.ReadTanks(service);
                    //await response.ReadMixers(service);
                    //await response.ReadPumps(service);
                    //await response.ReadSkids(service);
                    //await response.ReadOperators(service);
                    await response.ReadMaterialEquipments(service);
                    await response.ReadConnectors(service);
                    await response.ReadSkuLinesSimulation(service);
                    await response.ReadPlannedDowntimes(service);

                    return Result.Success(response);

                });
            }

        }
        public static async Task ReadCompleteEquipments(this NewSimulationDTO simulation, IServerCrudService service)
        {
           
            var rows = await service.GetById<ProcessFlowDiagram>(simulation);


            if (rows != null )
            {
                simulation.Lines=rows.ProccesEquipments.OfType<Line>().Select(x => x.MapToDto<LineDTO>()).ToList();
                simulation.Tanks=rows.ProccesEquipments.OfType<Tank>().Select(x => x.MapToDto<TankDTO>()).ToList();
                simulation.Mixers=rows.ProccesEquipments.OfType<Mixer>().Select(x => x.MapToDto<MixerDTO>()).ToList();
                simulation.Pumps=rows.ProccesEquipments.OfType<Pump>().Select(x => x.MapToDto<PumpDTO>()).ToList();
                simulation.Skids=rows.ProccesEquipments.OfType<ContinuousSystem>().Select(x => x.MapToDto<ContinuousSystemDTO>()).ToList();
                simulation.Operators=rows.ProccesEquipments.OfType<Operator>().Select(x => x.MapToDto<OperatorDTO>()).ToList();
                simulation.StreamJoiners=rows.ProccesEquipments.OfType<StreamJoiner>().Select(x => x.MapToDto<StreamJoinerDTO>()).ToList();

            }



        }

    }

    public static class GetPlannedByIdEndPoint
    {
        public class EndPoint : IEndPoint
        {
            public void MapEndPoint(IEndpointRouteBuilder app)
            {
                app.MapPost("api/getbyid/CompletedSimulationPlannedDTO", async (CompletedSimulationPlannedDTO request, IServerCrudService service) =>
                {

                

                    await request.ReadPlannedLines(service);
                    await request.ReadPlannedMixers(service);
                    return Result.Success(request);

                });
            }

        }
        //public static async Task ReadPlanned(this SimulationPlannedDTO request, IQueryRepository Repository)
        //{
        //    Expression<Func<SimulationPlanned, bool>> Criteria = x => x.Id == request.Id;

        //    string CacheKey = StaticClass.SimulationPlanneds.Cache.GetById(request.Id);
        //    var row = await Repository.GetAsync(Cache: CacheKey, Criteria: Criteria/*, Includes: includes*/);
        //    if (row != null)
        //    {
        //        request = row.Map();


        //    }

        //}

    }




}
