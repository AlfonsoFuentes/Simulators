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
        public List<MainProcessDTO> Items { get; set; } = new();
        string nameFilter = string.Empty;
        public Func<MainProcessDTO, bool> Criteria => x => x.Name.Contains(nameFilter, StringComparison.InvariantCultureIgnoreCase);
        public List<MainProcessDTO> FilteredItems => string.IsNullOrEmpty(nameFilter) ? Items :
            Items.Where(Criteria).ToList();
        protected override async Task OnInitializedAsync()
        {
            await GetAll();
        }
        async Task GetAll()
        {
            var result = await GenericService.GetAll<MainProcessResponseList, MainProcessGetAll>(new MainProcessGetAll());
            if (result.Succeeded)
            {
                Items = result.Data.Items;
            }
        }
        public async Task AddNew()
        {

            var parameters = new DialogParameters<MainProcessDialog>
            {

            };

            var options = new DialogOptions() { MaxWidth = MaxWidth.Medium };

            var dialog = await DialogService.ShowAsync<MainProcessDialog>("MainProcess", parameters, options);
            var result = await dialog.Result;
            if (result != null && !result.Canceled)
            {
                await GetAll();
                StateHasChanged();
            }
        }
        async Task Edit(MainProcessDTO response)
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
        public async Task Delete(MainProcessDTO response)
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
                DeleteMainProcessRequest request = new()
                {
                    Id = response.Id,
                    Name = response.Name,

                };
                var resultDelete = await GenericService.Post(request);
                if (resultDelete.Succeeded)
                {
                    await GetAll();
                    _snackBar.ShowSuccess(resultDelete.Messages);


                }
                else
                {
                    _snackBar.ShowError(resultDelete.Messages);
                }
            }

        }
        HashSet<MainProcessDTO> SelecteItems = null!;
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
                DeleteGroupMainProcessRequest request = new()
                {
                    SelecteItems = SelecteItems,

                };
                var resultDelete = await GenericService.Post(request);
                if (resultDelete.Succeeded)
                {
                    await GetAll();
                    _snackBar.ShowSuccess(resultDelete.Messages);
                    SelecteItems = null!;

                }
                else
                {
                    _snackBar.ShowError(resultDelete.Messages);
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
            var result = await GenericService.GetById<NewSimulationDTO, GetProcessByIdRequest>(new GetProcessByIdRequest()
            {
                MainProcessId = MainProcessId,
                FocusFactory= FocusFactory,



            });
            if (result.Succeeded)
            {

                _snackBar.ShowSuccess("Process flow digram was loaded succesfully");
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
            else
            {
                _snackBar.ShowError(result.Messages);
            }
        }

        public List<SimulationPlannedDTO> PlannedItems { get; set; } = new();
        SimulationPlannedResponseList SimulationPlannedResponseList = null!;
        async Task GetAllPlanneds(Guid _MainProcessId)
        {
            var result = await GenericService.GetAll<SimulationPlannedResponseList, SimulationPlannedGetAll>(new SimulationPlannedGetAll()
            {
                MainProcessId = MainProcessId,


            });
            if (result.Succeeded)
            {
                SimulationPlannedResponseList = result.Data;
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
