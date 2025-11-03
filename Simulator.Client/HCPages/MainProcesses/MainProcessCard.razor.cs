using Simulator.Shared.Models.HCs.MainProcesss;

namespace Simulator.Client.HCPages.MainProcesses
{
    public partial class MainProcessCard
    {
        [Parameter]
        public ProcessFlowDiagramDTO Model { get; set; } = null!;
        public char FirstLetter => Model.Name.First();
        [Parameter]
        public EventCallback GetAll { get; set; }
        [Parameter]
        public EventCallback<Guid> SelectProcess { get; set; }
        public async Task OnSelectProcess()
        {
            await SelectProcess.InvokeAsync(Model.Id);
        }
        async Task Edit()
        {


            var parameters = new DialogParameters<MainProcessDialog>
            {

                 { x => x.Model, Model },
            };
            var options = new DialogOptions() { MaxWidth = MaxWidth.Small };


            var dialog = await DialogService.ShowAsync<MainProcessDialog>("MainProcess", parameters, options);
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
               
                var resultDelete = await ClientService.Delete(Model);
                if (resultDelete.Succeeded)
                {
                    await GetAll.InvokeAsync();
     


                }
               
            }

        }
    }
}
