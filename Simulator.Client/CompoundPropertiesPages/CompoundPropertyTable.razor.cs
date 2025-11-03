using Simulator.Client.Infrastructure.Managers.ClientCRUDServices;
using Simulator.Shared.Models.CompoundProperties;
using Simulator.Shared.NewModels.Compounds;

namespace Simulator.Client.CompoundPropertiesPages
{
    public partial class CompoundPropertyTable
    {
        [Inject]
        public IClientCRUDService Service { get; set; } = null!;
        public List<NewCompoundPropertyDTO> Items { get; set; } = new();
        string nameFilter = string.Empty;
        public Func<NewCompoundPropertyDTO, bool> Criteria => x =>
        x.Name.Contains(nameFilter, StringComparison.InvariantCultureIgnoreCase) ;
        public List<NewCompoundPropertyDTO> FilteredItems => string.IsNullOrEmpty(nameFilter) ? Items :
            Items.Where(Criteria).ToList();

  
        protected override async Task OnInitializedAsync()
        {
            await GetAll();
        }
        async Task GetAll()
        {
            var resullt = await Service.GetAll(new NewCompoundPropertyDTO()
            {
               
            });
            if (resullt.Succeeded)
            {
                Items = resullt.Data;
            }
        }
        public async Task AddNew()
        {

            var parameters = new DialogParameters<CompundPropertyDialog>
            {

            };

            var options = new DialogOptions() { MaxWidth = MaxWidth.Medium };

            var dialog = await DialogService.ShowAsync<CompundPropertyDialog>("CompoundProperty", parameters, options);
            var result = await dialog.Result;
            if (result != null && !result.Canceled)
            {
                await GetAll();
                StateHasChanged();
            }
        }
        async Task Edit(NewCompoundPropertyDTO response)
        {


            var parameters = new DialogParameters<CompundPropertyDialog>
        {

             { x => x.Model, response },
        };
            var options = new DialogOptions() { MaxWidth = MaxWidth.Medium };


            var dialog = await DialogService.ShowAsync<CompundPropertyDialog>("CompoundProperty", parameters, options);
            var result = await dialog.Result;
            if (result != null && !result.Canceled)
            {
                await GetAll();
            }
        }
        public async Task Delete(NewCompoundPropertyDTO response)
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
        HashSet<NewCompoundPropertyDTO> SelecteItems = null!;

    }
}
