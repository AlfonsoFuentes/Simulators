using Simulator.Server.Databases.Entities.HC;
using Simulator.Shared.Enums.HCEnums.Enums;
using Simulator.Shared.Intefaces;
using Simulator.Shared.Models.HCs.SKUs;

namespace Simulator.Server.EndPoints.HCs.SKUs
{
    public class SKUEndPoint : IEndPoint
    {
        public void MapEndPoint(IEndpointRouteBuilder app)
        {
            app.MapPost("api/save/SKUDTO", async (SKUDTO dto, IServerCrudService service) =>
            {

                var result = await service.SaveAsync<SKU>(dto);

                if (result == 0)
                {
                    return Result.Fail("Something went wrong");
                }


                return Result.Success("Updated");

            });

            app.MapPost("api/getall/SKUDTO", async (SKUDTO dto, IServerCrudService service) =>
            {
                var query = await service.GetAllAsync<SKU>(dto);

                var result = query.Select(x => x.MapToDto<SKUDTO>()).ToList();

                return Result.Success(result);
            });
           

            // GetById
            app.MapPost("api/getbyid/SKUDTO", async (SKUDTO dto, IServerCrudService service) =>
            {

                var query = await service.GetById<SKU>(dto);

                if (query == null)
                {
                    return Result.Fail("Not Found");
                }

                var result = query.MapToDto<SKUDTO>();
                return Result.Success(result);
            });
            app.MapPost("api/delete/SKUDTO", async (SKUDTO dto, IServerCrudService service) =>
            {
                var result = await service.DeleteAsync<SKU>(dto);
                if (result == 0)
                {
                    return Result.Fail("SKU not found or already deleted.");
                }
                return Result.Success("SKU deleted successfully");
            });

            // 🔹 DeleteGroup
            app.MapPost("api/deletegroup/SKUDTO", async (List<SKUDTO> dtos, IServerCrudService service) =>
            {
                if (dtos == null || dtos.Count == 0)
                {
                    return Result.Fail("No items selected for deletion.");
                }

                var result = await service.DeleteGroupAsync<SKU>(dtos.Cast<IDto>().ToList());
                if (result == 0)
                {
                    return Result.Fail("No valid items found for deletion.");
                }
                return Result.Success($"Deleted {result} father(s) successfully");
            });

            // 🔹 OrderUp
            app.MapPost("api/orderup/SKUDTO", async (SKUDTO dto, IServerCrudService service) =>
            {
                var success = await service.OrderUpAsync<SKU>(dto);
                if (!success)
                {
                    return Result.Fail("Cannot move up: already at the top or father not found.");
                }
                return Result.Success("SKU moved up successfully");
            });

            // 🔹 OrderDown
            app.MapPost("api/orderdown/SKUDTO", async (SKUDTO dto, IServerCrudService service) =>
            {
                var success = await service.OrderDownAsync<SKU>(dto);
                if (!success)
                {
                    return Result.Fail("Cannot move down: already at the bottom or father not found.");
                }
                return Result.Success("SKU moved down successfully");
            });
            // En SKUEndpointExtensions
            app.MapPost("api/validate/SKUDTO", async (SKUDTO dto, IServerCrudService service) =>
            {
                var isValid = await service.ValidateAsync<SKU>(dto);
                return isValid
                    ? Result.Success()
                    : Result.Fail("Name already exists");
            });
        }
    }

}
