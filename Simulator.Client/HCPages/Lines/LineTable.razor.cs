using Simulator.Shared.Enums.HCEnums.Enums;
using Simulator.Shared.Models.HCs.Lines;

namespace Simulator.Client.HCPages.Lines;
public partial class LineTable
{
    public List<LineDTO> Items { get; set; } = new();
    string nameFilter = string.Empty;
    public Func<LineDTO, bool> Criteria => x => x.Name.Contains(nameFilter, StringComparison.InvariantCultureIgnoreCase);
    public List<LineDTO> FilteredItems => string.IsNullOrEmpty(nameFilter) ? Items :
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
        var result = await ClientService.GetAll(new LineDTO()
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
        LineDTO response = new()
        {
            MainProcessId = MainProcessId,
            FocusFactory = FocusFactory
        };

        var parameters = new DialogParameters<LineDialog>
        {
           { x => x.Model, response },
        };

        var options = new DialogOptions() { MaxWidth = MaxWidth.Large };

        var dialog = await DialogService.ShowAsync<LineDialog>("Line", parameters, options);
        var result = await dialog.Result;
        if (result != null && !result.Canceled)
        {
            await GetAll();
            StateHasChanged();
        }
        await RefreshProcessFlowDiagram.InvokeAsync();
    }
    async Task Edit(LineDTO response)
    {


        var parameters = new DialogParameters<LineDialog>
        {

             { x => x.Model, response },
        };
        var options = new DialogOptions() { MaxWidth = MaxWidth.Large };


        var dialog = await DialogService.ShowAsync<LineDialog>("Line", parameters, options);
        var result = await dialog.Result;
        if (result != null && !result.Canceled)
        {
            await GetAll();
        }
        await RefreshProcessFlowDiagram.InvokeAsync();
    }
    public async Task Delete(LineDTO response)
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
    HashSet<LineDTO> SelecteItems = null!;
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
