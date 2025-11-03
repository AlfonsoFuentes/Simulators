using Simulator.Client.HCPages.SimulationPlanneds;
using Simulator.Shared.Models.HCs.SimulationPlanneds;
using Simulator.Shared.NuevaSimlationconQwen;

namespace Simulator.Client.HCPages.MainProcesses.Plans
{
    public partial class SelectedPlannedCard
    {
        [Parameter]
        public SimulationPlannedDTO Model { get; set; } = new();
        [Parameter]
        public EventCallback GetAll { get; set; }
        async Task Edit()
        {


            var parameters = new DialogParameters<SimulationPlannedDialog>
        {

             { x => x.Model, Model },
        };
            var options = new DialogOptions() { MaxWidth = MaxWidth.Large };


            var dialog = await DialogService.ShowAsync<SimulationPlannedDialog>("SimulationPlanned", parameters, options);
            var result = await dialog.Result;
            if (result != null && !result.Canceled)
            {
                await GetAll.InvokeAsync();
            }

        }
        public async Task Delete()
        {
            var parameters = new DialogParameters<DialogTemplate>
        {
            { x => x.ContentText, $"Do you really want to delete {Model.Name}? This process cannot be undone." },
            { x => x.ButtonText, "Delete" },
            { x => x.Color, Color.Error }
        };

            var options = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall };

            var dialog = await DialogService.ShowAsync<DialogTemplate>("Delete", parameters, options);
            var result = await dialog.Result;


            if (!result!.Canceled)
            {
                SimulationPlannedDTO request = new()
                {
                    Id = Model.Id,
                    Name = Model.Name,

                };
                var resultDelete = await ClientService.Delete(request);
                if (resultDelete.Succeeded)
                {
                    await GetAll.InvokeAsync();
               


                }
        
            }

        }
        async Task LoadPlann()
        {
            CompletedSimulationPlannedDTO model = new CompletedSimulationPlannedDTO()
            {
                MainProcessId = Model.MainProcessId,
                Id = Model.Id
            };
            var result = await ClientService.GetById(model);
            if (result.Succeeded)
            {

                Model.PlannedLines = result.Data.PlannedLines;
                Model.PlannedMixers = result.Data.PlannedMixers;
                if (PlannedChanged != null)
                    await PlannedChanged.Invoke(Model);
            }

        }
        [Parameter]
        public Func<SimulationPlannedDTO, Task> PlannedChanged { get; set; } = null!;
    }
}
