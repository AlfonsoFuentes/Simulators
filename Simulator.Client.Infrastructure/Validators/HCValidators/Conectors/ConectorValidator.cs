using FluentValidation;
using Simulator.Shared.Models.HCs.Conectors;
using Web.Infrastructure.Managers.Generic;

namespace Web.Infrastructure.Validators.FinishinLines.Conectors
{

    public class InletConectorValidator : AbstractValidator<InletConnectorDTO>
    {
        private readonly IGenericService Service;

        public InletConectorValidator(IGenericService service)
        {
            Service = service;

            RuleFor(x => x.Froms.Count).NotEqual(0).WithMessage("From equipment must be defined!");


            RuleFor(x => x).MustAsync(ReviewIfInletExist)
               .When(x => x.From != null & x.ToId != Guid.Empty)
               .WithMessage(x => $"Connection -> From: {x.From!.Name} already exist");


        }


        async Task<bool> ReviewIfInletExist(ConectorDTO request, ConectorDTO dto, CancellationToken cancellationToken)
        {
            ValidateInletConectorNameRequest validate = new()
            {
                FromId = request.FromId,
                ToId = request.ToId,


                Id = request.Id

            };
            var result = await Service.Validate(validate);
            return !result;
        }

    }
    public class OutletConectorValidator : AbstractValidator<OutletConnectorDTO>
    {
        private readonly IGenericService Service;

        public OutletConectorValidator(IGenericService service)
        {
            Service = service;


            RuleFor(x => x.Tos.Count).NotEqual(0).WithMessage("To equipment must be defined!");



            RuleFor(x => x).MustAsync(ReviewIfOutletExist)
                .When(x => x.From != null & x.To != null)
                .WithMessage(x => $"Connection -> To: {x.To!.Name} already exist");



        }

        async Task<bool> ReviewIfOutletExist(ConectorDTO request, ConectorDTO dto, CancellationToken cancellationToken)
        {

            ValidateOutletConectorNameRequest validate = new()
            {
                FromId = request.FromId,
                ToId = request.ToId,


                Id = request.Id

            };
            var result = await Service.Validate(validate);
            return !result;
        }


    }
}
