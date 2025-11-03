using Simulator.Server.Databases.Entities.HC;
using Simulator.Shared.Intefaces;
using Simulator.Shared.Models.HCs.BackBoneSteps;

namespace Simulator.Server.EndPoints.HCs.BackBoneSteps
{
    public class BackBoneStepEndPoint : IEndPoint
    {
        public void MapEndPoint(IEndpointRouteBuilder app)
        {
            app.MapPost("api/save/BackBoneStepDTO", async (BackBoneStepDTO dto, IServerCrudService service) =>
            {

                var result = await service.SaveAsync<BackBoneStep>(dto);

                if (result == 0)
                {
                    return Result.Fail("Something went wrong");
                }


                return Result.Success("Updated");

            });

            app.MapPost("api/getall/BackBoneStepDTO", async (BackBoneStepDTO dto, IServerCrudService service) =>
            {
                var query = await service.GetAllAsync<BackBoneStep>(dto, parentId: $"{dto.MaterialId}");

                var result = query.Select(x => x.MapToDto< BackBoneStepDTO>()).ToList();

                return Result.Success(result);
            });


            // GetById
            app.MapPost("api/getbyid/BackBoneStepDTO", async (BackBoneStepDTO dto, IServerCrudService service) =>
            {

                var query = await service.GetById<BackBoneStep>(dto, parentId: $"{dto.MaterialId}");

                if (query == null)
                {
                    return Result.Fail("Not Found");
                }

                var result = query.MapToDto< BackBoneStepDTO>();
                return Result.Success(result);
            });
            app.MapPost("api/delete/BackBoneStepDTO", async (BackBoneStepDTO dto, IServerCrudService service) =>
            {
                var result = await service.DeleteAsync<BackBoneStep>(dto);
                if (result == 0)
                {
                    return Result.Fail("BackBoneStep not found or already deleted.");
                }
                return Result.Success("BackBoneStep deleted successfully");
            });

            // 🔹 DeleteGroup
            app.MapPost("api/deletegroup/BackBoneStepDTO", async (List<BackBoneStepDTO> dtos, IServerCrudService service) =>
            {
                if (dtos == null || dtos.Count == 0)
                {
                    return Result.Fail("No items selected for deletion.");
                }

                var result = await service.DeleteGroupAsync<BackBoneStep>(dtos.Cast<IDto>().ToList());
                if (result == 0)
                {
                    return Result.Fail("No valid items found for deletion.");
                }
                return Result.Success($"Deleted {result} father(s) successfully");
            });

            // 🔹 OrderUp
            app.MapPost("api/orderup/BackBoneStepDTO", async (BackBoneStepDTO dto, IServerCrudService service) =>
            {
                var success = await service.OrderUpAsync<BackBoneStep>(dto);
                if (!success)
                {
                    return Result.Fail("Cannot move up: already at the top or father not found.");
                }
                return Result.Success("BackBoneStep moved up successfully");
            });

            // 🔹 OrderDown
            app.MapPost("api/orderdown/BackBoneStepDTO", async (BackBoneStepDTO dto, IServerCrudService service) =>
            {
                var success = await service.OrderDownAsync<BackBoneStep>(dto);
                if (!success)
                {
                    return Result.Fail("Cannot move down: already at the bottom or father not found.");
                }
                return Result.Success("BackBoneStep moved down successfully");
            });
            // En BackBoneStepEndpointExtensions
            app.MapPost("api/validate/BackBoneStepDTO", async (BackBoneStepDTO dto, IServerCrudService service) =>
            {
                var isValid = await service.ValidateAsync<BackBoneStep>(dto);
                return isValid
                    ? Result.Success()
                    : Result.Fail("Name already exists");
            });
        }
    }

}
