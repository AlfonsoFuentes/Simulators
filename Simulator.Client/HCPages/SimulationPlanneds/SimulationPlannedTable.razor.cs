using Simulator.Shared.Models.HCs.SimulationPlanneds;
using Simulator.Shared.Simulations;

namespace Simulator.Client.HCPages.SimulationPlanneds;
public partial class SimulationPlannedTable
{
    public List<SimulationPlannedDTO> Items { get; set; } = new();
    string nameFilter = string.Empty;
    public Func<SimulationPlannedDTO, bool> Criteria => x => x.Name.Contains(nameFilter, StringComparison.InvariantCultureIgnoreCase);
    public List<SimulationPlannedDTO> FilteredItems => string.IsNullOrEmpty(nameFilter) ? Items :
        Items.Where(Criteria).ToList();
    [Parameter]
    public Guid MainProcessId { get; set; }
    [Parameter]
    public EventCallback RefreshProcessFlowDiagram { get; set; }
    [Parameter]
    public NewSimulation Simulation { get; set; } = new();
    [Parameter]
    public EventCallback<NewSimulation> SimulationChanged { get; set; }
    protected override async Task OnParametersSetAsync()
    {
        await GetAll();
    }

    async Task GetAll()
    {
        var result = await GenericService.GetAll<SimulationPlannedResponseList, SimulationPlannedGetAll>(new SimulationPlannedGetAll()
        {
            MainProcessId = MainProcessId,


        });
        if (result.Succeeded)
        {
            Items = result.Data.Items;
        }
    }
    public async Task AddNew()
    {
        SimulationPlannedDTO response = new() { MainProcessId = MainProcessId };

        var parameters = new DialogParameters<SimulationPlannedDialog>
        {
           { x => x.Model, response },
        };

        var options = new DialogOptions() { MaxWidth = MaxWidth.Large };

        var dialog = await DialogService.ShowAsync<SimulationPlannedDialog>("SimulationPlanned", parameters, options);
        var result = await dialog.Result;
        if (result != null && !result.Canceled)
        {
            await GetAll();
            StateHasChanged();
        }
        await RefreshProcessFlowDiagram.InvokeAsync();
    }
    async Task Edit(SimulationPlannedDTO response)
    {


        var parameters = new DialogParameters<SimulationPlannedDialog>
        {

             { x => x.Model, response },
        };
        var options = new DialogOptions() { MaxWidth = MaxWidth.Large };


        var dialog = await DialogService.ShowAsync<SimulationPlannedDialog>("SimulationPlanned", parameters, options);
        var result = await dialog.Result;
        if (result != null && !result.Canceled)
        {
            await GetAll();
        }
        await RefreshProcessFlowDiagram.InvokeAsync();
    }
    async Task LoadPlann(SimulationPlannedDTO response)
    {
        var result = await GenericService.GetById<SimulationPlannedDTO, GetPlannedByIdRequest>(new()
        {
            Id = response.Id
        });
        if (result.Succeeded)
        {
            
            response.PlannedLines = result.Data.PlannedLines;
            response.PlannedMixers = result.Data.PlannedMixers;
            Simulation.SetPlanned(response);
            await SimulationChanged.InvokeAsync(Simulation);
        }
        
    }
    public async Task Delete(SimulationPlannedDTO response)
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
            DeleteSimulationPlannedRequest request = new()
            {
                Id = response.Id,
                Name = response.Name,

            };
            var resultDelete = await GenericService.Post(request);
            if (resultDelete.Succeeded)
            {
                await GetAll();
                _snackBar.ShowSuccess(resultDelete.Messages);


            }
            else
            {
                _snackBar.ShowError(resultDelete.Messages);
            }
        }
        await RefreshProcessFlowDiagram.InvokeAsync();
    }
    HashSet<SimulationPlannedDTO> SelecteItems = null!;
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
            DeleteGroupSimulationPlannedRequest request = new()
            {
                SelecteItems = SelecteItems,
                MainProcessId = MainProcessId,

            };
            var resultDelete = await GenericService.Post(request);
            if (resultDelete.Succeeded)
            {
                await GetAll();
                _snackBar.ShowSuccess(resultDelete.Messages);
                SelecteItems = null!;

            }
            else
            {
                _snackBar.ShowError(resultDelete.Messages);
            }
        }
        await RefreshProcessFlowDiagram.InvokeAsync();

    }


}
