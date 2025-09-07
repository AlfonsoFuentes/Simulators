namespace Simulator.Shared.Commons.IdentityModels.Requests
{
    public abstract class PagedRequest
    {
        public int PageSize { get; set; }
        public int PageNumber { get; set; }

        public string[] Orderby { get; set; } = null!;
    }
}