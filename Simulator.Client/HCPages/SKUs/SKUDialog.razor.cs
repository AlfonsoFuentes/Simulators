using Simulator.Client.HCPages.Materials;
using Simulator.Shared.Models.HCs.Materials;
using Simulator.Shared.Models.HCs.SKUs;

namespace Simulator.Client.HCPages.SKUs;
public partial class SKUDialog
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
        await GetAllMaterials();
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
    public SKUDTO Model { get; set; } = new();
    async Task getById()
    {
        if (Model.Id == Guid.Empty)
        {
            return;
        }
        var result = await GenericService.GetById<SKUDTO, GetSKUByIdRequest>(new()
        {
            Id = Model.Id
        });
        if (result.Succeeded)
        {
            Model = result.Data;
            StateHasChanged();
        }
    }
    private Task<IEnumerable<MaterialDTO>> SearchBackBones(string value, CancellationToken token)
    {
        Func<MaterialDTO, bool> Criteria = x =>
        x.SAPName.Contains(value, StringComparison.InvariantCultureIgnoreCase) ||
         x.M_Number.Contains(value, StringComparison.InvariantCultureIgnoreCase) ||
         x.CommonName.Contains(value, StringComparison.InvariantCultureIgnoreCase)
        ;
        var backbonesByCategory = ProductBackBones.Items.Where(x => x.ProductCategory == Model.ProductCategory);
        IEnumerable<MaterialDTO> FilteredItems = string.IsNullOrEmpty(value) ? backbonesByCategory.AsEnumerable() :
             backbonesByCategory.Where(Criteria);
        return Task.FromResult(FilteredItems);
    }
    public async Task AddRawMaterial()
    {
        MaterialDTO Model = new MaterialDTO()
        {
            MaterialType = Shared.Enums.HCEnums.Enums.MaterialType.ProductBackBone,

        };

        var parameters = new DialogParameters<MaterialDialog>
        {
             { x => x.Model, Model },
        };

        var options = new DialogOptions() { MaxWidth = MaxWidth.Medium };

        var dialog = await DialogService.ShowAsync<MaterialDialog>("Product", parameters, options);
        var result = await dialog.Result;
        if (result != null)
        {
            await GetAllMaterials();

        }
    }
    MaterialResponseList ProductBackBones { get; set; } = new();
    async Task GetAllMaterials()
    {
        var result = await GenericService.GetAll<MaterialResponseList, ProductBackBoneGetAll>(new ProductBackBoneGetAll());
        if (result.Succeeded)
        {
            ProductBackBones = result.Data;
        }
    }
}
