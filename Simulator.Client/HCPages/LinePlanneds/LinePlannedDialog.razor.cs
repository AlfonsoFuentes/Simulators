using Simulator.Shared.Models.HCs.LinePlanneds;
using Simulator.Shared.Models.HCs.Lines;
using Simulator.Shared.Models.HCs.SKULines;

namespace Simulator.Client.HCPages.LinePlanneds;
public partial class LinePlannedDialog
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
        await GetAllLines();
        await getById();



    }
    LineResponseList LineResponseList = new();
    async Task GetAllLines()
    {
        var result = await GenericService.GetAll<LineResponseList, LineGetAll>(new LineGetAll()
        {
            MainProcessId = Model.MainProcesId,


        });
        if (result.Succeeded)
        {
            LineResponseList = result.Data;
        }
    }
    
    FluentValidationValidator _fluentValidationValidator = null!;

    private async Task Submit()
    {
        if (Model.SimulationPlannedId == Guid.Empty)
        {
            MudDialog.Close(DialogResult.Ok(true));
            return;
        }
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
    public LinePlannedDTO Model { get; set; } = new();
    async Task getById()
    {
        if (Model.Id == Guid.Empty)
        {
            return;
        }
        var result = await GenericService.GetById<LinePlannedDTO, GetLinePlannedByIdRequest>(new()
        {
            Id = Model.Id
        });
        if (result.Succeeded)
        {
            Model = result.Data;
            
        }
    }

    private Task<IEnumerable<LineDTO>> SearchLine(string value, CancellationToken token)
    {
        Func<LineDTO, bool> Criteria = x =>
        x.Name.Contains(value, StringComparison.InvariantCultureIgnoreCase)
        ;
        IEnumerable<LineDTO> FilteredItems = string.IsNullOrEmpty(value) ? LineResponseList.Items.AsEnumerable() :
             LineResponseList.Items.Where(Criteria);
        return Task.FromResult(FilteredItems);
    }
  

}
