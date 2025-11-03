using System.Reflection;
using System.Runtime.CompilerServices;


namespace Simulator.Server.Databases.Contracts
{
   
    
    public abstract class Entity : IEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string? CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedOn { get; set; }=DateTime.UtcNow;
        public bool IsDeleted { get; set; }
        public DateTime? DeletedOnUtc { get; set; }
        public virtual bool IsTenanted { get; } = false;
        public int Order { get; set; }


    }
}