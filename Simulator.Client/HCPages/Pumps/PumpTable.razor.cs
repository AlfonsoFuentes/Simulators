using Simulator.Shared.Enums.HCEnums.Enums;
using Simulator.Shared.Models.HCs.Pumps;

namespace Simulator.Client.HCPages.Pumps;
public partial class PumpTable
{
    public List<PumpDTO> Items { get; set; } = new();
    string nameFilter = string.Empty;
    public Func<PumpDTO, bool> Criteria => x => x.Name.Contains(nameFilter, StringComparison.InvariantCultureIgnoreCase);
    public List<PumpDTO> FilteredItems => string.IsNullOrEmpty(nameFilter) ? Items :
        Items.Where(Criteria).ToList();
    [Parameter]
    public Guid MainProcessId { get; set; }
    [Parameter]
    [EditorRequired]
    public FocusFactory FocusFactory { get; set; } = FocusFactory.None;
    protected override async Task OnParametersSetAsync()
    {
        await GetAll();
    }

    async Task GetAll()
    {
        var result = await ClientService.GetAll(new PumpDTO()
        {
            MainProcessId = MainProcessId,


        });
        if (result.Succeeded)
        {
            Items = result.Data;
        }
    }
    public async Task AddNew()
    {
        PumpDTO response = new()
        {
            MainProcessId = MainProcessId,
            FocusFactory = FocusFactory
        };

        var parameters = new DialogParameters<PumpDialog>
        {
           { x => x.Model, response },
        };

        var options = new DialogOptions() { MaxWidth = MaxWidth.Medium };

        var dialog = await DialogService.ShowAsync<PumpDialog>("Pump", parameters, options);
        var result = await dialog.Result;
        if (result != null && !result.Canceled)
        {
            await GetAll();
            StateHasChanged();
        }
        await RefreshProcessFlowDiagram.InvokeAsync();
    }
    async Task Edit(PumpDTO response)
    {


        var parameters = new DialogParameters<PumpDialog>
        {

             { x => x.Model, response },
        };
        var options = new DialogOptions() { MaxWidth = MaxWidth.Medium };


        var dialog = await DialogService.ShowAsync<PumpDialog>("Pump", parameters, options);
        var result = await dialog.Result;
        if (result != null && !result.Canceled)
        {
            await GetAll();
        }
        await RefreshProcessFlowDiagram.InvokeAsync();
    }
    public async Task Delete(PumpDTO response)
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
        await RefreshProcessFlowDiagram.InvokeAsync();

    }
    HashSet<PumpDTO> SelecteItems = null!;
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
        await RefreshProcessFlowDiagram.InvokeAsync();

    }
}
