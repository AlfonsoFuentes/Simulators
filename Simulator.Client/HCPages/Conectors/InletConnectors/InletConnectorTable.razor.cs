using Simulator.Shared.Enums.HCEnums.Enums;
using Simulator.Shared.Models.HCs.BaseEquipments;
using Simulator.Shared.Models.HCs.Conectors;

namespace Simulator.Client.HCPages.Conectors.InletConnectors
{
    public partial class InletConnectorTable
    {

        [Parameter]
        public EventCallback<List<InletConnectorDTO>> ItemsChanged { get; set; }
        [Parameter]
        public List<InletConnectorDTO> Items { get; set; } = new();


        
        public Guid EquipmentId => Equipment == null ? Guid.Empty : Equipment.Id;
        public Guid MainProcessId => Equipment == null ? Guid.Empty : Equipment.MainProcessId;
        public string EquipmentName => Equipment == null ? string.Empty : Equipment.Name;
        string TableLegend => $"Inlet connectors for {EquipmentName}";

        [Parameter]
        public EventCallback ValidateAsync { get; set; }

        [Parameter]
        public BaseEquipmentDTO Equipment { get; set; } = null!;
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await GetAll();
                await ValidateAsync.InvokeAsync();
            }
        }
        async Task GetAll()
        {
            if (EquipmentId != Guid.Empty)
            {
                var result = await ClientService.GetAll(new InletConnectorDTO()
                {
                    ToId = EquipmentId,
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
            InletConnectorDTO response = new InletConnectorDTO();
            response.To = Equipment;
            response.MainProcessId = MainProcessId;

            var parameters = new DialogParameters<InletConnectorDialog>
            {
                { x => x.Model, response },



            };

            var options = new DialogOptions() { MaxWidth = MaxWidth.Medium };

            var dialog = await DialogService.ShowAsync<InletConnectorDialog>("Inlet Connectors", parameters, options);
            var result = await dialog.Result;
            if (result != null && !result.Canceled)
            {
                if (EquipmentId == Guid.Empty)
                {
                    foreach (var from in response.Froms)
                    {
                        Items.Add(new InletConnectorDTO
                        {

                            ToId = EquipmentId,
                            MainProcessId = response.MainProcessId,
                            From = from,


                        });
                    }


                }
                await GetAll();
                await ValidateAsync.InvokeAsync();
                StateHasChanged();
            }
        }
        async Task Edit(InletConnectorDTO response)
        {
            response.To = Equipment;

            var parameters = new DialogParameters<InletConnectorDialog>
        {

                { x => x.Model, response },


        };
            var options = new DialogOptions() { MaxWidth = MaxWidth.Medium };


            var dialog = await DialogService.ShowAsync<InletConnectorDialog>("Material", parameters, options);
            var result = await dialog.Result;
            if (result != null && !result.Canceled)
            {
                await GetAll();


            }
        }
        public async Task Delete(InletConnectorDTO response)
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
              
                if (EquipmentId != Guid.Empty)
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
        HashSet<InletConnectorDTO> SelecteItems = null!;
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
               
                if (EquipmentId != Guid.Empty)
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
}
