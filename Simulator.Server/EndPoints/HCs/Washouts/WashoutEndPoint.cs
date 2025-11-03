using Simulator.Server.Databases.Entities.HC;
using Simulator.Shared.Intefaces;
using Simulator.Shared.Models.HCs.Washouts;

namespace Simulator.Server.EndPoints.HCs.Washouts
{
    public class WashoutEndPoint : IEndPoint
    {
        public void MapEndPoint(IEndpointRouteBuilder app)
        {
            app.MapPost("api/save/WashoutDTO", async (WashoutDTO dto, IServerCrudService service) =>
            {

                var result = await service.SaveAsync<Washout>(dto);

                if (result == 0)
                {
                    return Result.Fail($"Something went wrong Updating {nameof(Washout)} table");
                }


                return Result.Success($"{nameof(Washout)} table Updated");

            });

            app.MapPost("api/getall/WashoutDTO", async (WashoutDTO dto, IServerCrudService service) =>
            {
                var query = await service.GetAllAsync<Washout>(dto);

                var result = query.Select(x => x.MapToDto<WashoutDTO>()).ToList();

                return Result.Success(result);
            });


            // GetById
            app.MapPost("api/getbyid/WashoutDTO", async (WashoutDTO dto, IServerCrudService service) =>
            {

                var query = await service.GetById<Washout>(dto);

                if (query == null)
                {
                    return Result.Fail("Not Found");
                }

                var result = query.MapToDto<WashoutDTO>();
                return Result.Success(result);
            });
            app.MapPost("api/delete/WashoutDTO", async (WashoutDTO dto, IServerCrudService service) =>
            {
                var result = await service.DeleteAsync<Washout>(dto);
                if (result == 0)
                {
                    return Result.Fail($"{nameof(Washout)} row not found or already deleted.");
                }
                return Result.Success($"{nameof(Washout)} row deleted successfully");
            });

            // 🔹 DeleteGroup
            app.MapPost("api/deletegroup/WashoutDTO", async (List<WashoutDTO> dtos, IServerCrudService service) =>
            {
                if (dtos == null || dtos.Count == 0)
                {
                    return Result.Fail($"{nameof(Washout)} No items selected for deletion.");
                }

                var result = await service.DeleteGroupAsync<Washout>(dtos.Cast<IDto>().ToList());
                if (result == 0)
                {
                    return Result.Fail($"c No valid items found for deletion.");
                }
                return Result.Success($"Deleted {nameof(Washout)} rows successfully");
            });

            // 🔹 OrderUp
            app.MapPost("api/orderup/WashoutDTO", async (WashoutDTO dto, IServerCrudService service) =>
            {
                var success = await service.OrderUpAsync<Washout>(dto);
                if (!success)
                {
                    return Result.Fail("Cannot move up: already at the top or father not found.");
                }
                return Result.Success("Washout moved up successfully");
            });

            // 🔹 OrderDown
            app.MapPost("api/orderdown/WashoutDTO", async (WashoutDTO dto, IServerCrudService service) =>
            {
                var success = await service.OrderDownAsync<Washout>(dto);
                if (!success)
                {
                    return Result.Fail("Cannot move down: already at the bottom or father not found.");
                }
                return Result.Success("Washout moved down successfully");
            });
            // En WashoutEndpointExtensions
            app.MapPost("api/validate/WashoutDTO", async (WashoutDTO dto, IServerCrudService service) =>
            {
                var isValid = await service.ValidateAsync<Washout>(dto);
                return isValid
                    ? Result.Success()
                    : Result.Fail($"{dto.ValidationKey} in {nameof(Washout)} table, already exists");
            });
        }
    }
  
}
