using Simulator.Client.Infrastructure.Managers.ClientCRUDServices;
using Simulator.Shared.Models.CompoundProperties;
using Simulator.Shared.NewModels.Compounds;

namespace Simulator.Client.CompoundPropertiesPages
{
    public partial class CompundPropertyDialog
    {
        [Inject]
        public IClientCRUDService Service { get; set; } = null!;
        [CascadingParameter]
        private IMudDialogInstance MudDialog { get; set; } = null!;
        private bool Validated { get; set; } = true;
        async Task ValidateAsync()
        {
        
           var result= _fluentValidationValidator == null ? false : await _fluentValidationValidator.ValidateAsync(options => { options.IncludeAllRuleSets(); });
        }

        protected override async Task OnInitializedAsync()
        {
           
            await getById();

        }
        FluentValidationValidator _fluentValidationValidator = null!;

        private async Task Submit()
        {
            var result = await Service.Save(Model);


            if (result.Succeeded)
            {

                MudDialog.Close(DialogResult.Ok(true));
            }

        }


        private void Cancel() => MudDialog.Cancel();

        [Parameter]
        public NewCompoundPropertyDTO Model { get; set; } = null!;
        async Task getById()
        {
            if (Model.Id == Guid.Empty)
            {
                return;
            }
            var result = await Service.GetById(Model);
            if (result.Succeeded && result.Data is not null)
            {
                Model = result.Data as NewCompoundPropertyDTO;
            }
        }
       
    }
}
