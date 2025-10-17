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


        OutletConnectorResponseList OutletConnectorResponseList { get; set; } = new();

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
                var result = await GenericService.GetAll<OutletConnectorResponseList, OutletsConnectorGetAll>(new OutletsConnectorGetAll()
                {
                    FromId = EquipmentId,
                });
                if (result.Succeeded)
                {
                    OutletConnectorResponseList = result.Data;
                    Items = OutletConnectorResponseList.Items;
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
            { x => x.ContentText, $"Do you really want to delete {response.Name}? This process cannot be undone." },
            { x => x.ButtonText, "Delete" },
            { x => x.Color, Color.Error }
        };

            var options = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall };

            var dialog = await DialogService.ShowAsync<DialogTemplate>("Delete", parameters, options);
            var result = await dialog.Result;


            if (!result!.Canceled)
            {
                DeleteConectorRequest request = new()
                {
                    Id = response.Id,
                    Name = response.Name,

                };
                if (EquipmentId != Guid.Empty)
                {
                    var resultDelete = await GenericService.Post(request);
                    if (resultDelete.Succeeded)
                    {

                        _snackBar.ShowSuccess(resultDelete.Messages);

                    }
                    else
                    {
                        _snackBar.ShowError(resultDelete.Messages);
                    }
                }
                else
                {
                    Items.Remove(response);
                }
                await GetAll();
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
                DeleteGroupConectorRequest request = new()
                {
                    SelecteItems = SelecteItems.Select(x => x as ConectorDTO).ToHashSet(),


                };
                if (EquipmentId != Guid.Empty)
                {
                    var resultDelete = await GenericService.Post(request);
                    if (resultDelete.Succeeded)
                    {


                        _snackBar.ShowSuccess(resultDelete.Messages);
                        SelecteItems = null!;

                    }
                    else
                    {
                        _snackBar.ShowError(resultDelete.Messages);
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
    }
}
