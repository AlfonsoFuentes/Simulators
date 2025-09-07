using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Simulator.Server.Databases.Entities.Identity;

namespace Simulator.Server.Implementations.Databases
{
    public abstract class AuditableContext : IdentityDbContext<BlazorHeroUser, IdentityRole, string, IdentityUserClaim<string>,
        IdentityUserRole<string>, IdentityUserLogin<string>, IdentityRoleClaim<string>, IdentityUserToken<string>>
    {
        protected AuditableContext(DbContextOptions options) : base(options)
        {
        }




    }
}