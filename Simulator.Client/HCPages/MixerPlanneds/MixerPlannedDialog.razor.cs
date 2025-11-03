using Simulator.Shared.Models.HCs.BackBoneSteps;
using Simulator.Shared.Models.HCs.BaseEquipments;
using Simulator.Shared.Models.HCs.Conectors;
using Simulator.Shared.Models.HCs.MaterialEquipments;
using Simulator.Shared.Models.HCs.Materials;
using Simulator.Shared.Models.HCs.MixerPlanneds;
using Simulator.Shared.Models.HCs.Mixers;

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
    List<MixerDTO> MixerResponseList = new();
    async Task GetAllMixers()
    {
        var result = await ClientService.GetAll(new MixerDTO()
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
        var result = await ClientService.Save(Model);


        if (result.Succeeded)
        {
          
            MudDialog.Close(DialogResult.Ok(true));
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
        var result = await ClientService.GetById(Model);
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
        IEnumerable<MixerDTO?> FilteredItems = string.IsNullOrEmpty(value) ? MixerResponseList.AsEnumerable() :
             MixerResponseList.Where(Criteria);
        return Task.FromResult(FilteredItems);
    }
    List<MaterialEquipmentDTO> MaterialEquipmentResponseList = new();
    List<MaterialDTO> Materials => MaterialEquipmentResponseList.       Count == 0 ? new() : MaterialEquipmentResponseList.Select(x => x.Material!).ToList();
    private async Task ChangeMixer()
    {
        var result = await ClientService.GetAll(new MaterialEquipmentDTO()
        {
            ProccesEquipmentId = Model.MixerId,
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
    List<BackBoneStepDTO> BackBoneStepResponseList = new();
    private async Task ChangeBackBone()
    {
        var selectedmaterialEquipment = MaterialEquipmentResponseList.  FirstOrDefault(x => x.MaterialId == Model.BackBone.Id && x.ProccesEquipmentId == Model.MixerId);
        if (selectedmaterialEquipment != null)
        {
            Model.Capacity = selectedmaterialEquipment.Capacity;
            Model.ChangeCapacity();
        }
        var result = await ClientService.GetAll(new BackBoneStepDTO()
        {
            MaterialId = Model.BackBone.Id,
        });
        if (result.Succeeded)
        {
            BackBoneStepResponseList = result.Data;
            Model.BackBoneSteps = result.Data;

        }
    }
    private Task<IEnumerable<BackBoneStepDTO>> SearchBackboneStep(string value, CancellationToken token)
    {

        Func<BackBoneStepDTO, bool> Criteria = x =>
        x.StepName.Contains(value, StringComparison.InvariantCultureIgnoreCase)
        ;
        IEnumerable<BackBoneStepDTO> FilteredItems = string.IsNullOrEmpty(value) ? BackBoneStepResponseList.AsEnumerable() :
             BackBoneStepResponseList.Where(Criteria);
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
    List<OutletConnectorDTO> OutletConnectorResponseList = new();
    public List<BaseEquipmentDTO> WipTanks => OutletConnectorResponseList.Count == 0 ? new() : OutletConnectorResponseList.Select(x => x.To!).ToList();
    async Task GetAllWipsTanks()
    {
        var result = await ClientService.GetAll(new OutletConnectorDTO()
        {
            FromId = Model.MixerId,
        });
        if (result.Succeeded)
        {
            var MixerPump = result.Data.FirstOrDefault();
            if (MixerPump != null)
            {
                var resultWips = await ClientService.GetAll(new OutletConnectorDTO()
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
