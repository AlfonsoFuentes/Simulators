using Simulator.Shared.Models.HCs.BackBoneSteps;
using Simulator.Shared.Models.HCs.PlannedSKUs;

namespace Simulator.Client.HCPages.PlannedSKUs;
public partial class PlannedSKUTable
{
    [Parameter]
    public List<PlannedSKUDTO> Items { get; set; } = new();
    [Parameter]
    public EventCallback<List<PlannedSKUDTO>> ItemsChanged { get; set; }
    string nameFilter = string.Empty;
    public Func<PlannedSKUDTO, bool> Criteria => x =>
    x.SkuName.Contains(nameFilter, StringComparison.InvariantCultureIgnoreCase) ||
    x.SkuCode.Contains(nameFilter, StringComparison.InvariantCultureIgnoreCase);
    public List<PlannedSKUDTO> FilteredItems => string.IsNullOrEmpty(nameFilter) ? Items :
        Items.Where(Criteria).ToList();
    public List<PlannedSKUDTO> OrderedItems => Items.OrderBy(x=>x.Order).ToList();
    int LastOrder => OrderedItems.Count > 0 ? OrderedItems.Max(x => x.Order) + 1 : 1;
    [Parameter]
    [EditorRequired]
    public Guid LinePlannedId { get; set; }
    [Parameter]
    [EditorRequired]
    public Guid LineId { get; set; }
    [Parameter]
    public EventCallback ValidateAsync { get; set; }
    
   
    async Task GetAll()
    {
        if (LinePlannedId != Guid.Empty)
        {
            var result = await ClientService.GetAll(new PlannedSKUDTO()
            {
                LinePlannedId = LinePlannedId,


            });
            if (result.Succeeded)
            {
                Items = result.Data;
                await ItemsChanged.InvokeAsync(Items);
            }
          
        }
      
    }
    public async Task AddNew()
    {
        PlannedSKUDTO response = new()
        {
            LinePlannedId = LinePlannedId,
            LineId = LineId,
            Order = LastOrder,
        };

        var parameters = new DialogParameters<PlannedSKUDialog>
        {
           { x => x.Model, response },
        };

        var options = new DialogOptions() { MaxWidth = MaxWidth.Small };

        var dialog = await DialogService.ShowAsync<PlannedSKUDialog>("PlannedSKU", parameters, options);
        var result = await dialog.Result;
        if (result != null && !result.Canceled)
        {
            if (LinePlannedId == Guid.Empty)
            {
                Items.Add(response);
            }
            await GetAll();
            await ValidateAsync.InvokeAsync();
            StateHasChanged();
        }
    }
    async Task Edit(PlannedSKUDTO response)
    {


        var parameters = new DialogParameters<PlannedSKUDialog>
        {

             { x => x.Model, response },
        };
        var options = new DialogOptions() { MaxWidth = MaxWidth.Small };


        var dialog = await DialogService.ShowAsync<PlannedSKUDialog>("PlannedSKU", parameters, options);
        var result = await dialog.Result;
        if (result != null && !result.Canceled)
        {
            await GetAll();
            await ValidateAsync.InvokeAsync();
        }
    }
    public async Task Delete(PlannedSKUDTO response)
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
          
            if (LinePlannedId != Guid.Empty)
            {
                var resultDelete = await ClientService.Delete(response);
                if (resultDelete.Succeeded)
                {

                    await GetAll();


                }
                
            }
            else
            {
                Items.Remove(response);
            }
          
            await ValidateAsync.InvokeAsync();
        }

    }
    HashSet<PlannedSKUDTO> SelecteItems = null!;
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
           
            if (LinePlannedId != Guid.Empty)
            {
                var resultDelete = await ClientService.DeleteGroup(SelecteItems.ToList());
                if (resultDelete.Succeeded)
                {

             
                    SelecteItems = null!;

                }
              
            }
            else
            {
                Items.RemoveAll(x => SelecteItems.Contains(x));
            }
            await GetAll();
            await ValidateAsync.InvokeAsync();
        }

    }
    PlannedSKUDTO SelectedRow = null!;

    bool DisableUpButton => SelectedRow == null ? true : SelectedRow.Order == 1;
    bool DisableDownButton => SelectedRow == null ? true : SelectedRow.Order == LastOrder;

    void RowClicked(PlannedSKUDTO item)
    {
        SelectedRow = SelectedRow == null ? SelectedRow = item : SelectedRow = null!;
    }
    async Task Up()
    {
        if (SelectedRow == null) return;

        if (LinePlannedId != Guid.Empty)
        {
            var result = await ClientService.OrderUp(SelectedRow);
            if (result.Succeeded)
            {


            }
        }
        else
        {
            var previuousrow = Items.FirstOrDefault(x => x.Order == SelectedRow.Order - 1);
            if (previuousrow != null)
            {
                previuousrow.Order += 1;
                SelectedRow.Order -= 1;
            }
        }
        await GetAll();
        await ValidateAsync.InvokeAsync();
    }
    async Task Down()
    {
        if (SelectedRow == null) return;
        if (LinePlannedId != Guid.Empty)
        {
            var result = await ClientService.OrderUp(SelectedRow);

            if (result.Succeeded)
            {



            }
        }
        else
        {
            var nextrow = Items.FirstOrDefault(x => x.Order == SelectedRow.Order + 1);
            if (nextrow != null)
            {
                nextrow.Order -= 1;
                SelectedRow.Order += 1;
            }
        }
        await GetAll();
        await ValidateAsync.InvokeAsync();
    }
}
