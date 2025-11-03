using Simulator.Client.HCPages.Materials;
using Simulator.Shared.Enums.HCEnums.Enums;
using Simulator.Shared.Models.HCs.SKULines;
using Simulator.Shared.Models.HCs.Materials;

namespace Simulator.Client.HCPages.SKULines
{
    public partial class SKULineTable
    {

        [Parameter]
        public EventCallback<List<SKULineDTO>> ItemsChanged { get; set; }
        [Parameter]
        public List<SKULineDTO> Items { get; set; } = new();
        string nameFilter = string.Empty;
        public Func<SKULineDTO, bool> Criteria => x => x.SKU != null && (
        x.SKU.Name.Contains(nameFilter, StringComparison.InvariantCultureIgnoreCase) ||
        x.SKU.SkuCode.Contains(nameFilter, StringComparison.InvariantCultureIgnoreCase));
        public List<SKULineDTO> FilteredItems => string.IsNullOrEmpty(nameFilter) ? Items :
            Items.Where(Criteria).ToList();

        List<SKULineDTO> SKULineResponseList { get; set; } = new();
        [Parameter]
        public Guid LineId { get; set; }
        [Parameter]
        public string EquipmentName { get; set; } = string.Empty;
        string TableLegend => $"SKUs for {EquipmentName}";

        [Parameter]
        public EventCallback ValidateAsync { get; set; }
        bool DisableAdd = false;
        [Parameter]
        public PackageType PackageType { get; set; } = PackageType.None;
        [Parameter]
        public FocusFactory FocusFactory { get; set; } = FocusFactory.None;
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
            if (LineId != Guid.Empty)
            {
                var result = await ClientService.GetAll(new SKULineDTO()
                {
                    LineId = LineId,
                });
                if (result.Succeeded)
                {
                    SKULineResponseList = result.Data;
                    Items = SKULineResponseList;
                    await ItemsChanged.InvokeAsync(Items);
                }
            }

           
        }
        public async Task AddNew()
        {
            SKULineDTO response = new SKULineDTO();
            response.LineId = LineId;

            var parameters = new DialogParameters<SKULineDialog>
            {
                { x => x.Model, response },
                 { x => x.PackageType, PackageType },
                {x=>x.FocusFactory,FocusFactory }

            };

            var options = new DialogOptions() { MaxWidth = MaxWidth.Small };

            var dialog = await DialogService.ShowAsync<SKULineDialog>("SKUs", parameters, options);
            var result = await dialog.Result;
            if (result != null && !result.Canceled)
            {
                if (LineId == Guid.Empty)
                {
                    Items.Add(response);

                }
                await GetAll();
                await ValidateAsync.InvokeAsync();
                StateHasChanged();
            }
        }
        async Task Edit(SKULineDTO response)
        {


            var parameters = new DialogParameters<SKULineDialog>
        {

                { x => x.Model, response },
                { x => x.PackageType, PackageType },
                   {x=>x.FocusFactory,FocusFactory }

        };
            var options = new DialogOptions() { MaxWidth = MaxWidth.Small };


            var dialog = await DialogService.ShowAsync<SKULineDialog>("SKUs", parameters, options);
            var result = await dialog.Result;
            if (result != null && !result.Canceled)
            {
                await GetAll();


            }
        }
        public async Task Delete(SKULineDTO response)
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
               
                if (LineId != Guid.Empty)
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
        HashSet<SKULineDTO> SelecteItems = null!;
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
                
                if (LineId != Guid.Empty)
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
