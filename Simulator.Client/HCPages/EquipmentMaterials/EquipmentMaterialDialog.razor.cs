using Simulator.Client.HCPages.Materials;
using Simulator.Shared.Enums.HCEnums.Enums;
using Simulator.Shared.Models.HCs.MaterialEquipments;
using Simulator.Shared.Models.HCs.Materials;
using System.Collections.Generic;

namespace Simulator.Client.HCPages.EquipmentMaterials
{
    public partial class EquipmentMaterialDialog
    {
        [CascadingParameter]
        private IMudDialogInstance MudDialog { get; set; } = null!;
        private bool Validated { get; set; } = false;
        [Parameter]
        public bool IsStorageForOneFluid { get; set; } = false;
        async Task ValidateAsync()
        {
            Validated = _fluentValidationValidator == null ? false : await _fluentValidationValidator.ValidateAsync(options => { options.IncludeAllRuleSets(); });
        }
        [Parameter]
        public MaterialType MaterialType { get; set; } = MaterialType.None;
        [Parameter]
        public FocusFactory FocusFactory { get; set; } = FocusFactory.None;

        protected override async Task OnInitializedAsync()
        {
            await GetAllMaterials();
            await getById();

        }
        FluentValidationValidator _fluentValidationValidator = null!;

        private async Task Submit()
        {
            if (Model.ProccesEquipmentId == Guid.Empty)
            {
                MudDialog.Close(DialogResult.Ok(true));
                return;
            }
            var result = await ClientService.Save(Model);


            if (result.Succeeded)
            {

                MudDialog.Close(DialogResult.Ok(true));
            }


        }


        private void Cancel() => MudDialog.Cancel();

        [Parameter]
        public MaterialEquipmentDTO Model { get; set; } = new();
        async Task getById()
        {
            if (Model.Id == Guid.Empty)
            {
                return;
            }
            var result = await ClientService.GetById(Model);

            if (result.Succeeded)
            {
                Model = result.Data;
            }
        }
        private Task<IEnumerable<MaterialDTO>> SearchMaterial(string value, CancellationToken token)
        {

            Func<MaterialDTO, bool> Criteria = x =>
                x.SAPName.Contains(value, StringComparison.InvariantCultureIgnoreCase) ||
             x.M_Number.Contains(value, StringComparison.InvariantCultureIgnoreCase) ||
             x.CommonName.Contains(value, StringComparison.InvariantCultureIgnoreCase)
            ;
            IEnumerable<MaterialDTO> FilteredItems = string.IsNullOrEmpty(value) ? MaterialResponseList.AsEnumerable() :
                 MaterialResponseList.Where(Criteria);
            return Task.FromResult(FilteredItems);
        }
        public async Task AddMaterial()
        {
            MaterialDTO Model = new MaterialDTO()
            {
                MaterialType = MaterialType,


            };

            var parameters = new DialogParameters<MaterialDialog>
            {
                 { x => x.Model, Model },
            };

            var options = new DialogOptions() { MaxWidth = MaxWidth.Medium };

            var dialog = await DialogService.ShowAsync<MaterialDialog>("Material", parameters, options);
            var result = await dialog.Result;
            if (result != null)
            {
                await GetAllMaterials();

            }
        }
        List<MaterialDTO> MaterialResponseList { get; set; } = new();
        async Task GetAllMaterials()
        {
            //var result = await ClientService.GetAll(new MaterialDTO()
            //{

            //    FocusFactory = FocusFactory
            //});
            //if (result.Succeeded)
            //{
            //    MaterialResponseList = result.Data;
            //}
            if (Model.IsMixer || Model.IsSkid)
            {
                var result = await ClientService.GetAll(new BackBoneDto()
                {

                    FocusFactory = FocusFactory
                });
                if (result.Succeeded)
                {
                    MaterialResponseList = result.Data.Cast<MaterialDTO>().ToList();
                }
            }
            else
            {
                var result = await ClientService.GetAll(new MaterialDTO()
                {
                    MaterialType = MaterialType,
                    FocusFactory = FocusFactory

                });
                if (result.Succeeded)
                {
                    MaterialResponseList = result.Data.Cast<MaterialDTO>().ToList();
                }

            }

        }
    }
}
