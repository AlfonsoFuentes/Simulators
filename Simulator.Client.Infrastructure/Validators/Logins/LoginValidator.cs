using FluentValidation;
using Simulator.Shared.Commons.IdentityModels.Requests.Identity;

namespace Web.Infrastructure.Validators.Login
{
    public class LoginValidator : AbstractValidator<TokenRequest>
    {
        public LoginValidator()
        {
            RuleFor(x => x.Email).NotEmpty().WithMessage("Must supply valid email");
            RuleFor(x => x.Email).EmailAddress().WithMessage("Must supply valid email");
            RuleFor(x => x.Password).NotEmpty().WithMessage("Must supply valid password");
        }
    }
}
