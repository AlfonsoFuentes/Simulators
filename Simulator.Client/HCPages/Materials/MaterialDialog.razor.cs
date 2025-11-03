using Simulator.Shared.Models.HCs.Materials;

namespace Simulator.Client.HCPages.Materials;
public partial class MaterialDialog
{
    [CascadingParameter]
    private IMudDialogInstance MudDialog { get; set; } = null!;
    private bool Validated { get; set; } = false;
    async Task ValidateAsync()
    {
        Validated = _fluentValidationValidator == null ? false : await _fluentValidationValidator.ValidateAsync(options => { options.IncludeAllRuleSets(); });
    }
    bool loaded = false;
    protected override async Task OnInitializedAsync()
    {
       await getById();
        loaded = true;

    }
    FluentValidationValidator _fluentValidationValidator = null!;

    private async Task Submit()
    {
        var result = await ClientService.Save(Model);


        if (result.Succeeded)
        {
         
            MudDialog.Close(DialogResult.Ok(true));
        }


    }


    private void Cancel() => MudDialog.Cancel();

    [Parameter]
    public MaterialDTO Model { get; set; } = new();
    async Task getById()
    {
        if (Model.Id == Guid.Empty)
        {
            return;
        }
        var result = await ClientService.GetById(Model); ;
        if (result.Succeeded)
        {
            Model = result.Data;
        }
    }
}
