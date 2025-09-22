using Simulator.Shared.NuevaSimlationconQwen.Equipments;

namespace Simulator.Shared.NuevaSimlationconQwen.Materials
{
    public interface IEquipmentMaterial
    {
        IMaterial Material { get; set; }

        Amount TotalTime { get; }
        Amount TransferTime { get; set; }
        Amount BatchCycleTime { get; set; }
        Amount BatchSize { get; }
        Amount SkidFlow { get; }

        void CalculateFlows(Amount SkidFlow);
        void CalculateTime(Amount batchSize);
    }
    public class EquipmentMaterial : IEquipmentMaterial
    {
        public Amount SkidFlow { get; private set; } = new Amount(MassFlowUnits.Kg_min);
        public Amount BatchSize { get; private set; } = new Amount(MassUnits.KiloGram);
        public Amount TotalTime => TransferTime + BatchCycleTime;
        public Amount TransferTime { get; set; } = new Amount(TimeUnits.Minute);
        public Amount BatchCycleTime { get; set; } = new Amount(TimeUnits.Minute);
        public IMaterial Material { get; set; } = null!;
        public void CalculateTime(Amount batchSize)
        {
            BatchSize = batchSize;
            if (Material != null && Material is IRecipedMaterial RecipedMaterial)
            {
                foreach (var step in RecipedMaterial.RecipeSteps)
                {
                    step.CalculateTime(BatchSize);

                }
                var time = RecipedMaterial.RecipeSteps.Sum(x => x.Time.GetValue(TimeUnits.Minute));
                BatchCycleTime.SetValue(time, TimeUnits.Minute);
            }

        }
        public void CalculateFlows(Amount _SkidFlow)
        {
            SkidFlow = _SkidFlow;
            if (Material != null && Material is IRecipedMaterial RecipedMaterial)
            {
                foreach (var step in RecipedMaterial.RecipeSteps)
                {
                    step.Flow = SkidFlow * step.Percentage / 100;

                }
            }
        }

    }

    public interface IRecipedMaterial : IMaterial
    {

        public Queue<IRecipeStep> RecipeSteps { get; set; }

    }
    public class RecipedMaterial : Material, IRecipedMaterial
    {

        public Queue<IRecipeStep> RecipeSteps { get; set; } = new();

    }
}
