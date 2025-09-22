﻿using Simulator.Client.HCPages.Materials;
using Simulator.Shared.Enums.HCEnums.Enums;
using Simulator.Shared.Models.HCs.MaterialEquipments;
using Simulator.Shared.Models.HCs.Materials;

namespace Simulator.Client.HCPages.EquipmentMaterials
{
    public partial class EquipmentMaterialTable
    {

        [Parameter]
        public EventCallback<List<MaterialEquipmentDTO>> ItemsChanged { get; set; }
        [Parameter]
        public List<MaterialEquipmentDTO> Items { get; set; } = new();
        string nameFilter = string.Empty;
        public Func<MaterialEquipmentDTO, bool> Criteria => x => x.Material != null &&
        (x.MaterialCommonName.Contains(nameFilter, StringComparison.InvariantCultureIgnoreCase) ||
        x.MaterialM_Number.Contains(nameFilter, StringComparison.InvariantCultureIgnoreCase) ||
        x.MaterialSAPName.Contains(nameFilter, StringComparison.InvariantCultureIgnoreCase)

        );
        public List<MaterialEquipmentDTO> FilteredItems => string.IsNullOrEmpty(nameFilter) ? Items :
            Items.Where(Criteria).ToList();

        MaterialEquipmentResponseList MaterialEquipmentResponseList { get; set; } = new();
        [Parameter]
        public Guid EquipmentId { get; set; }
        [Parameter]
        public string EquipmentName { get; set; } = string.Empty;
        string TableLegend => $"Materials for {EquipmentName}";
        [Parameter]
        public bool IsStorageForOneFluid { get; set; } = false;
        [Parameter]
        public EventCallback ValidateAsync { get; set; }
        bool DisableAdd => IsStorageForOneFluid && Items.Count >= 1 ? true : false;

        [Parameter]
        public FluidToStorage FluidToStorage { get; set; } = FluidToStorage.None;
        [Parameter]
        public MaterialType MaterialType { get; set; } = MaterialType.None;
        [Parameter]
        public bool IsMixer { get; set; } = false;
        [Parameter]
        public bool IsSkid { get; set; } = false;
        [Parameter]
        public Guid MainProcessId  { get; set; } 
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
                var result = await GenericService.GetAll<MaterialEquipmentResponseList, MaterialEquipmentGetAll>(new MaterialEquipmentGetAll()
                {
                    EquipmentId = EquipmentId,
                });
                if (result.Succeeded)
                {
                    MaterialEquipmentResponseList = result.Data;
                    Items = MaterialEquipmentResponseList.Items;     
                    await ItemsChanged.InvokeAsync(Items);

                }

         
            }

        }
        public async Task AddNew()
        {
            MaterialEquipmentDTO response = new MaterialEquipmentDTO();
            response.ProccesEquipmentId = EquipmentId;
            response.IsMixer = IsMixer;
            response.IsSkid = IsSkid;
            response.MainProcessId = MainProcessId;
            var parameters = new DialogParameters<EquipmentMaterialDialog>
            {
                { x => x.Model, response },
                {x=>x.IsStorageForOneFluid,IsStorageForOneFluid },
                {x=>x.MaterialType,MaterialType },

            };

            var options = new DialogOptions() { MaxWidth = MaxWidth.Medium };

            var dialog = await DialogService.ShowAsync<EquipmentMaterialDialog>("Material", parameters, options);
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
        async Task Edit(MaterialEquipmentDTO response)
        {
            response.IsMixer = IsMixer;
            response.IsSkid = IsSkid;
            var parameters = new DialogParameters<EquipmentMaterialDialog>
        {

                { x => x.Model, response },
                {x=>x.IsStorageForOneFluid,IsStorageForOneFluid },
                {x=>x.MaterialType,MaterialType },

        };
            var options = new DialogOptions() { MaxWidth = MaxWidth.Medium };


            var dialog = await DialogService.ShowAsync<EquipmentMaterialDialog>("Material", parameters, options);
            var result = await dialog.Result;
            if (result != null && !result.Canceled)
            {
                await GetAll();


            }
        }
        public async Task Delete(MaterialEquipmentDTO response)
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
                DeleteMaterialEquipmentRequest request = new()
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
        HashSet<MaterialEquipmentDTO> SelecteItems = null!;
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
                DeleteGroupMaterialEquipmentRequest request = new()
                {
                    SelecteItems = SelecteItems,
                    EquipmentId = EquipmentId,
                    MainProcessId = MainProcessId,

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
