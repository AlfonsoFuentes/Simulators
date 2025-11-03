using Simulator.Shared.Models.HCs.BackBoneSteps;
using Simulator.Shared.Models.HCs.Materials;

namespace Simulator.Client.HCPages.BackBoneSteps;
public partial class BackBoneStepTable
{
    [Parameter]
    public MaterialDTO Material { get; set; } = null!;
    public List<BackBoneStepDTO> Items => Material == null ? new() : Material.BackBoneSteps;
    string nameFilter = string.Empty;
    public Func<BackBoneStepDTO, bool> Criteria => x => x.StepName.Contains(nameFilter, StringComparison.InvariantCultureIgnoreCase);
    public List<BackBoneStepDTO> FilteredItems => string.IsNullOrEmpty(nameFilter) ? Items :
        Items.Where(Criteria).ToList();
    public List<BackBoneStepDTO> OrderedItems => Items.OrderBy(x => x.Order).ToList();
    int LastOrder => OrderedItems.Count > 0 ? OrderedItems.Max(x => x.Order) + 1 : 1;

    public Guid MaterialId => Material == null ? Guid.Empty : Material.Id;

    [Parameter]
    public EventCallback<MaterialDTO> MaterialChanged { get; set; }

    [Parameter]
    public EventCallback ValidateAsync { get; set; }

    public string MaterialName => Material == null ? string.Empty : Material.CommonName;
    string Legend => $"Manufacture Order for {MaterialName}";
    protected override async Task OnParametersSetAsync()
    {
        await GetAll();

    }
    
    async Task GetAll()
    {
        if (MaterialId != Guid.Empty)
        {

            var result = await ClientService.GetAll(new BackBoneStepDTO()
            {

                MaterialId = MaterialId,
            });
            if (result.Succeeded)
            {
                Material.BackBoneSteps = result.Data;
                
            }
        }
        


    }
    public async Task AddNew()
    {
        BackBoneStepDTO response = new BackBoneStepDTO
        {
            MaterialId = MaterialId,
            Order = LastOrder,
        };

        var parameters = new DialogParameters<BackBoneStepDialog>
        {
             { x => x.Model, response },
             { y => y.FocusFactory, Material.FocusFactory },
        };

        var options = new DialogOptions() { MaxWidth = MaxWidth.Small };

        var dialog = await DialogService.ShowAsync<BackBoneStepDialog>("BackBoneStep", parameters, options);
        var result = await dialog.Result;
        if (result != null && !result.Canceled)
        {
            if (MaterialId == Guid.Empty)
            {
                Items.Add(response);

            }
            else
            {
                await GetAll();
            }
               
            await MaterialChanged.InvokeAsync(Material);
            await ValidateAsync.InvokeAsync();
            StateHasChanged();
        }
    }
    async Task Edit(BackBoneStepDTO response)
    {


        var parameters = new DialogParameters<BackBoneStepDialog>
        {

             { x => x.Model, response },
             { y => y.FocusFactory, Material.FocusFactory },
        };
        var options = new DialogOptions() { MaxWidth = MaxWidth.Small };


        var dialog = await DialogService.ShowAsync<BackBoneStepDialog>("BackBoneStep", parameters, options);
        var result = await dialog.Result;
        if (result != null && !result.Canceled)
        {
            if (MaterialId != Guid.Empty)
            {
                await GetAll();
            }
         
            await MaterialChanged.InvokeAsync(Material);
            await ValidateAsync.InvokeAsync();
        }
    }
    public async Task Delete(BackBoneStepDTO response)
    {
        var parameters = new DialogParameters<DialogTemplate>
        {
            { x => x.ContentText, $"Do you really want to delete {response.StepName}? This process cannot be undone." },
            { x => x.ButtonText, "Delete" },
            { x => x.Color, Color.Error }
        };

        var options = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall };

        var dialog = await DialogService.ShowAsync<DialogTemplate>("Delete", parameters, options);
        var result = await dialog.Result;


        if (!result!.Canceled)
        {
           
            if (MaterialId != Guid.Empty)
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
           
            await MaterialChanged.InvokeAsync(Material);
            await ValidateAsync.InvokeAsync();
        }

    }
    HashSet<BackBoneStepDTO> SelecteItems = null!;
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
           
            if (MaterialId != Guid.Empty)
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


          
            await MaterialChanged.InvokeAsync(Material);
            await ValidateAsync.InvokeAsync();
        }

    }
    BackBoneStepDTO SelectedRow = null!;

    bool DisableUpButton => SelectedRow == null ? true : SelectedRow.Order == 1;
    bool DisableDownButton => SelectedRow == null ? true : SelectedRow.Order == LastOrder;

    void RowClicked(BackBoneStepDTO item)
    {
        SelectedRow = SelectedRow == null ? SelectedRow = item : SelectedRow = null!;
    }
    async Task Up()
    {
        if (SelectedRow == null) return;

        if (MaterialId != Guid.Empty)
        {
            var result = await ClientService.OrderUp(SelectedRow);
            if (result.Succeeded)
            {
                await GetAll();

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
      
        await MaterialChanged.InvokeAsync(Material);
        await ValidateAsync.InvokeAsync();
    }
    async Task Down()
    {
        if (SelectedRow == null) return;
        if (MaterialId != Guid.Empty)
        {
            var result = await ClientService.OrderDown(SelectedRow);

            if (result.Succeeded)
            {

                await GetAll();

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
       
        await MaterialChanged.InvokeAsync(Material);
        await ValidateAsync.InvokeAsync();
    }
}
