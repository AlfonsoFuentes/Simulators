using Simulator.Shared.Models.HCs.Mixers;
using Web.Infrastructure.Managers.Generic;

namespace Web.Infrastructure.Validators.FinishinLines.Mixers
{

    public class HCMixerValidator : AbstractValidator<MixerDTO>
    {
        private readonly IGenericService Service;

        public HCMixerValidator(IGenericService service)
        {
            Service = service;

            RuleFor(x => x.Name).NotEmpty().WithMessage("Name must be defined!");
            
            
            RuleFor(x => x.Name).MustAsync(ReviewIfNameExist)
                .When(x => !string.IsNullOrEmpty(x.Name))
                .WithMessage(x => $"{x.Name} already exist");

          

        }

        async Task<bool> ReviewIfNameExist(MixerDTO request, string name, CancellationToken cancellationToken)
        {
            ValidateMixerNameRequest validate = new()
            {
                Name = name,
                MainProcessId = request.MainProcessId,

                Id = request.Id

            };
            var result = await Service.Validate(validate);
            return !result;
        }
        
    }
}
