using Simulator.Client.HCPages.Materials;
using Simulator.Client.HCPages.SKUs;
using Simulator.Shared.Enums.HCEnums.Enums;
using Simulator.Shared.Models.HCs.Materials;
using Simulator.Shared.Models.HCs.SKULines;
using Simulator.Shared.Models.HCs.SKUs;
using System.Linq;
using static MudBlazor.CategoryTypes;

namespace Simulator.Client.HCPages.SKULines
{
    public partial class SKULineDialog
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
        public PackageType PackageType { get; set; } = PackageType.None;
        [Parameter]
        public FocusFactory FocusFactory { get; set; } = FocusFactory.None;
        protected override async Task OnInitializedAsync()
        {
            await GetAllSkUs();
            await getById();

        }
        FluentValidationValidator _fluentValidationValidator = null!;

        private async Task Submit()
        {
            if (Model.LineId == Guid.Empty)
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
        public SKULineDTO Model { get; set; } = new();
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
        private Task<IEnumerable<SKUDTO>> SearchSKU(string value, CancellationToken token)
        {

            Func<SKUDTO, bool> Criteria = x =>
            x.SkuCode.Contains(value, StringComparison.InvariantCultureIgnoreCase) ||
             x.Name.Contains(value, StringComparison.InvariantCultureIgnoreCase) ||
             x.BackBoneCommonName.Contains(value, StringComparison.InvariantCultureIgnoreCase) ||
             x.BackBoneM_Number.Contains(value, StringComparison.InvariantCultureIgnoreCase)
            ;
            var Skufiltered = SKUResponseList.  Where(x => x.PackageType == PackageType);
            IEnumerable<SKUDTO> FilteredItems = string.IsNullOrEmpty(value) ? Skufiltered :
                 Skufiltered.Where(Criteria);
            return Task.FromResult(FilteredItems);
        }
        public async Task AddSKU()
        {
            SKUDTO Model = new SKUDTO()
            {
                PackageType = PackageType,
                FocusFactory=FocusFactory,


            };

            var parameters = new DialogParameters<SKUDialog>
            {
                 { x => x.Model, Model },
            };

            var options = new DialogOptions() { MaxWidth = MaxWidth.Medium };

            var dialog = await DialogService.ShowAsync<SKUDialog>("Sku", parameters, options);
            var result = await dialog.Result;
            if (result != null)
            {
                await GetAllSkUs();

            }
        }
        List<SKUDTO> SKUResponseList { get; set; } = new();
        async Task GetAllSkUs()
        {
            var result = await ClientService.GetAll(new SKUDTO()
            {
                 FocusFactory= FocusFactory,
            });
            if (result.Succeeded)
            {
                SKUResponseList = result.Data;
            }

        }
    }
}
