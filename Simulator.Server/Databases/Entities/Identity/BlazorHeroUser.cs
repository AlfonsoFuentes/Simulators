using Microsoft.AspNetCore.Identity;
using Simulator.Server.Databases.Contracts;
using System.ComponentModel.DataAnnotations.Schema;

namespace Simulator.Server.Databases.Entities.Identity
{

    public class BlazorHeroUser : IdentityUser<string>
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? CreatedBy { get; set; }

        [Column(TypeName = "text")]
        public string? ProfilePictureDataUrl { get; set; }

        public DateTime CreatedOn { get; set; }

        public string? LastModifiedBy { get; set; }

        public DateTime? LastModifiedOn { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? DeletedOn { get; set; }
        public bool IsActive { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }

        public DateTime? DeletedOnUtc { get; set; }

        public BlazorHeroUser()
        {
            Id=Guid.NewGuid().ToString();
        }

        public string? CreatedByUserName { get; set; }
        public int Order { get; set; }
    }
}