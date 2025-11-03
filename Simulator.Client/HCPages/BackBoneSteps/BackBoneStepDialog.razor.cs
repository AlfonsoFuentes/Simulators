using Simulator.Client.HCPages.Materials;
using Simulator.Shared.Enums.HCEnums.Enums;
using Simulator.Shared.Models.HCs.BackBoneSteps;
using Simulator.Shared.Models.HCs.Materials;
using static MudBlazor.CategoryTypes;

namespace Simulator.Client.HCPages.BackBoneSteps;
public partial class BackBoneStepDialog
{
    [CascadingParameter]
    private IMudDialogInstance MudDialog { get; set; } = null!;
    private bool Validated { get; set; } = false;
    async Task ValidateAsync()
    {
        Validated = _fluentValidationValidator == null ? false : await _fluentValidationValidator.ValidateAsync(options => { options.IncludeAllRuleSets(); });
    }


    FluentValidationValidator _fluentValidationValidator = null!;
    List<RawMaterialDto> RawMaterials = new();
    protected override async Task OnInitializedAsync()
    {
        await GetAllMaterials();
    }
    async Task GetAllMaterials()
    {
        var result = await ClientService.GetAll(new RawMaterialDto()
        {
            FocusFactory = FocusFactory,
        });
        if (result.Succeeded)
        {


            RawMaterials = result.Data;
        }
    }
    [Parameter]
    public FocusFactory FocusFactory { get; set; } = FocusFactory.None;
    private async Task Submit()
    {
        if (Model.MaterialId == Guid.Empty)
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
    public BackBoneStepDTO Model { get; set; } = new();

    private Task<IEnumerable<RawMaterialDto>> SearchRawMaterial(string value, CancellationToken token)
    {
        Func<RawMaterialDto, bool> Criteria = x =>
        x.SAPName.Contains(value, StringComparison.InvariantCultureIgnoreCase) ||
         x.M_Number.Contains(value, StringComparison.InvariantCultureIgnoreCase) ||
         x.CommonName.Contains(value, StringComparison.InvariantCultureIgnoreCase)
        ;
        IEnumerable<RawMaterialDto> FilteredItems = string.IsNullOrEmpty(value) ? RawMaterials.AsEnumerable() :
             RawMaterials.Where(Criteria);
        return Task.FromResult(FilteredItems);
    }
    public async Task AddRawMaterial()
    {


        var parameters = new DialogParameters<MaterialDialog>
        {

        };

        var options = new DialogOptions() { MaxWidth = MaxWidth.Medium };

        var dialog = await DialogService.ShowAsync<MaterialDialog>("Raw Material", parameters, options);
        var result = await dialog.Result;
        if (result != null)
        {
            await GetAllMaterials();

        }
    }

    void ChangetoWashout()
    {
        if (Model.BackBoneStepType == Shared.Enums.HCEnums.Enums.BackBoneStepType.Washout)
        {
            var rawmaterial = RawMaterials.FirstOrDefault(x => x.IsForWashing);
            if (rawmaterial != null)
            {
                Model.StepRawMaterial = rawmaterial;
            }


        }
        else
        {
            Model.StepRawMaterial = null!;
        }
    }
}
