using System.ComponentModel.DataAnnotations;

namespace Simulator.Shared.Commons.IdentityModels.Requests.Identity
{
    public class RegisterRequest
    {
      
        public string FirstName { get; set; } = string.Empty;

 
        public string LastName { get; set; } = string.Empty;

     
        public string Email { get; set; } = string.Empty;

       
        public string UserName =>$"{FirstName.Trim()}{LastName.Trim()}";

        public string Password { get; set; } = string.Empty;

        public string ConfirmPassword { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public bool ActivateUser { get; set; } = false;
        public bool AutoConfirmEmail { get; set; } = false;
    }
}