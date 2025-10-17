using Simulator.Shared.Models.CompoundProperties;

namespace Simulator.Client.CompoundPropertiesPages
{
    public partial class CompundPropertyDialog
    {
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
            var result = await GenericService.Post(Model);


            if (result.Succeeded)
            {
                _snackBar.ShowSuccess(result.Messages);
                MudDialog.Close(DialogResult.Ok(true));
            }
            else
            {
                _snackBar.ShowError(result.Messages);
            }

        }


        private void Cancel() => MudDialog.Cancel();

        [Parameter]
        public CompoundPropertyDTO Model { get; set; } = null!;
        async Task getById()
        {
            if (Model.Id == Guid.Empty)
            {
                return;
            }
            var result = await GenericService.GetById<CompoundPropertyDTO, GetCompoundPropertyByIdRequest>(new()
            {
                Id = Model.Id
            });
            if (result.Succeeded)
            {
                Model = result.Data;
                StateHasChanged();
            }
        }
       
    }
}
