using Simulator.Shared.Models.HCs.BackBoneSteps;
using Simulator.Shared.Models.HCs.MixerPlanneds;

namespace Simulator.Client.HCPages.MixerPlanneds;
public partial class MixerPlannedTable
{
    [Parameter]
    public List<MixerPlannedDTO> Items { get; set; } = new();
    [Parameter]
    public EventCallback<List<MixerPlannedDTO>> ItemsChanged { get; set; }
    string nameFilter = string.Empty;
    public Func<MixerPlannedDTO, bool> Criteria => x => x.MixerDTO!.Name.Contains(nameFilter, StringComparison.InvariantCultureIgnoreCase);
    public List<MixerPlannedDTO> FilteredItems => string.IsNullOrEmpty(nameFilter) ? Items :
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
            var result = await ClientService.GetAll(new MixerPlannedDTO()
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
        MixerPlannedDTO response = new() { SimulationPlannedId = SimulationPlannedId, MainProcesId = MainProcesId };

        var parameters = new DialogParameters<MixerPlannedDialog>
        {
           { x => x.Model, response },
        };

        var options = new DialogOptions() { MaxWidth = MaxWidth.Small };

        var dialog = await DialogService.ShowAsync<MixerPlannedDialog>("MixerPlanned", parameters, options);
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
    async Task Edit(MixerPlannedDTO response)
    {


        var parameters = new DialogParameters<MixerPlannedDialog>
        {

             { x => x.Model, response },
        };
        var options = new DialogOptions() { MaxWidth = MaxWidth.Small };


        var dialog = await DialogService.ShowAsync<MixerPlannedDialog>("MixerPlanned", parameters, options);
        var result = await dialog.Result;
        if (result != null && !result.Canceled)
        {
            await GetAll();
            await ValidateAsync.InvokeAsync();
        }
    }
    public async Task Delete(MixerPlannedDTO response)
    {
        var parameters = new DialogParameters<DialogTemplate>
        {
            { x => x.ContentText, $"Do you really want to delete this row? This process cannot be undone." },
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
    HashSet<MixerPlannedDTO> SelecteItems = null!;
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
