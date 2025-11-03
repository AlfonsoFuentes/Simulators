using Simulator.Server.Databases.Entities.HC;
using Simulator.Shared.Intefaces;
using Simulator.Shared.Models.HCs.Conectors;

namespace Simulator.Server.EndPoints.HCs.Conectors
{
    public class InletConnectorEndPoint : IEndPoint
    {
        public void MapEndPoint(IEndpointRouteBuilder app)
        {
            app.MapPost("api/save/InletConnectorDTO", async (InletConnectorDTO dto, IServerCrudService service) =>
            {

                var result = await service.SaveAsync<Conector>(dto);

                if (result == 0)
                {
                    return Result.Fail($"Something went wrong Updating {nameof(Conector)} table");
                }


                return Result.Success($"{nameof(Conector)} table Updated");

            });

            app.MapPost("api/getall/InletConnectorDTO", async (InletConnectorDTO dto, IServerCrudService service) =>
            {
                var query = await service.GetAllAsync<Conector>(dto,querysuffix:$"inlet", parentId: $"{dto.ToId}");

                var result = query.Select(x => x.MapToDto<InletConnectorDTO>()).ToList();

                return Result.Success(result);
            });


            // GetById
            app.MapPost("api/getbyid/InletConnectorDTO", async (InletConnectorDTO dto, IServerCrudService service) =>
            {

                var query = await service.GetById<Conector>(dto);

                if (query == null)
                {
                    return Result.Fail("Not Found");
                }

                var result = query.MapToDto<InletConnectorDTO>();
                return Result.Success(result);
            });
            app.MapPost("api/delete/InletConnectorDTO", async (InletConnectorDTO dto, IServerCrudService service) =>
            {
                var result = await service.DeleteAsync<Conector>(dto);
                if (result == 0)
                {
                    return Result.Fail($"{nameof(Conector)} row not found or already deleted.");
                }
                return Result.Success($"{nameof(Conector)} row deleted successfully");
            });

            // 🔹 DeleteGroup
            app.MapPost("api/deletegroup/InletConnectorDTO", async (List<InletConnectorDTO> dtos, IServerCrudService service) =>
            {
                if (dtos == null || dtos.Count == 0)
                {
                    return Result.Fail($"{nameof(Conector)} No items selected for deletion.");
                }

                var result = await service.DeleteGroupAsync<Conector>(dtos.Cast<IDto>().ToList());
                if (result == 0)
                {
                    return Result.Fail($"c No valid items found for deletion.");
                }
                return Result.Success($"Deleted {nameof(Conector)} rows successfully");
            });

            // 🔹 OrderUp
            app.MapPost("api/orderup/InletConnectorDTO", async (InletConnectorDTO dto, IServerCrudService service) =>
            {
                var success = await service.OrderUpAsync<Conector>(dto);
                if (!success)
                {
                    return Result.Fail("Cannot move up: already at the top or father not found.");
                }
                return Result.Success("Conector moved up successfully");
            });

            // 🔹 OrderDown
            app.MapPost("api/orderdown/InletConnectorDTO", async (InletConnectorDTO dto, IServerCrudService service) =>
            {
                var success = await service.OrderDownAsync<Conector>(dto);
                if (!success)
                {
                    return Result.Fail("Cannot move down: already at the bottom or father not found.");
                }
                return Result.Success("Conector moved down successfully");
            });
            // En InletConnectorEndpointExtensions
            app.MapPost("api/validate/InletConnectorDTO", async (InletConnectorDTO dto, IServerCrudService service) =>
            {
                var isValid = await service.ValidateAsync<Conector>(dto);
                return isValid
                    ? Result.Success()
                    : Result.Fail($"{dto.ValidationKey} in {nameof(Conector)} table, already exists");
            });
        }
    }
}
