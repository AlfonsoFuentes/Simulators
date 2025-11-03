using Simulator.Shared.Models.HCs.Materials;

namespace Simulator.Client.HCPages.Materials;
public partial class MaterialTable
{
    public List<MaterialDTO> Items { get; set; } = new();
    string nameFilter = string.Empty;
    public Func<MaterialDTO, bool> Criteria => x =>
    x.SAPName.Contains(nameFilter, StringComparison.InvariantCultureIgnoreCase)||
    x.M_Number.Contains(nameFilter, StringComparison.InvariantCultureIgnoreCase) ||
    x.CommonName.Contains(nameFilter, StringComparison.InvariantCultureIgnoreCase)||
    x.ProductCategory.ToString().Contains(nameFilter, StringComparison.InvariantCultureIgnoreCase)||
    x.PhysicalStateString.Contains(nameFilter, StringComparison.InvariantCultureIgnoreCase) ||
    x.MaterialType.ToString().Contains(nameFilter, StringComparison.InvariantCultureIgnoreCase) ;
    public List<MaterialDTO> FilteredItems => string.IsNullOrEmpty(nameFilter) ? Items :
        Items.Where(Criteria).ToList();
    protected override async Task OnInitializedAsync()
    {
        await GetAll();
    }
    async Task GetAll()
    {
        var result = await ClientService.GetAll(new MaterialDTO());
        if (result.Succeeded)
        {
            Items = result.Data;
        }
    }
    public async Task AddNew()
    {

        var parameters = new DialogParameters<MaterialDialog>
        {

        };

        var options = new DialogOptions() { MaxWidth = MaxWidth.Medium };

        var dialog = await DialogService.ShowAsync<MaterialDialog>("Material Dialog", parameters, options);
        var result = await dialog.Result;
        if (result != null && !result.Canceled)
        {
            await GetAll();
            StateHasChanged();
        }
    }
    async Task Edit(MaterialDTO response)
    {


        var parameters = new DialogParameters<MaterialDialog>
        {

             { x => x.Model, response },
        };
        var options = new DialogOptions() { MaxWidth = MaxWidth.Medium };


        var dialog = await DialogService.ShowAsync<MaterialDialog>("Material Dialog", parameters, options);
        var result = await dialog.Result;
        if (result != null && !result.Canceled)
        {
            await GetAll();
        }
    }
    public async Task Delete(MaterialDTO response)
    {
        var parameters = new DialogParameters<DialogTemplate>
        {
            { x => x.ContentText, $"Do you really want to delete {response.CommonName}? This process cannot be undone." },
            { x => x.ButtonText, "Delete" },
            { x => x.Color, Color.Error }
        };

        var options = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall };

        var dialog = await DialogService.ShowAsync<DialogTemplate>("Delete", parameters, options);
        var result = await dialog.Result;


        if (!result!.Canceled)
        {
         
            var resultDelete = await ClientService.Delete(response);
            if (resultDelete.Succeeded)
            {
                await GetAll();
              


            }
   
        }

    }
    HashSet<MaterialDTO> SelecteItems = null!;
    public async Task DeleteGroup()
    {
        if (SelecteItems == null) return;
        var parameters = new DialogParameters<DialogTemplate>
        {
            { x => x.ContentText, $"Do you really want to delete this {SelecteItems.Count} Items? This process cannot be undone." },
            { x => x.ButtonText, "Delete" },
            { x => x.Color, Color.Error }
        };

        var options = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall };

        var dialog = await DialogService.ShowAsync<DialogTemplate>("Delete", parameters, options);
        var result = await dialog.Result;


        if (!result!.Canceled)
        {
           
            var resultDelete = await ClientService.DeleteGroup(SelecteItems.ToList());
            if (resultDelete.Succeeded)
            {
                await GetAll();

                SelecteItems = null!;

            }
       
        }

    }
}
