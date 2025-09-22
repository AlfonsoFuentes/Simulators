﻿using Simulator.Client.HCPages.Materials;
using Simulator.Shared.Enums.HCEnums.Enums;
using Simulator.Shared.Models.HCs.EquipmentPlannedDownTimes;
using Simulator.Shared.Models.HCs.Materials;

namespace Simulator.Client.HCPages.EquipmentPlannedDownTimes
{
    public partial class EquipmentPlannedDownTimeTable
    {
      
        [Parameter]
        public EventCallback<List<EquipmentPlannedDownTimeDTO>> ItemsChanged { get; set; }
        [Parameter]
        public List<EquipmentPlannedDownTimeDTO> Items { get; set; } = new();
        string nameFilter = string.Empty;
        public Func<EquipmentPlannedDownTimeDTO, bool> Criteria => x =>  x.StartTimeString.Contains(nameFilter, StringComparison.InvariantCultureIgnoreCase);
        public List<EquipmentPlannedDownTimeDTO> FilteredItems => string.IsNullOrEmpty(nameFilter) ? Items :
            Items.Where(Criteria).ToList();

        EquipmentPlannedDownTimeResponseList EquipmentPlannedDownTimeResponseList { get; set; } = new();
        [Parameter]
        public Guid EquipmentId { get; set; }
        [Parameter]
        public string EquipmentName { get; set; } = string.Empty;
        string TableLegend => $"Planned down time for {EquipmentName}";
       
        [Parameter]
        public EventCallback ValidateAsync { get; set; }
        bool DisableAdd=  false;
        
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await GetAll();
                await ValidateAsync.InvokeAsync();
                StateHasChanged();
            }
        }
        async Task GetAll()
        {
            if (EquipmentId != Guid.Empty)
            {
                var result = await GenericService.GetAll<EquipmentPlannedDownTimeResponseList, EquipmentPlannedDownTimeGetAll>(new EquipmentPlannedDownTimeGetAll()
                {
                    EquipmentId = EquipmentId,
                });
                if (result.Succeeded)
                {
                    EquipmentPlannedDownTimeResponseList = result.Data;
                    Items = EquipmentPlannedDownTimeResponseList.Items;
                    await ItemsChanged.InvokeAsync(Items);

                }
             
            }
       
         
        }
        public async Task AddNew()
        {
            EquipmentPlannedDownTimeDTO response = new EquipmentPlannedDownTimeDTO();
            response.BaseEquipmentId = EquipmentId;
        
            var parameters = new DialogParameters<EquipmentPlannedDownTimeDialog>
            {
                { x => x.Model, response },
             
            
            };

            var options = new DialogOptions() { MaxWidth = MaxWidth.Small };

            var dialog = await DialogService.ShowAsync<EquipmentPlannedDownTimeDialog>("Material", parameters, options);
            var result = await dialog.Result;
            if (result != null && !result.Canceled)
            {
                if (EquipmentId == Guid.Empty)
                {
                    Items.Add(response);

                }
                await GetAll();
                await ValidateAsync.InvokeAsync();
                StateHasChanged();
            }
        }
        async Task Edit(EquipmentPlannedDownTimeDTO response)
        {


            var parameters = new DialogParameters<EquipmentPlannedDownTimeDialog>
        {

                { x => x.Model, response },
       
                
        };
            var options = new DialogOptions() { MaxWidth = MaxWidth.Small };


            var dialog = await DialogService.ShowAsync<EquipmentPlannedDownTimeDialog>("Material", parameters, options);
            var result = await dialog.Result;
            if (result != null && !result.Canceled)
            {
                await GetAll();


            }
        }
        public async Task Delete(EquipmentPlannedDownTimeDTO response)
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
                DeleteEquipmentPlannedDownTimeRequest request = new()
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
        HashSet<EquipmentPlannedDownTimeDTO> SelecteItems = null!;
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
                DeleteGroupEquipmentPlannedDownTimeRequest request = new()
                {
                    SelecteItems = SelecteItems,
                    EquipmentId = EquipmentId,

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
