using Simulator.Shared.Models.HCs.SKUs;

namespace Simulator.Client.HCPages.SKUs;
public partial class SKUTable
{
    public List<SKUDTO> Items { get; set; } = new();
    string nameFilter = string.Empty;
    public Func<SKUDTO, bool> Criteria => x => 
    x.Name.Contains(nameFilter, StringComparison.InvariantCultureIgnoreCase)||
    x.SkuCode.Contains(nameFilter, StringComparison.InvariantCultureIgnoreCase);
    public List<SKUDTO> FilteredItems => string.IsNullOrEmpty(nameFilter) ? Items :
        Items.Where(Criteria).ToList();
    protected override async Task OnInitializedAsync()
    {
        await GetAll();
    }
    async Task GetAll()
    {
        var result = await ClientService.GetAll(new SKUDTO());
        if (result.Succeeded)
        {
            Items = result.Data;
        }
    }
    public async Task AddNew()
    {

        var parameters = new DialogParameters<SKUDialog>
        {

        };

        var options = new DialogOptions() { MaxWidth = MaxWidth.Medium };

        var dialog = await DialogService.ShowAsync<SKUDialog>("SKU", parameters, options);
        var result = await dialog.Result;
        if (result != null && !result.Canceled)
        {
            await GetAll();
            StateHasChanged();
        }
    }
    async Task Edit(SKUDTO response)
    {


        var parameters = new DialogParameters<SKUDialog>
        {

             { x => x.Model, response },
        };
        var options = new DialogOptions() { MaxWidth = MaxWidth.Medium };


        var dialog = await DialogService.ShowAsync<SKUDialog>("SKU", parameters, options);
        var result = await dialog.Result;
        if (result != null && !result.Canceled)
        {
            await GetAll();
        }
    }
    public async Task Delete(SKUDTO response)
    {
        var parameters = new DialogParameters<DialogTemplate>
        {
            { x => x.ContentText, $"Do you really want to delete {response.Name}? This process cannot be undone." },
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
    HashSet<SKUDTO> SelecteItems = null!;
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
