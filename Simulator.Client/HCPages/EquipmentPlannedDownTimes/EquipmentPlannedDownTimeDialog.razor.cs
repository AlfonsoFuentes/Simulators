using Simulator.Client.HCPages.Materials;
using Simulator.Shared.Enums.HCEnums.Enums;
using Simulator.Shared.Models.HCs.EquipmentPlannedDownTimes;
using Simulator.Shared.Models.HCs.Materials;

namespace Simulator.Client.HCPages.EquipmentPlannedDownTimes
{
    public partial class EquipmentPlannedDownTimeDialog
    {
        [CascadingParameter]
        private IMudDialogInstance MudDialog { get; set; } = null!;
        private bool Validated { get; set; } = false;
        
        async Task ValidateAsync()
        {
            Validated = _fluentValidationValidator == null ? false : await _fluentValidationValidator.ValidateAsync(options => { options.IncludeAllRuleSets(); });
        }
      
     
        protected override async Task OnInitializedAsync()
        {
            
            await getById();

        }
        FluentValidationValidator _fluentValidationValidator = null!;

        private async Task Submit()
        {
            if (Model.BaseEquipmentId == Guid.Empty)
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
        public EquipmentPlannedDownTimeDTO Model { get; set; } = new();
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
        
    }
}
