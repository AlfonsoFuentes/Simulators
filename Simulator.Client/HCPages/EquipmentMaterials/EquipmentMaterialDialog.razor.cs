using Simulator.Client.HCPages.Materials;
using Simulator.Shared.Enums.HCEnums.Enums;
using Simulator.Shared.Models.HCs.MaterialEquipments;
using Simulator.Shared.Models.HCs.Materials;

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
            var result = await GenericService.Post(Model);


            if (result.Succeeded)
            {
                _snackBar.ShowSuccess(result.Messages);
                MudDialog.Close(DialogResult.Ok(true));
            }
            else
            {
                _snackBar.ShowError(result.Messages);
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
            var result = await GenericService.GetById<MaterialEquipmentDTO, GetMaterialEquipmentByIdRequest>(new()
            {
                Id = Model.Id
            });
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
            IEnumerable<MaterialDTO> FilteredItems = string.IsNullOrEmpty(value) ? MaterialResponseList.Items.AsEnumerable() :
                 MaterialResponseList.Items.Where(Criteria);
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
        MaterialResponseList MaterialResponseList { get; set; } = new();
        async Task GetAllMaterials()
        {
            if (Model.IsMixer || Model.IsSkid)
            {
                var result = await GenericService.GetAll<MaterialResponseList, BackBoneGetAll>(new BackBoneGetAll()
                {

                });
                if (result.Succeeded)
                {
                    MaterialResponseList = result.Data;
                }
            }
            else
            {
                var result = await GenericService.GetAll<MaterialResponseList, MaterialGetAll>(new MaterialGetAll()
                {
                    MaterialType = MaterialType,
                });
                if (result.Succeeded)
                {
                    MaterialResponseList = result.Data;
                }

            }

        }
    }
}
