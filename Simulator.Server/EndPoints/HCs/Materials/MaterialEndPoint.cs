using Simulator.Server.Databases.Entities.HC;
using Simulator.Shared.Intefaces;
using Simulator.Shared.Models.HCs.Materials;


namespace Simulator.Server.EndPoints.HCs.Materials
{
    public class MaterialEndPoint : IEndPoint
    {
        public void MapEndPoint(IEndpointRouteBuilder app)
        {
            app.MapPost("api/save/MaterialDTO", async (MaterialDTO dto, IServerCrudService service) =>
            {

                var result = await service.SaveAsync<Material>(dto);

                if (result == 0)
                {
                    return Result.Fail("Something went wrong");
                }


                return Result.Success("Updated");

            });

            app.MapPost("api/getall/MaterialDTO", async (MaterialDTO dto, IServerCrudService service) =>
            {
                var query = await service.GetAllAsync<Material>(dto);

                var result = query.Select(x => x.MapToDto<MaterialDTO>()).ToList();

                return Result.Success(result);
            });
            app.MapPost("api/getall/RawMaterialDto", async (RawMaterialDto dto, IServerCrudService service) =>
            {
                var query = await service.GetAllAsync<Material>(dto);

                var result = query.Select(x => x.MapToDto<RawMaterialDto>()).ToList();

                return Result.Success(result);
            });
            app.MapPost("api/getall/ProductBackBoneDto", async (ProductBackBoneDto dto, IServerCrudService service) =>
            {
                var query = await service.GetAllAsync<Material>(dto);

                var result = query.Select(x => x.MapToDto<ProductBackBoneDto>()).ToList();

                return Result.Success(result);
            });
            app.MapPost("api/getall/BackBoneDto", async (BackBoneDto dto, IServerCrudService service) =>
            {
                var query = await service.GetAllAsync<Material>(dto);

                var result = query.Select(x => x.MapToDto<BackBoneDto>()).ToList();

                return Result.Success(result);
            });

            // GetById
            app.MapPost("api/getbyid/MaterialDTO", async (MaterialDTO dto, IServerCrudService service) =>
            {

                var query = await service.GetById<Material>(dto);

                if (query == null)
                {
                    return Result.Fail("Not Found");
                }

                var result = query.MapToDto<MaterialDTO>();
                return Result.Success(result);
            });
            app.MapPost("api/delete/MaterialDTO", async (MaterialDTO dto, IServerCrudService service) =>
            {
                var result = await service.DeleteAsync<Material>(dto);
                if (result == 0)
                {
                    return Result.Fail("Material not found or already deleted.");
                }
                return Result.Success("Material deleted successfully");
            });

            // 🔹 DeleteGroup
            app.MapPost("api/deletegroup/MaterialDTO", async (List<MaterialDTO> dtos, IServerCrudService service) =>
            {
                if (dtos == null || dtos.Count == 0)
                {
                    return Result.Fail("No items selected for deletion.");
                }

                var result = await service.DeleteGroupAsync<Material>(dtos.Cast<IDto>().ToList());
                if (result == 0)
                {
                    return Result.Fail("No valid items found for deletion.");
                }
                return Result.Success($"Deleted {result} father(s) successfully");
            });

            // 🔹 OrderUp
            app.MapPost("api/orderup/MaterialDTO", async (MaterialDTO dto, IServerCrudService service) =>
            {
                var success = await service.OrderUpAsync<Material>(dto);
                if (!success)
                {
                    return Result.Fail("Cannot move up: already at the top or father not found.");
                }
                return Result.Success("Material moved up successfully");
            });

            // 🔹 OrderDown
            app.MapPost("api/orderdown/MaterialDTO", async (MaterialDTO dto, IServerCrudService service) =>
            {
                var success = await service.OrderDownAsync<Material>(dto);
                if (!success)
                {
                    return Result.Fail("Cannot move down: already at the bottom or father not found.");
                }
                return Result.Success("Material moved down successfully");
            });
            // En MaterialEndpointExtensions
            app.MapPost("api/validate/MaterialDTO", async (MaterialDTO dto, IServerCrudService service) =>
            {
                var isValid = await service.ValidateAsync<Material>(dto);
                return isValid
                    ? Result.Success()
                    : Result.Fail("Name already exists");
            });
        }
    }
  
}