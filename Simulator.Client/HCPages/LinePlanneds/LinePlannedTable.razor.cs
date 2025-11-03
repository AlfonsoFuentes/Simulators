using Simulator.Shared.Models.HCs.LinePlanneds;

namespace Simulator.Client.HCPages.LinePlanneds;
public partial class LinePlannedTable
{
    [Parameter]
    public List<LinePlannedDTO> Items { get; set; } = new();
    [Parameter]
    public EventCallback<List<LinePlannedDTO>> ItemsChanged { get; set; }
    string nameFilter = string.Empty;
    public Func<LinePlannedDTO, bool> Criteria => x => x.Name.Contains(nameFilter, StringComparison.InvariantCultureIgnoreCase);
    public List<LinePlannedDTO> FilteredItems => string.IsNullOrEmpty(nameFilter) ? Items :
        Items.Where(Criteria).ToList();
    [Parameter]
    [EditorRequired]
    public Guid SimulationPlannedId { get; set; }
    [Parameter]
    [EditorRequired]
    public Guid MainProcesId { get; set; }
    [Parameter]
    public EventCallback ValidateAsync { get; set; }
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if(firstRender)
        {
            await GetAll();
        }
    }

    async Task GetAll()
    {
        if(SimulationPlannedId != Guid.Empty)
        {
            var result = await ClientService.GetAll(new LinePlannedDTO()
            {
                SimulationPlannedId = SimulationPlannedId,


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
        LinePlannedDTO response = new() { SimulationPlannedId = SimulationPlannedId, MainProcesId = MainProcesId };

        var parameters = new DialogParameters<LinePlannedDialog>
        {
           { x => x.Model, response },
        };

        var options = new DialogOptions() { MaxWidth = MaxWidth.Medium };

        var dialog = await DialogService.ShowAsync<LinePlannedDialog>("LinePlanned", parameters, options);
        var result = await dialog.Result;
        if (result != null && !result.Canceled)
        {
            if (SimulationPlannedId == Guid.Empty)
            {
                Items.Add(response);
            }
            await GetAll();
            await ValidateAsync.InvokeAsync();
            StateHasChanged();
        }
    }
    async Task Edit(LinePlannedDTO response)
    {


        var parameters = new DialogParameters<LinePlannedDialog>
        {

             { x => x.Model, response },
        };
        var options = new DialogOptions() { MaxWidth = MaxWidth.Medium };


        var dialog = await DialogService.ShowAsync<LinePlannedDialog>("LinePlanned", parameters, options);
        var result = await dialog.Result;
        if (result != null && !result.Canceled)
        {
            await GetAll();
            await ValidateAsync.InvokeAsync();
        }
    }
    public async Task Delete(LinePlannedDTO response)
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
            
            if (SimulationPlannedId != Guid.Empty)
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
    HashSet<LinePlannedDTO> SelecteItems = null!;
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
           
            if(SimulationPlannedId!=Guid.Empty)
            {
                var resultDelete = await ClientService.DeleteGroup(SelecteItems.ToList());
                if (resultDelete.Succeeded)
                {
                    await GetAll();

                    SelecteItems = null!;

                }
               
            }
            else
            {
                Items.RemoveAll(x => SelecteItems.Contains(x));
            }
          
            await ValidateAsync.InvokeAsync();
        }

    }
}
