using Simulator.Shared.Commons.Uploads;

namespace Simulator.Shared.Commons.IdentityModels.Requests
{
    public class UploadRequest
    {
        public string FileName { get; set; } = string.Empty;
        public string Extension { get; set; } = string.Empty;
        public UploadType UploadType { get; set; }
        public byte[] Data { get; set; } = null!;
    }
}