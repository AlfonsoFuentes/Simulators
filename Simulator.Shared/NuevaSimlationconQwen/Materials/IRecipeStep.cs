using Simulator.Shared.Enums.HCEnums.Enums;

namespace Simulator.Shared.NuevaSimlationconQwen.Materials
{
    public interface IStepTimeCalculator
    {
        Amount CalculateTime(IRecipeStep step, Amount batchSize);
    }
    public class AddStepTimeCalculator : IStepTimeCalculator
    {
        public Amount CalculateTime(IRecipeStep step, Amount batchSize)
        {
            var massValue = step.Percentage > 0
                ? batchSize.GetValue(MassUnits.KiloGram) * step.Percentage / 100
                : 0;

            step.Mass = new Amount(massValue, MassUnits.KiloGram);
            if (step.Flow == null)
            {
                return new Amount(0, TimeUnits.Minute);
            }
            var flowValue = step.Flow.GetValue(MassFlowUnits.Kg_min);

            if (flowValue == 0)
                return new Amount(0, TimeUnits.Minute);

            var timeValue = massValue / flowValue;
            return new Amount(timeValue, TimeUnits.Minute);
        }
    }
    public class FixedTimeStepCalculator : IStepTimeCalculator
    {
        public Amount CalculateTime(IRecipeStep step, Amount batchSize)
        {
            // Para todos los pasos que no son Add, el tiempo es fijo (ya definido en step.Time)
            return step.Time;
        }
    }
    public static class StepTimeCalculatorFactory
    {
        private static readonly IStepTimeCalculator _addCalculator = new AddStepTimeCalculator();
        private static readonly IStepTimeCalculator _fixedTimeCalculator = new FixedTimeStepCalculator();

        public static IStepTimeCalculator GetCalculator(BackBoneStepType stepType)
        {
            return stepType switch
            {
                BackBoneStepType.Add => _addCalculator,
                _ => _fixedTimeCalculator // Todos los demás usan tiempo fijo
            };
        }
    }
    public interface IRecipeStep
    {
        Guid Id { get; set; }
        int StepNumber { get; set; }
        Guid? RawMaterialId { get; set; }
        string RawMaterialName { get; set; }
        Amount Time { get; set; }
        Amount Flow { get; set; }
        Amount Mass { get; set; }
        double Percentage { get; set; }
        void CalculateTime(Amount batchSize);
        BackBoneStepType BackBoneStepType { get; set; }
        string StepDescription { get; }
    }
    public class RecipeStep : IRecipeStep
    {
        public override string ToString()
        {
            return StepDescription;
        }
        public string StepDescription => $"Step {StepNumber}: {BackBoneStepType}, Time: {Time}, Flow: {Flow}, Mass: {Mass}, Percentage: {Percentage}%";
        public BackBoneStepType BackBoneStepType { get; set; }
        public Guid Id { get; set; }
        public int StepNumber { get; set; }
        public Guid? RawMaterialId { get; set; }
        public Amount Time { get; set; } = new(0, TimeUnits.Minute);
        public Amount Flow { get; set; } = new(0, MassFlowUnits.Kg_min);
        public Amount Mass { get; set; } = new(0, MassUnits.KiloGram);
        public double Percentage { get; set; }
        public string RawMaterialName { get; set; } = string.Empty;
        public void CalculateTime(Amount batchSize)
        {
            try
            {
                var calculator = StepTimeCalculatorFactory.GetCalculator(BackBoneStepType);
                Time = calculator.CalculateTime(this, batchSize);
            }
            catch (Exception ex)
            {
                // Manejo de errores, si es necesario
                throw new InvalidOperationException($"Error calculating time for step {StepNumber}: {ex.Message}", ex);
            }

        }
    }
}
