using Simulator.Shared.Commons.IdentityModels.Requests.Identity;

namespace Web.Infrastructure.Validators.Login
{
    public class RegisterValidator : AbstractValidator<RegisterRequest>
    {
        public RegisterValidator()
        {
            RuleFor(x => x.Email).NotEmpty().WithMessage("Must supply email");
            RuleFor(x => x.Email).EmailAddress().WithMessage("Must supply valid email");
            RuleFor(x => x.Password).NotEmpty().WithMessage("Must supply valid password");
            RuleFor(x => x.ConfirmPassword).Must(ComparePassWord).WithMessage("Password does not match!");
            RuleFor(x => x.Password).MinimumLength(6).WithMessage("Must supply password with minimum 6 characters");
            RuleFor(x => x.FirstName).NotEmpty().WithMessage("Must supply first name");
            RuleFor(x => x.LastName).NotEmpty().WithMessage("Must supply last name");
            RuleFor(x => x.UserName).NotEmpty().WithMessage("Must supply user name");
            RuleFor(x => x.UserName).MinimumLength(6).WithMessage("Must supply user name with minimum 6 characters");
        }
        bool ComparePassWord(RegisterRequest register, string Password)
        {
            return register.ConfirmPassword.Equals(register.Password);
        }
    }
}
