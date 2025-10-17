using Simulator.Shared.Enums.HCEnums.Enums;
using Simulator.Shared.Models.HCs.BaseEquipments;
using Simulator.Shared.Models.HCs.Conectors;
using Simulator.Shared.Models.HCs.Lines;

namespace Simulator.Client.HCPages.Conectors.InletConnectors
{
    public partial class InletConnectorDialog
    {
        [CascadingParameter]
        private IMudDialogInstance MudDialog { get; set; } = null!;
        private bool Validated { get; set; } = false;

        async Task ValidateAsync()
        {
            Validated = _fluentValidationValidator == null ? false : await _fluentValidationValidator.ValidateAsync(options => { options.IncludeAllRuleSets(); });
        }

        Guid MainProcessId => Model.MainProcessId;
        ProccesEquipmentType EquipmentType => ToEquipment == null ? ProccesEquipmentType.None : ToEquipment.EquipmentType;
      
        public BaseEquipmentDTO ToEquipment=>Model.To!;

        protected override async Task OnInitializedAsync()
        {
            await GetAllEquipments();
            await getById();

        }
        FluentValidationValidator _fluentValidationValidator = null!;

        private async Task Submit()
        {
            if (Model.ToId == Guid.Empty)
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
        public InletConnectorDTO Model { get; set; } = new();
        async Task getById()
        {
            if (Model.Id == Guid.Empty)
            {
                return;
            }
            var result = await GenericService.GetById<InletConnectorDTO, GetInletConectorByIdRequest>(new()
            {
                Id = Model.Id
            });
            if (result.Succeeded)
            {
                Model = result.Data;
            }
        }

        List<BaseEquipmentDTO?> Items { get; set; } = new();
        async Task GetAllEquipments()
        {
            var result = await GenericService.GetAll<BaseEquipmentList, BaseEquipmentGetAll>(new BaseEquipmentGetAll()
            {
                MainProcessId = MainProcessId,
            });
            if (result.Succeeded)
            {
                switch (EquipmentType)
                {
                    case ProccesEquipmentType.Pump:
                        Items = result.Data.Items.Where(x => 
                        x!.EquipmentType == ProccesEquipmentType.Tank ||
                        x!.EquipmentType == ProccesEquipmentType.Mixer).ToList();
                        break;
                    case ProccesEquipmentType.Mixer:
                        Items = result.Data.Items.Where(x => x!.EquipmentType == ProccesEquipmentType.Pump || x.EquipmentType == ProccesEquipmentType.Operator).ToList();
                        break;
                    case ProccesEquipmentType.Tank:
                    case ProccesEquipmentType.Line:
                        Items = result.Data.Items.Where(x => x!.EquipmentType == ProccesEquipmentType.Pump).ToList();
                        break;
                    case ProccesEquipmentType.StreamJoiner:
                        Items = result.Data.Items.Where(x => x!.EquipmentType == ProccesEquipmentType.Pump).ToList();
                        break;
                    default:
                        Items = result.Data.Items.Where(x => x!.EquipmentType == ProccesEquipmentType.Pump).ToList();
                        break;

                }

            }

        }
        //private Task<IEnumerable<BaseEquipmentDTO?>> SearchEquipment(string value, CancellationToken token)
        //{
        //    Func<BaseEquipmentDTO, bool> Criteria = x =>
        //    x!.Name.Contains(value, StringComparison.InvariantCultureIgnoreCase)
        //    ;
        //    var items = Items;
        //    if (EquipmentType == ProccesEquipmentType.Pump)
        //    {
        //        items = items.Where(x => x.EquipmentType != ProccesEquipmentType.Pump).ToList();
        //    }
        //    else if (EquipmentType == ProccesEquipmentType.Mixer)
        //    {
        //        items = items.Where(x => x.EquipmentType == ProccesEquipmentType.Pump || x.EquipmentType == ProccesEquipmentType.Operator).ToList();
        //    }
        //    else
        //    {
        //        items = items.Where(x => x.EquipmentType == ProccesEquipmentType.Pump).ToList();
        //    }
        //    IEnumerable<BaseEquipmentDTO?> FilteredItems = string.IsNullOrEmpty(value) ? items.AsEnumerable() :
        //         items.Where(Criteria);
        //    return Task.FromResult(FilteredItems);
        //}
    }
}
