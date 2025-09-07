using Simulator.Shared.Commons.IdentityModels.Requests;

namespace Simulator.Server.Interfaces.Storage
{
    public interface IUploadService
    {
        string UploadAsync(UploadRequest request);
    }
}