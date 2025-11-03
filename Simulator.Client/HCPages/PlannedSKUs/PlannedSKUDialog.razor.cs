using Simulator.Shared.Models.HCs.Lines;
using Simulator.Shared.Models.HCs.PlannedSKUs;
using Simulator.Shared.Models.HCs.SKULines;
using Simulator.Shared.Models.HCs.SKUs;
using System.Linq;
using static MudBlazor.CategoryTypes;

namespace Simulator.Client.HCPages.PlannedSKUs;
public partial class PlannedSKUDialog
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
        await GetAllSKULines();
        await getById();



    }


    FluentValidationValidator _fluentValidationValidator = null!;

    private async Task Submit()
    {
        if (Model.LinePlannedId == Guid.Empty)
        {
            MudDialog.Close(DialogResult.Ok(true));
            return;
        }
        var result = await ClientService.Save(Model);


        if (result.Succeeded)
        {
        
            MudDialog.Close(DialogResult.Ok(true));
        }
      

    }


    private void Cancel() => MudDialog.Cancel();

    [Parameter]
    public PlannedSKUDTO Model { get; set; } = new();
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
    List<SKULineDTO> SKULineResponseList = new();
    List<SKUDTO?> SKUs => SKULineResponseList.Count == 0 ? new() : SKULineResponseList.Select(x => x.SKU).ToList();
    async Task GetAllSKULines()
    {
        var result = await ClientService.GetAll(new SKULineDTO()
        {
            LineId = Model.LineId,
        });
        if (result.Succeeded)
        {
            SKULineResponseList = result.Data;


        }
    }
    private Task<IEnumerable<SKUDTO?>> SearchSKULine(string value, CancellationToken token)
    {
        Func<SKUDTO?, bool> Criteria = x =>
        x!.BackBoneCommonName.Contains(value, StringComparison.InvariantCultureIgnoreCase) ||
        x!.SKUCodeName.Contains(value, StringComparison.InvariantCultureIgnoreCase) ||
        x!.BackBoneM_Number.Contains(value, StringComparison.InvariantCultureIgnoreCase) ||
        x!.PackageType.ToString().Contains(value, StringComparison.InvariantCultureIgnoreCase) ||
        x!.ProductCategory.ToString().Contains(value, StringComparison.InvariantCultureIgnoreCase) ||
        x!.Size.ToString().Contains(value, StringComparison.InvariantCultureIgnoreCase)
        ;
        IEnumerable<SKUDTO?> FilteredItems = string.IsNullOrEmpty(value) ? SKUs.AsEnumerable() :
             SKUs.Where(Criteria);
        return Task.FromResult(FilteredItems);
    }
    void ChangeSku()
    {
        var speedfond = SKULineResponseList.FirstOrDefault(x => x.LineId == Model.LineId && x.SKUId == Model.SKUId);
        if (speedfond != null)
        {
            Model.LineSpeed = speedfond.LineSpeed;
            Model.Case_Shift = speedfond.Case_Shift;
            Model.ChangeLineSpeed();
        }

    }

}
