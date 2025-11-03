using Simulator.Shared.Models.HCs.Mixers;
using Simulator.Shared.Models.HCs.PreferedMixers;
using Simulator.Shared.Models.HCs.SKULines;
using Simulator.Shared.Models.HCs.SKUs;

namespace Simulator.Client.HCPages.PreferedMixers
{
    public partial class PreferedMixerDialog
    {
        [CascadingParameter]
        private IMudDialogInstance MudDialog { get; set; } = null!;
        private bool Validated { get; set; } = false;
        async Task ValidateAsync()
        {
            Validated = _fluentValidationValidator == null ? false : await _fluentValidationValidator.ValidateAsync(options => { options.IncludeAllRuleSets(); });
        }
        [Parameter]
        public Guid MainProcessId {  get; set; }
        protected override async Task OnInitializedAsync()
        {
            await GetAllMixers();
            await getById();



        }


        FluentValidationValidator _fluentValidationValidator = null!;

        private async Task Submit()
        {
            if (Model.LinePlannedId == Guid.Empty)
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
        public PreferedMixerDTO Model { get; set; } = new();
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
 
        List<MixerDTO> Mixers= new();
        async Task GetAllMixers()
        {
            var result = await ClientService.GetAll(new MixerDTO()
            {
                MainProcessId = MainProcessId,
            });
            if (result.Succeeded)
            {
                Mixers = result.Data;


            }
        }
        private Task<IEnumerable<MixerDTO?>> SearchMixer(string value, CancellationToken token)
        {
            Func<MixerDTO?, bool> Criteria = x =>
            x!.Name.Contains(value, StringComparison.InvariantCultureIgnoreCase) 
            ;
            IEnumerable<MixerDTO?> FilteredItems = string.IsNullOrEmpty(value) ? Mixers.AsEnumerable() :
                 Mixers.Where(Criteria);
            return Task.FromResult(FilteredItems);
        }
        
    }
}
