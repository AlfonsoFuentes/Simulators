using Simulator.Shared.Models.HCs.StreamJoiners;
using Web.Infrastructure.Managers.Generic;

namespace Simulator.Client.Infrastructure.Validators.HCValidators.StreamJoiners
{
    public class StreamJoinerValidator : AbstractValidator<StreamJoinerDTO>
    {
        private readonly IGenericService Service;

        public StreamJoinerValidator(IGenericService service)
        {
            Service = service;

            RuleFor(x => x.Name).NotEmpty().WithMessage("Name must be defined!");



            RuleFor(x => x.Name).MustAsync(ReviewIfNameExist)
                .When(x => !string.IsNullOrEmpty(x.Name))
                .WithMessage(x => $"{x.Name} already exist");



        }

        async Task<bool> ReviewIfNameExist(StreamJoinerDTO request, string name, CancellationToken cancellationToken)
        {
            ValidateStreamJoinerNameRequest validate = new()
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
