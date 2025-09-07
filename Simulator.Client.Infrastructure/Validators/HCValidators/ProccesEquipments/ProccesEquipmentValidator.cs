using Web.Infrastructure.Managers.Generic;

namespace Web.Infrastructure.Validators.FinishinLines.ProccesEquipments
{

    //public class ProccesEquipmentValidator : AbstractValidator<ProccesEquipmentDTO>
    //{
    //    private readonly IGenericService Service;

    //    public ProccesEquipmentValidator(IGenericService service)
    //    {
    //        Service = service;

    //        RuleFor(x => x.Name).NotEmpty().WithMessage("Name must be defined!");
           

    //        RuleFor(x => x.Name).MustAsync(ReviewIfNameExist)
    //            .When(x => !string.IsNullOrEmpty(x.Name))
    //            .WithMessage(x => $"{x.Name} already exist");

          

    //    }

    //    async Task<bool> ReviewIfNameExist(ProccesEquipmentDTO request, string name, CancellationToken cancellationToken)
    //    {
    //        ValidateProccesEquipmentNameRequest validate = new()
    //        {
    //            Name = name,


    //            Id = request.Id

    //        };
    //        var result = await Service.Validate(validate);
    //        return !result;
    //    }
       
    //}
}
