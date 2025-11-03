using Simulator.Shared.Enums.HCEnums.Enums;
using Simulator.Shared.Models.HCs.MainProcesss;
using Simulator.Shared.Models.HCs.SimulationPlanneds;
using Simulator.Shared.NuevaSimlationconQwen;
using Simulator.Shared.Simulations;
using static Simulator.Shared.StaticClasses.StaticClass;

namespace Simulator.Client.HCPages.MainProcesses
{
    public partial class MainProcessCards
    {
        public List<ProcessFlowDiagramDTO> Items { get; set; } = new();
        string nameFilter = string.Empty;
        public Func<ProcessFlowDiagramDTO, bool> Criteria => x => x.Name.Contains(nameFilter, StringComparison.InvariantCultureIgnoreCase);
        public List<ProcessFlowDiagramDTO> FilteredItems => string.IsNullOrEmpty(nameFilter) ? Items :
            Items.Where(Criteria).ToList();
        protected override async Task OnInitializedAsync()
        {
            await GetAll();
        }
        async Task GetAll()
        {
            var result = await ClientService.GetAll(new ProcessFlowDiagramDTO());
            if (result.Succeeded)
            {
                Items = result.Data;
            }
        }
        public async Task AddNew()
        {

            var parameters = new DialogParameters<MainProcessDialog>
            {

            };

            var options = new DialogOptions() { MaxWidth = MaxWidth.Small };

            var dialog = await DialogService.ShowAsync<MainProcessDialog>("MainProcess", parameters, options);
            var result = await dialog.Result;
            if (result != null && !result.Canceled)
            {
                await GetAll();
                StateHasChanged();
            }
        }
        async Task Edit(ProcessFlowDiagramDTO response)
        {


            var parameters = new DialogParameters<MainProcessDialog>
        {

             { x => x.Model, response },
        };
            var options = new DialogOptions() { MaxWidth = MaxWidth.Medium };


            var dialog = await DialogService.ShowAsync<MainProcessDialog>("MainProcess", parameters, options);
            var result = await dialog.Result;
            if (result != null && !result.Canceled)
            {
                await GetAll();
            }
        }
        public async Task Delete(ProcessFlowDiagramDTO response)
        {
            var parameters = new DialogParameters<DialogTemplate>
        {
            { x => x.ContentText, $"Do you really want to delete {response.Name}? This process cannot be undone." },
            { x => x.ButtonText, "Delete" },
            { x => x.Color, Color.Error }
        };

            var options = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall };

            var dialog = await DialogService.ShowAsync<DialogTemplate>("Delete", parameters, options);
            var result = await dialog.Result;


            if (!result!.Canceled)
            {
               
                var resultDelete = await ClientService.Delete(response);
                if (resultDelete.Succeeded)
                {
                    await GetAll();
                   


                }
                
            }

        }
        HashSet<ProcessFlowDiagramDTO> SelecteItems = null!;
        public async Task DeleteGroup()
        {
            if (SelecteItems == null) return;
            var parameters = new DialogParameters<DialogTemplate>
        {
            { x => x.ContentText, $"Do you really want to delete this {SelecteItems.Count} Items? This process cannot be undone." },
            { x => x.ButtonText, "Delete" },
            { x => x.Color, Color.Error }
        };

            var options = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall };

            var dialog = await DialogService.ShowAsync<DialogTemplate>("Delete", parameters, options);
            var result = await dialog.Result;


            if (!result!.Canceled)
            {
                
                var resultDelete = await ClientService.DeleteGroup(SelecteItems.ToList());
                if (resultDelete.Succeeded)
                {
                    await GetAll();
                 
                    SelecteItems = null!;

                }
               
            }

        }
        public NewSimulationDTO SimulationDTO { get; set; } = null!;
        public GeneralSimulation Simulation { get; set; } = null!;
        Guid MainProcessId { get; set; }
        public async Task SelectProcessAndPlannedByID2(Guid _MainProcessId)
        {
            MainProcessId = _MainProcessId;
            await GetAllPlanneds(_MainProcessId);
            await SelectProcess(_MainProcessId);
        }
        FocusFactory FocusFactory { get; set; } = FocusFactory.None;
        public async Task SelectProcessAndPlannedByID(Guid _MainProcessId)
        {
            MainProcessId = _MainProcessId;
            var selectitem = Items.FirstOrDefault(x => x.Id == _MainProcessId);
            if (selectitem != null) FocusFactory = selectitem.FocusFactory;
            var task1 = GetAllPlanneds(MainProcessId);
            var task2 = SelectProcess(MainProcessId);

            // Esperar a que ambas terminen
            await Task.WhenAll(task1, task2);

        }
        public async Task SelectAllProcess()
        {

            await SelectProcess(MainProcessId);

        }
        public async Task SelectAllPlan()
        {

            await GetAllPlanneds(MainProcessId);

        }
        bool SimulationLoading { get; set; }
        public async Task SelectProcess(Guid _MainProcessId)
        {
            SimulationLoading = true;
            Simulation = null!;
            var result = await ClientService.GetById(new NewSimulationDTO()
            {
                 Id = MainProcessId,
              
                FocusFactory= FocusFactory,



            });
            if (result.Succeeded)
            {

              
                SimulationDTO = result.Data;

                if (SimulationDTO != null)
                {
                    Simulation = new GeneralSimulation();
                    Simulation.ReadSimulationDataFromDTO(SimulationDTO);

                    if (SelectedPlanned != null)
                        Simulation.SetPlanned(SelectedPlanned);
                    SimulationLoading = false;
                    StateHasChanged();
              
                }




            }
           
        }

        public List<SimulationPlannedDTO> PlannedItems { get; set; } = new();
     
        async Task GetAllPlanneds(Guid _MainProcessId)
        {
            var result = await ClientService.GetAll(new SimulationPlannedDTO()
            {
                MainProcessId = MainProcessId,


            });
            if (result.Succeeded)
            {
                PlannedItems = result.Data;
                StateHasChanged();
            }
        }
        SimulationPlannedDTO SelectedPlanned { get; set; } = null!;
        async Task SelectPlan(SimulationPlannedDTO planned)
        {
            await Task.Delay(1);
            SelectedPlanned = planned;
            if (!SimulationLoading && Simulation != null)
                Simulation.SetPlanned(planned);
            StateHasChanged();
        }
    }
}
