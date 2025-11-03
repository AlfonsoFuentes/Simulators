using Simulator.Shared.Models.HCs.LinePlanneds;
using Simulator.Shared.Models.HCs.Lines;
using Simulator.Shared.Models.HCs.PlannedSKUs;
using Simulator.Shared.Models.HCs.PreferedMixers;
using Simulator.Shared.Models.HCs.SKULines;
using static MudBlazor.CategoryTypes;
using static Simulator.Shared.StaticClasses.StaticClass;

namespace Simulator.Client.HCPages.LinePlanneds;
public partial class LinePlannedDialog
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
        await GetAllLines();
        await getById();
        await GetAllPlannedSKUS();
        await GetAllPreferedMixers();

    }
    async Task GetAllPreferedMixers()
    {
        if (Model.Id != Guid.Empty)
        {
            var result = await ClientService.GetAll(new PreferedMixerDTO()
            {
                LinePlannedId = Model.Id,


            });
            if (result.Succeeded)
            {
                Model.PreferedMixerDTOs = result.Data;
        
            }

        }

    }
    async Task GetAllPlannedSKUS()
    {
        if (Model.Id != Guid.Empty)
        {
            var result = await ClientService.GetAll( new  PlannedSKUDTO()
            {
                LinePlannedId = Model.Id,


            });
            if (result.Succeeded)
            {
                Model.PlannedSKUDTOs = result.Data;

            }

        }

    }
    List<LineDTO> LineResponseList = new();
    async Task GetAllLines()
    {
        var result = await ClientService.GetAll(new LineDTO()
        {
            MainProcessId = Model.MainProcesId,


        });
        if (result.Succeeded)
        {
            LineResponseList = result.Data;
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
    public LinePlannedDTO Model { get; set; } = new();
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

    private Task<IEnumerable<LineDTO>> SearchLine(string value, CancellationToken token)
    {
        Func<LineDTO, bool> Criteria = x =>
        x.Name.Contains(value, StringComparison.InvariantCultureIgnoreCase)
        ;
        IEnumerable<LineDTO> FilteredItems = string.IsNullOrEmpty(value) ? LineResponseList.AsEnumerable() :
             LineResponseList.Where(Criteria);
        return Task.FromResult(FilteredItems);
    }


}
