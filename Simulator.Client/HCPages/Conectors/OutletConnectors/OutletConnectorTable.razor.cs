using Simulator.Shared.Enums.HCEnums.Enums;
using Simulator.Shared.Models.HCs.BaseEquipments;
using Simulator.Shared.Models.HCs.Conectors;

namespace Simulator.Client.HCPages.Conectors.OutletConnectors
{
    public partial class OutletConnectorTable
    {

        [Parameter]
        public EventCallback<List<OutletConnectorDTO>> ItemsChanged { get; set; }
        [Parameter]
        public List<OutletConnectorDTO> Items { get; set; } = new();


      

        public Guid EquipmentId => Equipment == null ? Guid.Empty : Equipment.Id;
        public Guid MainProcessId => Equipment == null ? Guid.Empty : Equipment.MainProcessId;
        public string EquipmentName => Equipment == null ? string.Empty : Equipment.Name;
        string TableLegend => $"Outlet connectors for {EquipmentName}";
        [Parameter]
        public BaseEquipmentDTO Equipment { get; set; } = null!;
        [Parameter]
        public EventCallback ValidateAsync { get; set; }

     
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
                var result = await ClientService.GetAll(new OutletConnectorDTO()
                {
                    FromId = EquipmentId,
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
            OutletConnectorDTO response = new OutletConnectorDTO();
            response.From = Equipment;
            response.MainProcessId = MainProcessId;
            var parameters = new DialogParameters<OutletConnectorDialog>
            {
                { x => x.Model, response },
              
             

            };

            var options = new DialogOptions() { MaxWidth = MaxWidth.Medium };

            var dialog = await DialogService.ShowAsync<OutletConnectorDialog>("Outlets Connector", parameters, options);
            var result = await dialog.Result;
            if (result != null && !result.Canceled)
            {
                if (EquipmentId == Guid.Empty)
                {
                    foreach (var to in response.Tos)
                    {
                        Items.Add(new OutletConnectorDTO
                        {
                            FromId = EquipmentId,
                        
                            MainProcessId = response.MainProcessId,
                            To = to,


                        });
                    }
               

                }
                await GetAll();
                await ValidateAsync.InvokeAsync();
                StateHasChanged();
            }
        }
        async Task Edit(OutletConnectorDTO response)
        {
            response.From = Equipment;

            var parameters = new DialogParameters<OutletConnectorDialog>
        {

                { x => x.Model, response },
    

        };
            var options = new DialogOptions() { MaxWidth = MaxWidth.Medium };


            var dialog = await DialogService.ShowAsync<OutletConnectorDialog>("Material", parameters, options);
            var result = await dialog.Result;
            if (result != null && !result.Canceled)
            {
                await GetAll();


            }
        }
        public async Task Delete(OutletConnectorDTO response)
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
        HashSet<OutletConnectorDTO> SelecteItems = null!;
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
