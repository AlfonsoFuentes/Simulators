using Microsoft.AspNetCore.Identity;
using Simulator.Server.Databases.Entities.Identity;
using Simulator.Server.Implementations.Databases;
using Simulator.Shared.Constants.Role;
using Simulator.Shared.Constants.User;

namespace Simulator.Server.Services
{
    public interface IDatabaseSeeder
    {
        void Initialize();
    }
    public class DatabaseSeeder : IDatabaseSeeder
    {

        private readonly BlazorHeroContext _db;
        private readonly UserManager<BlazorHeroUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DatabaseSeeder(
            UserManager<BlazorHeroUser> userManager,
            RoleManager<IdentityRole> roleManager,
            BlazorHeroContext db)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _db = db;

        }

        public void Initialize()
        {
            AddAdministrator();

            _db.SaveChanges();
        }

        private void AddAdministrator()
        {
            Task.Run(async () =>
            {
                //Check if Role Exists

                var adminRoleInDb = await _roleManager.FindByNameAsync(RoleConstants.AdministratorRole);
                if (adminRoleInDb == null)
                {
                    var adminRole = new IdentityRole(RoleConstants.AdministratorRole);
                    await _roleManager.CreateAsync(adminRole);
                    adminRoleInDb = await _roleManager.FindByNameAsync(RoleConstants.AdministratorRole);

                }

                var basicRoleInDb = await _roleManager.FindByNameAsync(RoleConstants.BasicRole);
                if (basicRoleInDb == null)
                {
                    var basicRole = new IdentityRole(RoleConstants.BasicRole);
                    await _roleManager.CreateAsync(basicRole);
                    basicRoleInDb = await _roleManager.FindByNameAsync(RoleConstants.BasicRole);

                }
                //Check if User Exists
                var superUser = new BlazorHeroUser
                {
             
                    FirstName = "Alfonso",
                    LastName = "Fuentes",
                    Email = "alfonsofuen@gmail.com",
                    UserName = "afuentes",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    CreatedOn = DateTime.Now,
                    IsActive = true
                };
                var superUserInDb = await _userManager.FindByEmailAsync(superUser.Email);
                if (superUserInDb == null)
                {
                    await _userManager.CreateAsync(superUser, UserConstants.DefaultPassword);
                    var result = await _userManager.AddToRoleAsync(superUser, RoleConstants.AdministratorRole);
                    if (result.Succeeded)
                    {

                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {

                        }
                    }
                }

            }).GetAwaiter().GetResult();
        }


    }
}
