using Simulator.Client.HCPages.Materials;
using Simulator.Shared.Enums.HCEnums.Enums;
using Simulator.Shared.Models.HCs.BackBoneSteps;
using Simulator.Shared.Models.HCs.BaseEquipments;
using Simulator.Shared.Models.HCs.Conectors;
using Simulator.Shared.Models.HCs.MaterialEquipments;
using Simulator.Shared.Models.HCs.Materials;
using Simulator.Shared.Models.HCs.MixerPlanneds;
using Simulator.Shared.Models.HCs.Mixers;
using Simulator.Shared.Models.HCs.Tanks;
using static MudBlazor.CategoryTypes;
using static Simulator.Shared.StaticClasses.StaticClass;

namespace Simulator.Client.HCPages.MixerPlanneds;
public partial class MixerPlannedDialog
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
        await GetAllMixers();
        await getById();



    }
    MixerResponseList MixerResponseList = new();
    async Task GetAllMixers()
    {
        var result = await GenericService.GetAll<MixerResponseList, MixerGetAll>(new MixerGetAll()
        {
            MainProcessId = Model.MainProcesId,


        });
        if (result.Succeeded)
        {
            MixerResponseList = result.Data;
        }
    }
    FluentValidationValidator _fluentValidationValidator = null!;

    private async Task Submit()
    {
        if (Model.SimulationPlannedId == Guid.Empty)
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
    public MixerPlannedDTO Model { get; set; } = new();
    async Task getById()
    {
        if (Model.Id == Guid.Empty)
        {
            return;
        }
        var result = await GenericService.GetById<MixerPlannedDTO, GetMixerPlannedByIdRequest>(new()
        {
            Id = Model.Id
        });
        if (result.Succeeded)
        {
            Model = result.Data;
            await ChangeMixer();
            await ChangeBackBone();
            await GetAllWipsTanks();
        }
    }

    private Task<IEnumerable<MixerDTO?>> SearchMixer(string value, CancellationToken token)
    {
        Func<MixerDTO, bool> Criteria = x =>
        x.Name.Contains(value, StringComparison.InvariantCultureIgnoreCase)
        ;
        IEnumerable<MixerDTO?> FilteredItems = string.IsNullOrEmpty(value) ? MixerResponseList.Items.AsEnumerable() :
             MixerResponseList.Items.Where(Criteria);
        return Task.FromResult(FilteredItems);
    }
    MaterialEquipmentResponseList MaterialEquipmentResponseList = new();
    List<MaterialDTO> Materials => MaterialEquipmentResponseList.Items.Count == 0 ? new() : MaterialEquipmentResponseList.Items.Select(x => x.Material!).ToList();
    private async Task ChangeMixer()
    {
        var result = await GenericService.GetAll<MaterialEquipmentResponseList, MaterialEquipmentGetAll>(new MaterialEquipmentGetAll()
        {
            EquipmentId = Model.MixerId,
        });
        if (result.Succeeded)
        {
            MaterialEquipmentResponseList = result.Data;


        }
        await GetAllWipsTanks();
    }
    private Task<IEnumerable<MaterialDTO>> SearchMaterial(string value, CancellationToken token)
    {

        Func<MaterialDTO, bool> Criteria = x =>
        x.SAPName.Contains(value, StringComparison.InvariantCultureIgnoreCase) ||
         x.M_Number.Contains(value, StringComparison.InvariantCultureIgnoreCase)
        ;
        IEnumerable<MaterialDTO> FilteredItems = string.IsNullOrEmpty(value) ? Materials.AsEnumerable() :
             Materials.Where(Criteria);
        return Task.FromResult(FilteredItems);
    }
    BackBoneStepResponseList BackBoneStepResponseList = new();
    private async Task ChangeBackBone()
    {
        var selectedmaterialEquipment = MaterialEquipmentResponseList.Items.FirstOrDefault(x => x.MaterialId == Model.BackBone.Id && x.ProccesEquipmentId == Model.MixerId);
        if (selectedmaterialEquipment != null)
        {
            Model.Capacity = selectedmaterialEquipment.Capacity;
            Model.ChangeCapacity();
        }
        var result = await GenericService.GetAll<BackBoneStepResponseList, BackBoneStepGetAll>(new BackBoneStepGetAll()
        {
            MaterialId = Model.BackBone.Id,
        });
        if (result.Succeeded)
        {
            BackBoneStepResponseList = result.Data;
            Model.BackBoneSteps = result.Data.Items;

        }
    }
    private Task<IEnumerable<BackBoneStepDTO>> SearchBackboneStep(string value, CancellationToken token)
    {

        Func<BackBoneStepDTO, bool> Criteria = x =>
        x.StepName.Contains(value, StringComparison.InvariantCultureIgnoreCase)
        ;
        IEnumerable<BackBoneStepDTO> FilteredItems = string.IsNullOrEmpty(value) ? BackBoneStepResponseList.Items.AsEnumerable() :
             BackBoneStepResponseList.Items.Where(Criteria);
        return Task.FromResult(FilteredItems);
    }
    private Task<IEnumerable<BaseEquipmentDTO?>> SearchWipTank(string value, CancellationToken token)
    {

        Func<BaseEquipmentDTO, bool> Criteria = x =>
        x.Name.Contains(value, StringComparison.InvariantCultureIgnoreCase)
        ;
        IEnumerable<BaseEquipmentDTO?> FilteredItems = string.IsNullOrEmpty(value) ? WipTanks.AsEnumerable() :
             WipTanks.Where(Criteria);
        return Task.FromResult(FilteredItems);
    }
    OutletConnectorResponseList OutletConnectorResponseList = new();
    public List<BaseEquipmentDTO> WipTanks => OutletConnectorResponseList.Items.Count == 0 ? new() : OutletConnectorResponseList.Items.Select(x => x.To!).ToList();
    async Task GetAllWipsTanks()
    {
        var result = await GenericService.GetAll<OutletConnectorResponseList, OutletsConnectorGetAll>(new OutletsConnectorGetAll()
        {
            FromId = Model.MixerId,
        });
        if (result.Succeeded)
        {
            var MixerPump = result.Data.Items.FirstOrDefault();
            if (MixerPump != null)
            {
                var resultWips = await GenericService.GetAll<OutletConnectorResponseList, OutletsConnectorGetAll>(new OutletsConnectorGetAll()
                {
                    FromId = MixerPump.ToId,
                });
                if (resultWips.Succeeded)
                {
                    OutletConnectorResponseList = resultWips.Data;
                }
            }


        }
    }

    void ChangeBackBoneStep()
    {
        Model.MixerLevel = Model.CalculateMixerLevel();
        Model.ChangeMixerLevel();
    }

}
