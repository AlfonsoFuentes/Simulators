using Simulator.Shared.NuevaSimlationconQwen.Equipments;
using Simulator.Shared.NuevaSimlationconQwen.Equipments.Pumps;

namespace Simulator.Shared.NuevaSimlationconQwen.States.PlannedDownTimes
{
    public interface IPlannedDownTimeState
    {
        bool CheckStatus(DateTime currentdate);
    }
    public class CheckScheduledPlannedDownTimeState : IPlannedDownTimeState
    {
        IEquipment _Equipment = null!;
        public CheckScheduledPlannedDownTimeState(IEquipment Equipment)
        {
            _Equipment = Equipment;

        }
        public bool CheckStatus(DateTime currentdate)
        {
            var currentTime = currentdate.TimeOfDay; // ← TimeSpan que representa la hora actual del día

            var activeDownTime = _Equipment.PlannedDownTimes
                .FirstOrDefault(x => x.Start <= currentTime && currentTime < x.End);

            if (activeDownTime != null)
            {
                _Equipment.StartCriticalReport(_Equipment, "Planned downtime", $"Planned downtime scheduled from {activeDownTime.Start} to {activeDownTime.End}");
                // Cambiar al estado de parada activa, pasando el End como TimeSpan
                _Equipment.PlannedDownTimeState = new ScheduledPlannedDownTimeState(_Equipment, activeDownTime.End);
                return true;
            }
            return false;
        }
    }
    public class ScheduledPlannedDownTimeState : IPlannedDownTimeState
    {

        private TimeSpan EndDate;
        IEquipment _Equipment = null!;
        public ScheduledPlannedDownTimeState(IEquipment Equipment, TimeSpan _EndDate)
        {
            EndDate = _EndDate;
            _Equipment = Equipment;

        }



        public bool CheckStatus(DateTime currentdate)
        {
            var currentTime = currentdate.TimeOfDay; // ← TimeSpan que representa la hora actual del día
            if (currentTime >= EndDate)
            {
                _Equipment.EndCriticalReport();
                _Equipment.PlannedDownTimeState = new CheckScheduledPlannedDownTimeState(_Equipment);
                return true;
            }
            return false;
        }
    }
}
