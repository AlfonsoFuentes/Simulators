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
    public SKUDTO Model { get; set; } = new();
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
            await GetAllMaterials();
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
        var backbonesByCategory = ProductBackBones.Where(x => x.ProductCategory == Model.ProductCategory);
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
    List<ProductBackBoneDto> ProductBackBones { get; set; } = new();
    async Task GetAllMaterials()
    {
        var result = await ClientService.GetAll(new ProductBackBoneDto()
        {
            FocusFactory=Model.FocusFactory,
        });
        if (result.Succeeded)
        {
            ProductBackBones = result.Data;
        }
    }
}
