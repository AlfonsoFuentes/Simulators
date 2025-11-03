using Simulator.Shared.Enums.HCEnums.Enums;
using Simulator.Shared.Models.HCs.BaseEquipments;
using Simulator.Shared.Models.HCs.Conectors;

namespace Simulator.Client.HCPages.Conectors.OutletConnectors
{
    public partial class OutletConnectorDialog
    {
        [CascadingParameter]
        private IMudDialogInstance MudDialog { get; set; } = null!;
        private bool Validated { get; set; } = false;

        async Task ValidateAsync()
        {
            Validated = _fluentValidationValidator == null ? false : await _fluentValidationValidator.ValidateAsync(options => { options.IncludeAllRuleSets(); });
        }

        public BaseEquipmentDTO FromEquipment => Model.From!;
        Guid MainProcessId => Model.MainProcessId;
        ProccesEquipmentType EquipmentType => FromEquipment == null ? ProccesEquipmentType.None : FromEquipment.EquipmentType;
        protected override async Task OnInitializedAsync()
        {
            await GetAllEquipments();
            await getById();

        }
        FluentValidationValidator _fluentValidationValidator = null!;

        private async Task Submit()
        {
            if (Model.FromId == Guid.Empty)
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
        public OutletConnectorDTO Model { get; set; } = new();
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

        List<BaseEquipmentDTO> Items { get; set; } = new();
        async Task GetAllEquipments()
        {
            var result = await ClientService.GetAll(new BaseEquipmentDTO()
            {
                MainProcessId = MainProcessId,
            });
            if (result.Succeeded)
            {
                switch (EquipmentType)
                {
                    case ProccesEquipmentType.Pump:
                        Items = result.Data.Where(x =>
                        x!.EquipmentType == ProccesEquipmentType.Tank ||
                        x!.EquipmentType == ProccesEquipmentType.Mixer ||
                        x!.EquipmentType == ProccesEquipmentType.Line ||
                        x!.EquipmentType == ProccesEquipmentType.ContinuousSystem).ToList();
                        break;
                    case ProccesEquipmentType.Mixer:
                    case ProccesEquipmentType.Tank:
                        Items = result.Data.Where(x => x!.EquipmentType == ProccesEquipmentType.Pump).ToList();
                        break;
                    case ProccesEquipmentType.Operator:
                        Items = result.Data.Where(x => x!.EquipmentType == ProccesEquipmentType.Mixer
                        || x!.EquipmentType == ProccesEquipmentType.ContinuousSystem).ToList();
                        break;
                    case ProccesEquipmentType.ContinuousSystem:
                        Items = result.Data.Where(x => x!.EquipmentType == ProccesEquipmentType.Tank).ToList();
                        break;
                    case ProccesEquipmentType.StreamJoiner:
                        Items = result.Data.Where(x => x!.EquipmentType == ProccesEquipmentType.Line).ToList();
                        break;
                    default:
                        Items = result.Data.Where(x => x!.EquipmentType == ProccesEquipmentType.Pump).ToList();
                        break;

                }
            }

        }
        
    }
}
