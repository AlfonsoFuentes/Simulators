using Simulator.Shared.Models.HCs.Washouts;

namespace Simulator.Client.HCPages.Washouts;
public partial class WashoutTable
{
    public List<WashoutDTO> Items { get; set; } = new();
    string nameFilter = string.Empty;
    public Func<WashoutDTO, bool> Criteria => x => x.ProductCategoryCurrent.ToString().Contains(nameFilter, StringComparison.InvariantCultureIgnoreCase);
    public List<WashoutDTO> FilteredItems => string.IsNullOrEmpty(nameFilter) ? Items :
        Items.Where(Criteria).ToList();
    protected override async Task OnInitializedAsync()
    {
        await GetAll();
    }
    async Task GetAll()
    {
        var result = await ClientService.GetAll(new WashoutDTO());
        if (result.Succeeded)
        {
            Items = result.Data;
        }
    }
    public async Task AddNew()
    {

        var parameters = new DialogParameters<WashoutDialog>
        {

        };

        var options = new DialogOptions() { MaxWidth = MaxWidth.Small };

        var dialog = await DialogService.ShowAsync<WashoutDialog>("Washout Dialog", parameters, options);
        var result = await dialog.Result;
        if (result != null && !result.Canceled)
        {
            await GetAll();
            StateHasChanged();
        }
    }
    async Task Edit(WashoutDTO response)
    {


        var parameters = new DialogParameters<WashoutDialog>
        {

             { x => x.Model, response },
        };
        var options = new DialogOptions() { MaxWidth = MaxWidth.Small };


        var dialog = await DialogService.ShowAsync<WashoutDialog>("Washout Dialog", parameters, options);
        var result = await dialog.Result;
        if (result != null && !result.Canceled)
        {
            await GetAll();
        }
    }
    public async Task Delete(WashoutDTO response)
    {
        var parameters = new DialogParameters<DialogTemplate>
        {
            { x => x.ContentText, $"Do you really want to delete {response.ProductCategoryCurrent}? This process cannot be undone." },
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
    HashSet<WashoutDTO> SelecteItems = null!;
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
