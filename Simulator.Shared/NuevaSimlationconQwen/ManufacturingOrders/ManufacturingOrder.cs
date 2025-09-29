using Simulator.Shared.NuevaSimlationconQwen.Equipments.Lines;
using Simulator.Shared.NuevaSimlationconQwen.Equipments.Mixers;
using Simulator.Shared.NuevaSimlationconQwen.Equipments.Pumps;
using Simulator.Shared.NuevaSimlationconQwen.Equipments.Tanks;
using Simulator.Shared.NuevaSimlationconQwen.Materials;

namespace Simulator.Shared.NuevaSimlationconQwen.ManufacturingOrders
{
    public interface IVesselManufactureOrder
    {
        IMaterial Material { get; }
        ProcessLine Line { get; }
        List<IManufactureOrder> ManufactureOrdersFromMixers { get; }
        Amount MassPendingToProduce { get; }
        Amount MassPendingToDeliver { get; }
        IManufactureOrder LastInOrder { get; }
        Amount TotalMassProducingInMixer { get; }
        Amount TimeToEmptyMassInProcess { get; }
        Amount TotalMassStoragedOrProducing { get; }
        Amount AverageOutletFlow { get; }
        FromLineToWipProductionOrder LineCurrentProductionOrder { get; set; }
        FromLineToWipProductionOrder LineNextProductionOrder { get; }
        ProcessWipTankForLine WIP { get; set; }
        string LineName { get; }
        string MaterialName { get; }
        Amount MassDelivered { get; }
        Amount MassProduced { get; }

        void AddMassDelivered(Amount mass);
        void AddMassProduced(Amount mass);
        void AddMixerManufactureOrder(IManufactureOrder mixerManufactureOrder);
        void AddRunTime();
        void RemoveManufactureOrdersFromMixers(IManufactureOrder mixerManufactureOrder);
        void SendToLineCurrentOrderIsProduced();
        bool IsSendToLineCurrentOrderIsProduced { get; set; }
    }
    public class WIPInletMixerManufacturingOrder : IVesselManufactureOrder
    {
        public Amount PendingBatchTime => ManufactureOrdersFromMixers.Count == 0 ? new Amount(0, TimeUnits.Minute) :
            new Amount(_ManufactureOrdersFromMixers.Sum(x => x.PendingBatchTime.GetValue(TimeUnits.Minute)), TimeUnits.Minute);
        public List<IManufactureOrder> ManufactureOrdersFromMixers => _ManufactureOrdersFromMixers;
        List<IManufactureOrder> _ManufactureOrdersFromMixers = new List<IManufactureOrder>();
        public void AddMixerManufactureOrder(IManufactureOrder mixerManufactureOrder)
        {
            MassProduced += mixerManufactureOrder.BatchSize;
            _ManufactureOrdersFromMixers.Add(mixerManufactureOrder);
        }
        public void RemoveManufactureOrdersFromMixers(IManufactureOrder mixerManufactureOrder)
        {
            if (_ManufactureOrdersFromMixers.Contains(mixerManufactureOrder))
            {

                _ManufactureOrdersFromMixers.Remove(mixerManufactureOrder);
            }


        }

        public IManufactureOrder LastInOrder => _ManufactureOrdersFromMixers.Count == 0 ? null! : _ManufactureOrdersFromMixers.OrderBy(x => x.Order).Last();
        public Guid Id { get; } = Guid.NewGuid();
        public IMaterial Material => LineCurrentProductionOrder.Material;
        public ProcessLine Line => LineCurrentProductionOrder.Line;

      
        public ProcessWipTankForLine WIP { get; set; } = null!;
        public FromLineToWipProductionOrder LineCurrentProductionOrder { get; set; } = null!;
        // ✅ Constructor seguro
        public WIPInletMixerManufacturingOrder(ProcessWipTankForLine wip, FromLineToWipProductionOrder productionorder)
        {
            LineCurrentProductionOrder = productionorder;

            WIP = wip;

            LineNextProductionOrder = Line.InformNextProductionOrder;

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
        public Amount MassDelivered { get; private set; } = new Amount(0, MassUnits.KiloGram);
        public Amount MassProduced { get; private set; } = new Amount(0, MassUnits.KiloGram);
        public Amount MassToProduce => LineCurrentProductionOrder.TotalQuantityToProduce + WIP.LoLolevel * 1.1;
        public Amount MassToDeliver => LineCurrentProductionOrder.TotalQuantityToProduce;
        public Amount MassPendingToProduce => MassToProduce - MassProduced;
        public Amount MassPendingToDeliver => MassToDeliver - MassDelivered;
        public Amount RunTime { get; private set; } = new Amount(0, TimeUnits.Second);
        public Amount AverageOutletFlow => LineCurrentProductionOrder.AverageFlow;/* => RunTime.Value == 0 ? new Amount(0, MassFlowUnits.Kg_sg) :
            new Amount(MassDelivered.GetValue(MassUnits.KiloGram) / RunTime.GetValue(TimeUnits.Minute), MassFlowUnits.Kg_min);*/



        public Amount TimeToEmptyMassInProcess => AverageOutletFlow.Value == 0 ? new Amount(0, TimeUnits.Minute) :
           new Amount(TotalMassStoragedOrProducing.GetValue(MassUnits.KiloGram) / AverageOutletFlow.GetValue(MassFlowUnits.Kg_min), TimeUnits.Minute);

        Amount OneSecon { get; set; } = new Amount(1, TimeUnits.Second);

        public FromLineToWipProductionOrder LineNextProductionOrder { get; private set; } = null!;
        public bool IsSendToLineCurrentOrderIsProduced { get; set; } = false;

        public void AddRunTime()
        {
            RunTime += OneSecon;
        }
        public void AddMassDelivered(Amount mass)
        {
            MassDelivered += mass;
        }
        public void AddMassProduced(Amount mass)
        {
            MassProduced += mass;
        }
        public void SendToLineCurrentOrderIsProduced()
        {
            Line.ReceivedWIPCurrentOrderRealesed(LineCurrentProductionOrder);
            IsSendToLineCurrentOrderIsProduced = true;
        }

    }
    public class WIPInletSKIDManufacturingOrder : IVesselManufactureOrder
    {
        public Amount PendingBatchTime => ManufactureOrdersFromMixers.Count == 0 ? new Amount(0, TimeUnits.Minute) :
            new Amount(_ManufactureOrdersFromMixers.Sum(x => x.PendingBatchTime.GetValue(TimeUnits.Minute)), TimeUnits.Minute);
        public List<IManufactureOrder> ManufactureOrdersFromMixers => _ManufactureOrdersFromMixers;
        List<IManufactureOrder> _ManufactureOrdersFromMixers = new List<IManufactureOrder>();
        public void AddMixerManufactureOrder(IManufactureOrder mixerManufactureOrder)
        {
            MassProduced += mixerManufactureOrder.BatchSize;
            _ManufactureOrdersFromMixers.Add(mixerManufactureOrder);
        }
        public void RemoveManufactureOrdersFromMixers(IManufactureOrder mixerManufactureOrder)
        {
            if (_ManufactureOrdersFromMixers.Contains(mixerManufactureOrder))
            {

                _ManufactureOrdersFromMixers.Remove(mixerManufactureOrder);
            }


        }
        public FromLineToWipProductionOrder LineNextProductionOrder { get; private set; }
        public IManufactureOrder LastInOrder => _ManufactureOrdersFromMixers.Count == 0 ? null! : _ManufactureOrdersFromMixers.OrderBy(x => x.Order).Last();
        public Guid Id { get; } = Guid.NewGuid();
        public IMaterial Material => LineCurrentProductionOrder.Material;
        public ProcessLine Line => LineCurrentProductionOrder.Line;


        public ProcessWipTankForLine WIP { get; set; } = null!;
        public FromLineToWipProductionOrder LineCurrentProductionOrder { get; set; } = null!;
        // ✅ Constructor seguro
        public WIPInletSKIDManufacturingOrder(ProcessWipTankForLine wip, FromLineToWipProductionOrder productionorder)
        {
            LineCurrentProductionOrder = productionorder;

            WIP = wip;
            LineNextProductionOrder = Line.InformNextProductionOrder;


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
        public Amount MassDelivered { get; private set; } = new Amount(0, MassUnits.KiloGram);
        public Amount MassProduced { get; private set; } = new Amount(0, MassUnits.KiloGram);
        public Amount MassToProduce => LineCurrentProductionOrder.TotalQuantityToProduce + WIP.LoLolevel * 1.1;
        public Amount MassToDeliver => LineCurrentProductionOrder.TotalQuantityToProduce;
        public Amount MassPendingToProduce => MassToProduce - MassProduced;
        public Amount MassPendingToDeliver => MassToDeliver - MassDelivered;
        public Amount RunTime { get; private set; } = new Amount(0, TimeUnits.Second);
        public Amount AverageOutletFlow => LineCurrentProductionOrder.AverageFlow;/* RunTime.Value == 0 ? new Amount(0, MassFlowUnits.Kg_sg) :
            new Amount(MassDelivered.GetValue(MassUnits.KiloGram) / RunTime.GetValue(TimeUnits.Minute), MassFlowUnits.Kg_min);*/



        public Amount TimeToEmptyMassInProcess => AverageOutletFlow.Value == 0 ? new Amount(0, TimeUnits.Minute) :
           new Amount(MassPendingToProduce.GetValue(MassUnits.KiloGram) / AverageOutletFlow.GetValue(MassFlowUnits.Kg_min), TimeUnits.Minute);

        Amount OneSecon { get; set; } = new Amount(1, TimeUnits.Second);
        public void AddRunTime()
        {
            RunTime += OneSecon;
        }
        public void AddMassDelivered(Amount mass)
        {
            MassDelivered += mass;
        }
        public void AddMassProduced(Amount mass)
        {
            MassProduced += mass;
        }
        public bool IsSendToLineCurrentOrderIsProduced { get; set; } = false;
        public void SendToLineCurrentOrderIsProduced()
        {
            Line.ReceivedWIPCurrentOrderRealesed(LineCurrentProductionOrder);
            IsSendToLineCurrentOrderIsProduced = true;
        }

    }
    public class FromLineToWipProductionOrder
    {
        public ProductionSKURun ProductionSKURun { get; set; } = null!;
        public ProcessSKUByLine SKU { get; set; }

        public Guid Id { get; } = Guid.NewGuid();
        public IMaterial Material => SKU.Material;
        public Amount Size => SKU.Size;
        public ProcessLine Line { get; set; } = null!;
        public List<ProcessWipTankForLine> WIPs { get; set; } = new List<ProcessWipTankForLine>();
        public Amount TotalQuantityToProduce { get; set; } = new Amount(0, MassUnits.KiloGram);
        public Amount TimeToChangeSKU => SKU.TimeToChangeFormat;
        public Amount AverageFlow => ProductionSKURun.AverageMassFlow;
        public List<ManufaturingEquipment> PreferredManufacturer { get; set; } = new();
        // ✅ Constructor seguro
        public FromLineToWipProductionOrder(ProcessLine line, ProcessSKUByLine _SKU)
        {
            SKU = _SKU;

            Line = line;
            TotalQuantityToProduce = SKU.TotalPlannedMass;
            ProductionSKURun = new ProductionSKURun(SKU);
        }

        // ✅ Propiedad calculada: nombre del material (para reportes)
        public string MaterialName => Material.CommonName;

        // ✅ Propiedad calculada: nombre de la línea
        public string LineName => Line.Name;
        public void ReceiveWipCanHandleMaterial(ProcessWipTankForLine wip)
        {

            WIPs.Add(wip);
        }

    }

    public class FromWIPToMixerManufactureOrder
    {
        public Guid Id { get; } = Guid.NewGuid();
        public IMaterial Material { get; private set; } = null!;
        public ProcessWipTankForLine WIPTank { get; private set; } = null!;
        public IVesselManufactureOrder WIPOrder { get; private set; } = null!;

        public FromWIPToMixerManufactureOrder(IVesselManufactureOrder _WIPOrder, ProcessWipTankForLine wip)
        {
            Material = _WIPOrder.Material;
            WIPTank = wip;
            WIPOrder = _WIPOrder;
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
    public interface IManufactureOrder
    {
        IVesselManufactureOrder WIPOrder { get; }
        Amount BatchSize { get; set; }
        Amount PendingBatchTime { get; }
        int Order { get; set; }
        Amount CurrentBatchTime { get; set; }
        Queue<IRecipeStep> RecipeSteps { get; set; }
        IRecipeStep CurrentStep { get; set; }
        IMaterial Material { get; }
        Amount CurrentStarvedTime { get; set; }
        int TotalSteps { get; set; }
        ManufaturingEquipment Mixer { get; }
    }
    public class MixerManufactureOrder : IManufactureOrder
    {
        public Guid Id { get; } = Guid.NewGuid();
        public ManufaturingEquipment Mixer { get; private set; } = null!;
        public IVesselManufactureOrder WIPOrder { get; private set; } = null!;
        public int Order { get; set; }
        public IMaterial Material { get; private set; } = null!;
        public int TotalSteps { get; set; } = 0;
        public MixerManufactureOrder(ManufaturingEquipment Mixer, IVesselManufactureOrder WIPOrder)
        {
            this.Mixer = Mixer;
            this.WIPOrder = WIPOrder;
            this.Material = WIPOrder.Material;
            if (Mixer.EquipmentMaterials.Any(x => x.Material.Id == Material.Id))
            {

                var equipmentmaterial = Mixer.EquipmentMaterials.First(x => x.Material.Id == Material.Id);
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
        public Amount RealBatchTime => BatchTime + CurrentStarvedTime;
        public Amount CurrentStarvedTime { get; set; } = new Amount(0, TimeUnits.Minute);
        public Amount CurrentBatchTime { get; set; } = new Amount(0, TimeUnits.Minute);
        public Amount TheoricalPendingBatchTime => BatchTime - CurrentBatchTime;
        public Amount CurrentMixerLevel => Mixer.CurrentLevel;
        public bool IsBatchFinished { get; set; } = false;
        public bool IsBatchStarved { get; set; } = false;

        public IRecipeStep CurrentStep { get; set; } = null!;
        public Queue<IRecipeStep> RecipeSteps { get; set; } = new Queue<IRecipeStep>();
        public IManufactureFeeder SelectedFeder { get; set; } = null!;
        public Amount PendingBatchTime => RealBatchTime - CurrentBatchTime;

    }
    public class SKIDManufactureOrder : IManufactureOrder
    {
        public Guid Id { get; } = Guid.NewGuid();
        public ManufaturingEquipment Mixer { get; private set; } = null!;
        public IVesselManufactureOrder WIPOrder { get; private set; } = null!;
        public int Order { get; set; }
        public IMaterial Material { get; private set; } = null!;
        public int TotalSteps { get; set; } = 0;
        public SKIDManufactureOrder(ManufaturingEquipment Mixer, IVesselManufactureOrder WIPOrder)
        {
            this.Mixer = Mixer;
            this.WIPOrder = WIPOrder;
            Material = WIPOrder.Material;


        }

        public Amount BatchSize { get; set; } = new(0, MassUnits.KiloGram);
        public Amount BatchTime { get; set; } = new(0, MassUnits.KiloGram);
        public Amount RealBatchTime => BatchTime + CurrentStarvedTime;
        public Amount CurrentStarvedTime { get; set; } = new Amount(0, TimeUnits.Minute);
        public Amount CurrentBatchTime { get; set; } = new Amount(0, TimeUnits.Minute);
        public Amount TheoricalPendingBatchTime => BatchTime - CurrentBatchTime;
        public Amount CurrentMixerLevel => Mixer.CurrentLevel;
        public bool IsBatchFinished { get; set; } = false;
        public bool IsBatchStarved { get; set; } = false;

        public IRecipeStep CurrentStep { get; set; } = null!;
        public Queue<IRecipeStep> RecipeSteps { get; set; } = new Queue<IRecipeStep>();
        public IManufactureFeeder SelectedFeder { get; set; } = null!;
        public Amount PendingBatchTime => RealBatchTime - CurrentBatchTime;

    }

}
