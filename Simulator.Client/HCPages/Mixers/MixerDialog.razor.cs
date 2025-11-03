using Simulator.Shared.Models.HCs.Mixers;

namespace Simulator.Client.HCPages.Mixers;
public partial class MixerDialog
{
    [CascadingParameter]
    private IMudDialogInstance MudDialog { get; set; } = null!;
    private bool Validated { get; set; } = false;
    async Task ValidateAsync()
    {
        Validated = _fluentValidationValidator == null ? false : await _fluentValidationValidator.ValidateAsync(options => { options.IncludeAllRuleSets(); });
    }

    protected override async Task OnInitializedAsync()
    {
      
        await getById();
     
       

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
    public MixerDTO Model { get; set; } = new();
    async Task getById()
    {
        if (Model.Id == Guid.Empty)
        {
            return;
        }
        var result = await ClientService.GetById(Model);
        if (result.Succeeded)
        {
            Model = result.Data;
        }
    }
   
}
