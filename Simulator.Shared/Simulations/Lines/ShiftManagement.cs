using Simulator.Shared.Enums.HCEnums.Enums;

namespace Simulator.Shared.Simulations.Lines
{
    public class ShiftManagement
    {
        BaseLine Line = null!;
        public ShiftManagement(BaseLine line)
        {
            Line = line;
        }
        DateTime CurrenDate { get; set; }
        public void SetTime(DateTime currentdate)
        {
            CurrenDate = currentdate;
            CurrentShift = CheckShift();
        }
        public CurrentShift CurrentShift { get;private set; }
        CurrentShift CheckShift() =>
            CurrenDate.Hour switch
            {
                >= 6 and < 14 => CheckShift1(),
                >= 14 and < 22 => CheckShift2(),
                _ => CheckShift3()
            };
        CurrentShift CheckShift1()
        {
            if (CurrentShift != CurrentShift.Shift_1) Line.InitDataForNextShift();
            return CurrentShift.Shift_1;
        }
        CurrentShift CheckShift2()
        {
            if (CurrentShift != CurrentShift.Shift_2) Line.InitDataForNextShift();
            return CurrentShift.Shift_2;
        }
        CurrentShift CheckShift3()
        {
            if (CurrentShift != CurrentShift.Shift_3) Line.InitDataForNextShift();
            return CurrentShift.Shift_3;
        }
       
        public void SetShiftType(ShiftType type) => ShiftType = type;
        public ShiftType ShiftType { get; private set; }
        public bool CheckPlannedShift(CurrentShift currentShift) => currentShift switch
        {

            CurrentShift.Shift_1 => ShiftType switch
            {
                ShiftType.Shift_1_2_3 => true,
                ShiftType.Shift_1_2 => true,
                ShiftType.Shift_1_3 => true,
                ShiftType.Shift_1 => true,
                _ => false,
            },
            CurrentShift.Shift_2 => ShiftType switch
            {
                ShiftType.Shift_1_2_3 => true,
                ShiftType.Shift_1_2 => true,
                ShiftType.Shift_2_3 => true,
                ShiftType.Shift_2 => true,
                _ => false,
            },
            CurrentShift.Shift_3 => ShiftType switch
            {
                ShiftType.Shift_1_2_3 => true,
                ShiftType.Shift_2_3 => true,
                ShiftType.Shift_1_3 => true,
                ShiftType.Shift_3 => true,
                _ => false
            },

            _ => false
        };
        public CurrentShift NextShift => CurrentShift switch
        {
            CurrentShift.Shift_1 => CurrentShift.Shift_2,
            CurrentShift.Shift_2 => CurrentShift.Shift_3,
            CurrentShift.Shift_3 => CurrentShift.Shift_1,
            _ => CurrentShift.Shift_1,
        };
        public bool IsPlannedShift => CheckPlannedShift(CurrentShift);
        public bool IsNextShiftPlanned => CheckPlannedShift(NextShift);

        public bool IsLinePlanned => ShiftType != ShiftType.None;

    }

}
