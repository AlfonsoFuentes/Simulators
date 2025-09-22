using Simulator.Shared.NuevaSimlationconQwen.Equipments.Lines;
using Simulator.Shared.NuevaSimlationconQwen.Equipments.Mixers;
using Simulator.Shared.NuevaSimlationconQwen.Equipments.Pumps;
using Simulator.Shared.NuevaSimlationconQwen.Equipments.Tanks;
using Simulator.Shared.NuevaSimlationconQwen.Materials;

namespace Simulator.Shared.NuevaSimlationconQwen.ManufacturingOrders
{
    public class WIPManufacturingOrder
    {
        public List<MixerManufactureOrder> ManufactureOrdersFromMixers => _ManufactureOrdersFromMixers;
        List<MixerManufactureOrder> _ManufactureOrdersFromMixers = new List<MixerManufactureOrder>();
        public void AddMixerManufactureOrder(MixerManufactureOrder mixerManufactureOrder)
        {
            _ManufactureOrdersFromMixers.Add(mixerManufactureOrder);
        }
        public void RemoveManufactureOrdersFromMixers(MixerManufactureOrder mixerManufactureOrder)
        {
            if (_ManufactureOrdersFromMixers.Contains(mixerManufactureOrder))
            {
                _ManufactureOrdersFromMixers.Remove(mixerManufactureOrder);
            }
            else
            {

            }

        }
        public MixerManufactureOrder LastInOrder => _ManufactureOrdersFromMixers.Count == 0 ? null! : _ManufactureOrdersFromMixers.OrderBy(x => x.Order).Last();
        public Guid Id { get; } = Guid.NewGuid();
        public IMaterial Material { get; set; } = null!;  // ← Renombrado: más claro que MaterialId
        public ProcessLine Line { get; set; } = null!;


        public ProcessWipTankForLine WIP { get; set; } = null!;

        // ✅ Constructor seguro
        public WIPManufacturingOrder(ProcessWipTankForLine wip, ProcessLine line, IMaterial material, Amount totalQuantity)
        {
            Material = material ?? throw new ArgumentNullException(nameof(material));
            Line = line ?? throw new ArgumentNullException(nameof(line));
            WIP = wip;
            MassToDeliver = totalQuantity;
        }

        // ✅ Propiedad calculada: nombre del material (para reportes)
        public string MaterialName => Material.CommonName;

        // ✅ Propiedad calculada: nombre de la línea
        public string LineName => Line.Name;
        public Amount TotalMassProducingInMixer => new Amount(
               _ManufactureOrdersFromMixers.Sum(x => x.BatchSize.GetValue(MassUnits.KiloGram)),
               MassUnits.KiloGram
           );
        public Amount TotalMassStoragedOrProducing => WIP.CurrentLevel + TotalMassProducingInMixer;
        public Amount MassToDeliver { get; set; } = new Amount(0, MassUnits.KiloGram);
        public Amount MassDelivered { get; set; } = new Amount(0, MassUnits.KiloGram);
        public Amount MassPendingToProduce => MassToDeliver - MassDelivered - TotalMassStoragedOrProducing;
        public Amount RunTime { get; private set; } = new Amount(0, TimeUnits.Second);
        public Amount AverageOutletFlow => RunTime.Value == 0 ? new Amount(0, MassFlowUnits.Kg_sg) :
            new Amount(MassDelivered.GetValue(MassUnits.KiloGram) / RunTime.GetValue(TimeUnits.Minute), MassFlowUnits.Kg_min);
        public Amount TimeToFinishRun => AverageOutletFlow.Value == 0 ? new Amount(0, TimeUnits.Minute) :
           new Amount(MassPendingToProduce.GetValue(MassUnits.KiloGram) / AverageOutletFlow.GetValue(MassFlowUnits.Kg_min), TimeUnits.Minute);
        public Amount TimeToEmptyMassInProcess => AverageOutletFlow.Value == 0 ? new Amount(0, TimeUnits.Minute) :
           new Amount(TotalMassStoragedOrProducing.GetValue(MassUnits.KiloGram) / AverageOutletFlow.GetValue(MassFlowUnits.Kg_min), TimeUnits.Minute);
        Amount OneSecon { get; set; } = new Amount(1, TimeUnits.Second);
        public void AddRunTime()
        {
            RunTime += OneSecon;
        }
    }

    public class FromLineToWipProductionOrder
    {
        public Guid Id { get; } = Guid.NewGuid();
        public IMaterial Material { get; set; } = null!;  // Material que se esta produciendo
        public ProcessLine Line { get; set; } = null!;
        public List<ProcessWipTankForLine> WIPs { get; set; } = new List<ProcessWipTankForLine>();
        public Amount TotalQuantityToProduce { get; set; } = new Amount(0, MassUnits.KiloGram);

        public List<ManufaturingEquipment> PreferredManufacturer { get; set; } = new();
        // ✅ Constructor seguro
        public FromLineToWipProductionOrder(ProcessLine line, IMaterial material, Amount totalQuantity)
        {
            Material = material ?? throw new ArgumentNullException(nameof(material));
            Line = line ?? throw new ArgumentNullException(nameof(line));
            TotalQuantityToProduce = totalQuantity;
        }

        // ✅ Propiedad calculada: nombre del material (para reportes)
        public string MaterialName => Material.CommonName;

        // ✅ Propiedad calculada: nombre de la línea
        public string LineName => Line.Name;


    }

    public class FromWIPToMixerManufactureOrder
    {
        public Guid Id { get; } = Guid.NewGuid();
        public IRecipedMaterial Material { get; private set; } = null!;
        public ProcessWipTankForLine WIPTank { get; private set; } = null!;


        public FromWIPToMixerManufactureOrder(IRecipedMaterial material, ProcessWipTankForLine wip)
        {
            Material = material ?? throw new ArgumentNullException(nameof(material));
            WIPTank = wip ?? throw new ArgumentNullException(nameof(wip));
        }

    }
    public class TransferFromMixertoWIPOrder
    {
        public TransferFromMixertoWIPOrder(ProcessMixer SourceMixer, ProcessWipTankForLine DestinationWip, Amount TotalQuantityToTransfer, Amount TransferFlow)
        {
            this.SourceMixer = SourceMixer;
            this.TotalQuantityToTransfer = TotalQuantityToTransfer;
            this.TransferFlow = TransferFlow;
            this.DestinationWip = DestinationWip;
        }

        public Guid Id { get; } = Guid.NewGuid();
        public ProcessMixer SourceMixer { get; set; } = null!;
        public ProcessWipTankForLine DestinationWip { get; set; } = null!;
        public Amount TotalQuantityToTransfer { get; set; } = new Amount(0, MassUnits.KiloGram);
        public Amount MassReceived { get; set; } = new Amount(0, MassUnits.KiloGram);
        public Amount PendingToReceive => TotalQuantityToTransfer - MassReceived;

        public Amount TransferFlow { get; set; } = new Amount(0, MassFlowUnits.Kg_min);

    }
    public class MixerManufactureOrder
    {
        public Guid Id { get; } = Guid.NewGuid();
        public ProcessMixer Mixer { get; private set; } = null!;
        public ProcessWipTankForLine Wip { get; set; } = null!;
        public int Order { get; set; }
        public IMaterial Material { get; private set; } = null!;
        public int TotalSteps { get; set; } = 0;
        public MixerManufactureOrder(ProcessMixer Mixer, ProcessWipTankForLine Wip, IMaterial material)
        {
            this.Mixer = Mixer;
            this.Wip = Wip;

            if (Mixer.EquipmentMaterials.Any(x => x.Material.Id == material.Id))
            {
                Material = material;
                var equipmentmaterial = Mixer.EquipmentMaterials.First(x => x.Material.Id == material.Id);
                var recipe = (RecipedMaterial)equipmentmaterial.Material;
                if (recipe != null)
                {
                    TotalSteps = recipe.RecipeSteps.Count;
                    BatchSize = equipmentmaterial.BatchSize;
                    BatchTime = equipmentmaterial.BatchCycleTime;
                    foreach (var item in recipe.RecipeSteps.OrderBy(x => x.StepNumber))
                    {

                        RecipeSteps.Enqueue(item);
                    }
                }

            }

        }

        public Amount BatchSize { get; set; } = new(0, MassUnits.KiloGram);
        public Amount BatchTime { get; set; } = new(0, MassUnits.KiloGram);

        public Amount CurrentStarvedTime { get; set; } = new Amount(0, TimeUnits.Minute);
        public Amount CurrentBatchTime { get; set; } = new Amount(0, TimeUnits.Minute);
        public Amount TheoricalPendingBatchTime => BatchTime - CurrentBatchTime;
        public Amount CurrentMixerLevel => Mixer.CurrentLevel;
        public bool IsBatchFinished { get; set; } = false;
        public bool IsBatchStarved { get; set; } = false;

        public IRecipeStep CurrentStep { get; set; } = null!;
        public Queue<IRecipeStep> RecipeSteps { get; set; } = new Queue<IRecipeStep>();
        public IManufactureFeeder SelectedFeder { get; set; } = null!;
    }


}
