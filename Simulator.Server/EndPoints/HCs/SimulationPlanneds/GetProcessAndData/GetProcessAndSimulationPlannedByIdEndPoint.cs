using Microsoft.AspNetCore.Mvc;
using Simulator.Server.Databases.Entities.HC;
using Simulator.Shared.Models.HCs.SimulationPlanneds;
using Simulator.Shared.Simulations;
using System.Threading.Channels;
namespace Simulator.Server.EndPoints.HCs.SimulationPlanneds.GetProcessAndData
{
    public static class GetSimulationByIdEndPoint
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

  

    //public static class GetSimulationByIdEndPoint2
    //{
    //    public class EndPoint : IEndPoint
    //    {
    //        public void MapEndPoint(IEndpointRouteBuilder app)
    //        {
    //            app.MapPost(StaticClass.SimulationPlanneds.EndPoint.GetProcess, async (GetProcessByIdRequest request, IQueryRepository Repository, CancellationToken ct) =>
    //            {
    //                var response = new NewSimulationDTO();

    //                // Canal para enviar actualizaciones en tiempo real
    //                var channel = Channel.CreateUnbounded<ProgressUpdate>();

    //                // Lista de tareas a ejecutar en paralelo
    //                var tasks = new List<Task>
    //            {
    //                ExecuteAndNotifyAsync(() => response.ReadSimulationMaterials(Repository), "Leyendo materiales...", channel.Writer, ct),
    //                ExecuteAndNotifyAsync(() => response.ReadSkuSimulation(Repository), "Leyendo SKU de simulación...", channel.Writer, ct),
    //                ExecuteAndNotifyAsync(() => response.ReadWashoutTime(Repository), "Leyendo tiempos de lavado...", channel.Writer, ct),
    //                ExecuteAndNotifyAsync(() => response.ReadLines(request.MainProcessId, Repository), "Leyendo líneas...", channel.Writer, ct),
    //                ExecuteAndNotifyAsync(() => response.ReadTanks(request.MainProcessId, Repository), "Leyendo tanques...", channel.Writer, ct),
    //                ExecuteAndNotifyAsync(() => response.ReadMixers(request.MainProcessId, Repository), "Leyendo mezcladores...", channel.Writer, ct),
    //                ExecuteAndNotifyAsync(() => response.ReadPumps(request.MainProcessId, Repository), "Leyendo bombas...", channel.Writer, ct),
    //                ExecuteAndNotifyAsync(() => response.ReadSkids(request.MainProcessId, Repository), "Leyendo skids...", channel.Writer, ct),
    //                ExecuteAndNotifyAsync(() => response.ReadOperators(request.MainProcessId, Repository), "Leyendo operadores...", channel.Writer, ct),
    //                ExecuteAndNotifyAsync(() => response.ReadMaterialEquipments(request.MainProcessId, Repository), "Leyendo equipos de material...", channel.Writer, ct),
    //                ExecuteAndNotifyAsync(() => response.ReadConnectors(request.MainProcessId, Repository), "Leyendo conectores...", channel.Writer, ct),
    //                ExecuteAndNotifyAsync(() => response.ReadSkuLinesSimulation(Repository), "Leyendo SKU en líneas...", channel.Writer, ct),
    //                ExecuteAndNotifyAsync(() => response.ReadPlannedDowntimes(Repository), "Leyendo paradas planificadas...", channel.Writer, ct)
    //            };

    //                // Iniciar todas las tareas en paralelo
    //                _ = Task.WhenAll(tasks); // No await aquí — queremos que corran en background

    //                // Devolver stream SSE
    //                return Results.Stream<IAsyncEnumerable<ProgressUpdate>>(
    //                    async (outputStream, cancel) =>
    //                    {
    //                        await using var writer = new AsyncTextWriter(outputStream);

    //                        // Leer del canal y enviar al cliente
    //                        await foreach (var update in channel.Reader.ReadAllAsync(cancel))
    //                        {
    //                            await writer.WriteAsync($" {JsonSerializer.Serialize(update)}\n\n");
    //                            await writer.FlushAsync();
    //                        }

    //                        // Enviar mensaje final con el resultado completo
    //                        await writer.WriteAsync($" {JsonSerializer.Serialize(new ProgressUpdate { Message = "Simulación completada", IsComplete = true, Data = response })}\n\n");
    //                        await writer.FlushAsync();
    //                    },
    //                    contentType: "text/event-stream",
    //                    cancellationToken: ct
    //                );
    //            })
    //            .WithMetadata(new ProducesResponseTypeAttribute(200, "text/event-stream"));
    //        }
    //    }

    //    // 👇 Clase para el mensaje de progreso (debe estar en Server y Shared)
    //    public class ProgressUpdate
    //    {
    //        public string Message { get; set; } = string.Empty;
    //        public bool IsComplete { get; set; } = false;
    //        public NewSimulationDTO? Data { get; set; }
    //    }

    //    // 👇 Método auxiliar que ejecuta una tarea y notifica
    //    private static async Task ExecuteAndNotifyAsync(
    //        Func<Task> action,
    //        string message,
    //        ChannelWriter<ProgressUpdate> writer,
    //        CancellationToken ct)
    //    {
    //        try
    //        {
    //            await action(); // Ejecuta la tarea
    //            await writer.WriteAsync(new ProgressUpdate { Message = message, IsComplete = false }, ct);
    //        }
    //        catch (Exception ex)
    //        {
    //            await writer.WriteAsync(new ProgressUpdate { Message = $"Error: {message} - {ex.Message}", IsComplete = false }, ct);
    //        }
    //    }
    //}
}
