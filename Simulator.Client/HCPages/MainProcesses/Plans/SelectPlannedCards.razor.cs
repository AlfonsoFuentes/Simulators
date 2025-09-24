using Simulator.Client.HCPages.SimulationPlanneds;
using Simulator.Shared.Models.HCs.SimulationPlanneds;
using Simulator.Shared.NuevaSimlationconQwen;

namespace Simulator.Client.HCPages.MainProcesses.Plans
{
    public partial class SelectPlannedCards
    {
        [Parameter]
        public List<SimulationPlannedDTO> Items { get; set; } = new();
        string nameFilter = string.Empty;
        public Func<SimulationPlannedDTO, bool> Criteria => x => x.Name.Contains(nameFilter, StringComparison.InvariantCultureIgnoreCase);
        public List<SimulationPlannedDTO> FilteredItems => string.IsNullOrEmpty(nameFilter) ? Items :
            Items.Where(Criteria).ToList();

        [Parameter]
        public Guid MainProcessId {  get; set; }
        [Parameter]
        public Func<SimulationPlannedDTO, Task> PlannedChanged { get; set; } = null!;

        [Parameter]
        public EventCallback GetAll { get; set; }
      

        public async Task AddNew()
        {
            SimulationPlannedDTO response = new() { MainProcessId = MainProcessId };

            var parameters = new DialogParameters<SimulationPlannedDialog>
        {
           { x => x.Model, response },
        };

            var options = new DialogOptions() { MaxWidth = MaxWidth.Large };

            var dialog = await DialogService.ShowAsync<SimulationPlannedDialog>("SimulationPlanned", parameters, options);
            var result = await dialog.Result;
            if (result != null && !result.Canceled)
            {
                await GetAll.InvokeAsync();
           
            }

        }
      
       
    
}
}
