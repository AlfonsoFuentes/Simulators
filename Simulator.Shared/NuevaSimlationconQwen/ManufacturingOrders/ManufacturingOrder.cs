using Simulator.Shared.NuevaSimlationconQwen.Equipments.Lines;
using Simulator.Shared.NuevaSimlationconQwen.Equipments.Mixers;
using Simulator.Shared.NuevaSimlationconQwen.Equipments.Pumps;
using Simulator.Shared.NuevaSimlationconQwen.Equipments.Tanks;
using Simulator.Shared.NuevaSimlationconQwen.Equipments.Tanks.States;
using Simulator.Shared.NuevaSimlationconQwen.Materials;
using Simulator.Shared.NuevaSimlationconQwen.Reports;
using static Simulator.Shared.StaticClasses.StaticClass;

namespace Simulator.Shared.NuevaSimlationconQwen.ManufacturingOrders
{
    public interface ITankManufactureOrder
    {
        Amount TotalMassProducingInMixer { get; }
        IManufactureOrder LastInOrder { get; }
        List<IManufactureOrder> ManufactureOrdersFromMixers { get; }
        IMaterial Material { get; }
        void AddMixerManufactureOrder(IManufactureOrder mixerManufactureOrder);
        void AddRunTime();
        void RemoveManufactureOrdersFromMixers(IManufactureOrder mixerManufactureOrder);
        IRequestTansferTank Tank { get; set; }
        Amount RunTime { get; }
        Amount OneSecond { get; set; }
        Amount TotalMassStoragedOrProducing { get; }
        Amount TimeToEmptyMassInProcess { get; }
        WipTankForLineReport Report { get; }
    }
    public class RecipedTankManufactureOrder : ITankManufactureOrder
    {
        public Amount TimeToEmptyMassInProcess => Tank.AverageFlowRate.Value == 0 ? new Amount(0, TimeUnits.Minute) :
           new Amount(TotalMassStoragedOrProducing.GetValue(MassUnits.KiloGram) / Tank.AverageFlowRate.GetValue(MassFlowUnits.Kg_min), TimeUnits.Minute);

        public Amount TotalMassStoragedOrProducing => Tank.CurrentLevel + TotalMassProducingInMixer;
        public Amount TotalMassProducingInMixer => new Amount(
               _ManufactureOrdersFromMixers.Sum(x => x.BatchSize.GetValue(MassUnits.KiloGram)),
               MassUnits.KiloGram
           );
        public IManufactureOrder LastInOrder => _ManufactureOrdersFromMixers.Count == 0 ? null! : _ManufactureOrdersFromMixers.OrderBy(x => x.Order).Last();
        public Amount RunTime { get; set; } = new Amount(0, TimeUnits.Second);
        public Amount OneSecond { get; set; } = new Amount(1, TimeUnits.Second);
        public Guid Id { get; } = Guid.NewGuid();
        public IMaterial Material { get; private set; } = null!;
        public IRequestTansferTank Tank { get; set; } = null!;
        public RecipedTankManufactureOrder(IRequestTansferTank tank, IMaterial material)
        {
            Tank = tank;
            Material = material;
            Report = new();
            Report.Name = tank.Name;
        }
        public List<IManufactureOrder> ManufactureOrdersFromMixers => _ManufactureOrdersFromMixers;

        public WipTankForLineReport Report { get; private set; } = null!;

        List<IManufactureOrder> _ManufactureOrdersFromMixers = new List<IManufactureOrder>();
        public void AddMixerManufactureOrder(IManufactureOrder mixerManufactureOrder)
        {
            _ManufactureOrdersFromMixers.Add(mixerManufactureOrder);
        }
        public void RemoveManufactureOrdersFromMixers(IManufactureOrder mixerManufactureOrder)
        {
            if (_ManufactureOrdersFromMixers.Contains(mixerManufactureOrder))
            {
                _ManufactureOrdersFromMixers.Remove(mixerManufactureOrder);
            }
        }
        public void AddRunTime()
        {
            RunTime += OneSecond;
        }
    }
    public interface IWIPManufactureOrder : ITankManufactureOrder
    {
        ProcessLine Line { get; }
        Amount MassPendingToDeliver { get; }
        Amount AverageOutletFlow { get; }
        FromLineToWipProductionOrder LineCurrentProductionOrder { get; set; }
        FromLineToWipProductionOrder LineNextProductionOrder { get; }
        string LineName { get; }
        string MaterialName { get; }
        Amount MassDelivered { get; }
        Amount MassProduced { get; }
        Amount MassPendingToProduce { get; }


        void AddMassDelivered(Amount mass);
        void AddMassProduced(Amount mass);


        bool IsPendingToProduceCompleted();
    }
    public class WIPInletMixerManufacturingOrder : IWIPManufactureOrder
    {
        public WipTankForLineReport Report { get; private set; } = null!;
        public Amount OneSecond { get; set; } = new Amount(1, TimeUnits.Second);
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


        public IRequestTansferTank Tank { get; set; } = null!;
        public FromLineToWipProductionOrder LineCurrentProductionOrder { get; set; } = null!;
        // ✅ Constructor seguro
        public WIPInletMixerManufacturingOrder(ProcessWipTankForLine wip, FromLineToWipProductionOrder productionorder)
        {
            LineCurrentProductionOrder = productionorder;

            Tank = wip;

            LineNextProductionOrder = Line.InformNextProductionOrder;
            Report = new WipTankForLineReport(); 
            Report.Name = wip.Name;

        }

        // ✅ Propiedad calculada: nombre del material (para reportes)
        public string MaterialName => Material.CommonName;

        // ✅ Propiedad calculada: nombre de la línea
        public string LineName => Line.Name;
        public Amount TotalMassProducingInMixer => new Amount(
               _ManufactureOrdersFromMixers.Sum(x => x.BatchSize.GetValue(MassUnits.KiloGram)),
               MassUnits.KiloGram
           );
        public Amount TotalMassStoragedOrProducing => Tank.CurrentLevel + TotalMassProducingInMixer;
        public Amount MassDelivered { get; private set; } = new Amount(0, MassUnits.KiloGram);
        public Amount MassProduced { get; private set; } = new Amount(0, MassUnits.KiloGram);
        public Amount MassToProduce => LineCurrentProductionOrder.TotalQuantityToProduce + Tank.LoLolevel * 1.1;
        public Amount MassToDeliver => LineCurrentProductionOrder.TotalQuantityToProduce;
        public Amount MassPendingToProduce => MassToProduce - MassProduced;
        public Amount MassPendingToDeliver => MassToDeliver - MassDelivered;
        public Amount RunTime { get; private set; } = new Amount(0, TimeUnits.Second);
        public Amount AverageOutletFlow => LineCurrentProductionOrder.AverageFlow;/* => RunTime.Value == 0 ? new Amount(0, MassFlowUnits.Kg_sg) :
            new Amount(MassDelivered.GetValue(MassUnits.KiloGram) / RunTime.GetValue(TimeUnits.Minute), MassFlowUnits.Kg_min);*/



        public Amount TimeToEmptyMassInProcess => AverageOutletFlow.Value == 0 ? new Amount(0, TimeUnits.Minute) :
           new Amount(TotalMassStoragedOrProducing.GetValue(MassUnits.KiloGram) / AverageOutletFlow.GetValue(MassFlowUnits.Kg_min), TimeUnits.Minute);



        public FromLineToWipProductionOrder LineNextProductionOrder { get; private set; } = null!;


        public void AddRunTime()
        {
            RunTime += OneSecond;
        }
        public void AddMassDelivered(Amount mass)
        {
            MassDelivered += mass;
        }
        public void AddMassProduced(Amount mass)
        {
            MassProduced += mass;
        }

        public bool IsPendingToProduceCompleted()
        {
            var result = MassPendingToProduce.Value <= 0;
            if (result)
            {
                Line.ReceivedWIPCurrentOrderRealesed(LineCurrentProductionOrder);

                return true;
            }
            return false;
        }

    }
    public class WIPInletSKIDManufacturingOrder : IWIPManufactureOrder
    {
        public Amount OneSecond { get; set; } = new Amount(1, TimeUnits.Second);
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
        public WipTankForLineReport Report { get; private set; } = null!;

        public IRequestTansferTank Tank { get; set; } = null!;
        public FromLineToWipProductionOrder LineCurrentProductionOrder { get; set; } = null!;
        // ✅ Constructor seguro
        public WIPInletSKIDManufacturingOrder(ProcessWipTankForLine wip, FromLineToWipProductionOrder productionorder)
        {
            LineCurrentProductionOrder = productionorder;

            Tank = wip;
            LineNextProductionOrder = Line.InformNextProductionOrder;
            Report = new WipTankForLineReport();
            Report.Name = wip.Name;

        }

        // ✅ Propiedad calculada: nombre del material (para reportes)
        public string MaterialName => Material.CommonName;

        // ✅ Propiedad calculada: nombre de la línea
        public string LineName => Line.Name;
        public Amount TotalMassProducingInMixer => new Amount(
               _ManufactureOrdersFromMixers.Sum(x => x.BatchSize.GetValue(MassUnits.KiloGram)),
               MassUnits.KiloGram
           );
        public Amount TotalMassStoragedOrProducing => Tank.CurrentLevel + TotalMassProducingInMixer;
        public Amount MassDelivered { get; private set; } = new Amount(0, MassUnits.KiloGram);
        public Amount MassProduced { get; private set; } = new Amount(0, MassUnits.KiloGram);
        public Amount MassToProduce => LineCurrentProductionOrder.TotalQuantityToProduce + Tank.LoLolevel * 1.1;
        public Amount MassToDeliver => LineCurrentProductionOrder.TotalQuantityToProduce;
        public Amount MassPendingToProduce => MassToProduce - MassProduced;
        public Amount MassPendingToDeliver => MassToDeliver - MassDelivered;
        public Amount RunTime { get; private set; } = new Amount(0, TimeUnits.Second);
        public Amount AverageOutletFlow => LineCurrentProductionOrder.AverageFlow;/* RunTime.Value == 0 ? new Amount(0, MassFlowUnits.Kg_sg) :
            new Amount(MassDelivered.GetValue(MassUnits.KiloGram) / RunTime.GetValue(TimeUnits.Minute), MassFlowUnits.Kg_min);*/



        public Amount TimeToEmptyMassInProcess => AverageOutletFlow.Value == 0 ? new Amount(0, TimeUnits.Minute) :
           new Amount(MassPendingToProduce.GetValue(MassUnits.KiloGram) / AverageOutletFlow.GetValue(MassFlowUnits.Kg_min), TimeUnits.Minute);


        public void AddRunTime()
        {
            RunTime += OneSecond;
        }
        public void AddMassDelivered(Amount mass)
        {
            MassDelivered += mass;
        }
        public void AddMassProduced(Amount mass)
        {
            MassProduced += mass;
        }


        public bool IsPendingToProduceCompleted()
        {
            var result = MassPendingToProduce.Value <= 0;
            if (result)
            {
                Line.ReceivedWIPCurrentOrderRealesed(LineCurrentProductionOrder);

                return true;
            }
            return false;
        }
    }
    public class FromLineToWipProductionOrder
    {
        public Amount ProducedCases => ProductionSKURun.ProducedCases;
        public Amount PlannedCases => ProductionSKURun.PlannedCases;
        public Amount TimePlanned => ProductionSKURun.TimePlanned;
        public Amount TimeConsumed => ProductionSKURun.TimeConsumed;
        public Amount ProducedMass => ProductionSKURun.ProducedMass;
        public Amount PlannedMass => ProductionSKURun.TotalPlannedMass;
        public ProductionSKURun ProductionSKURun { get; set; } = null!;
        public ProcessSKUByLine SKU { get; set; }

        public Guid Id { get; } = Guid.NewGuid();
        public IMaterial Material => SKU.Material;
        public Amount Size => SKU.Size;
        public ProcessLine Line { get; set; } = null!;
        public List<WipTankForLineReport> WipTankReports { get; set; } = new();
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
            WipTankReports.Add(wip.CurrentReport);
        }

    }

    public class FromWIPToMixerManufactureOrder
    {
        public Guid Id { get; } = Guid.NewGuid();
        public IMaterial Material { get; private set; } = null!;
        public ProcessWipTankForLine WIPTank { get; private set; } = null!;
        public IWIPManufactureOrder WIPOrder { get; private set; } = null!;

        public FromWIPToMixerManufactureOrder(IWIPManufactureOrder _WIPOrder, ProcessWipTankForLine wip)
        {
            Material = _WIPOrder.Material;
            WIPTank = wip;
            WIPOrder = _WIPOrder;
        }

    }
    public class TransferFromMixertoWIPOrder
    {
        public TransferFromMixertoWIPOrder(ProcessMixer SourceMixer, IProcessTank DestinationWip, Amount TotalQuantityToTransfer, Amount TransferFlow)
        {
            this._SourceMixer = SourceMixer;
            this.TotalQuantityToTransfer = TotalQuantityToTransfer;
            this.TransferFlow = TransferFlow;
            this._DestinationWip = DestinationWip;
        }
        public ProcessMixer SourceMixer => _SourceMixer;
        public IProcessTank DestinationWip => _DestinationWip;
        public Guid Id { get; } = Guid.NewGuid();
        public ProcessMixer _SourceMixer { get; set; } = null!;
        public IProcessTank _DestinationWip { get; set; } = null!;
        public Amount TotalQuantityToTransfer { get; set; } = new Amount(0, MassUnits.KiloGram);
        public Amount MassReceived { get; set; } = new Amount(0, MassUnits.KiloGram);
        public Amount PendingToReceive => TotalQuantityToTransfer - MassReceived;
        public Amount TransferFlow { get; set; } = new Amount(0, MassFlowUnits.Kg_min);

        public bool CanTransferWithoutOverflowingDestination()
        {
            if (_DestinationWip == null || PendingToReceive.Value == 0)
                return true; // Nada que transferir → seguro

            if (TransferFlow.Value == 0)
                return false; // Flujo inválido
            var currentLevel = _DestinationWip.CurrentLevel;
            var futurelevel = currentLevel + PendingToReceive;
            var pendingtimeToreceive = PendingToReceive / TransferFlow;
            var outletDuringPendingTime = _DestinationWip.OutletFlows * pendingtimeToreceive;

            futurelevel -= outletDuringPendingTime;
            if (futurelevel <= _DestinationWip.Capacity)
            {
                return true;
            }
            return false;

        }
        public bool IsTransferComplete => PendingToReceive.Value <= 0;

        public bool IsTransferStarved => _DestinationWip.InletState is ITransferFromMixerStarved;
        public bool IsTransferAvailable => _DestinationWip.InletState is ITransferFromMixerAvailable;

        Amount OneSecond { get; set; } = new Amount(1, TimeUnits.Second);
        public void SetCurrentMassTransfered()
        {
            Amount massTransfered = TransferFlow * OneSecond;
            if (PendingToReceive <= massTransfered)
            {
                massTransfered = PendingToReceive;
            }
            _DestinationWip.CurrentLevel += massTransfered;
            MassReceived += massTransfered;
            _SourceMixer.ReceiveReportOfMassDelivered(massTransfered);
        }
        public void SendMixerTransferIsFinished()
        {
            SourceMixer.ReceiveTransferFinalizedFromWIP();

        }

    }
    public interface IManufactureOrder
    {
        ITankManufactureOrder WIPOrder { get; }
        Amount BatchSize { get; set; }
        Amount PendingBatchTime { get; }
        int Order { get; set; }
        Amount CurrentBatchTime { get; }
        Queue<IRecipeStep> RecipeSteps { get; set; }
        IRecipeStep CurrentStep { get; set; }
        IMaterial Material { get; }
        Amount CurrentStarvedTime { get; }
        int TotalSteps { get; set; }
        ManufaturingEquipment ManufaturingEquipment { get; }
    }
    public class MixerManufactureOrder : IManufactureOrder
    {
        public BatchReport BatchReport { get; private set; } = null!;
        public Guid Id { get; } = Guid.NewGuid();
        public ManufaturingEquipment ManufaturingEquipment { get; private set; } = null!;
        public ITankManufactureOrder WIPOrder { get; private set; } = null!;
        public int Order { get; set; }
        public IMaterial Material { get; private set; } = null!;
        public int TotalSteps { get; set; } = 0;
        public MixerManufactureOrder(ManufaturingEquipment Mixer, ITankManufactureOrder WIPOrder, BatchReport _BatchReport)
        {
            this.ManufaturingEquipment = Mixer;
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
            WIPOrder.Report.Batches.Add(_BatchReport);
            BatchReport = _BatchReport;
        }


        public Amount BatchSize { get; set; } = new(0, MassUnits.KiloGram);
        public Amount BatchTime { get; set; } = new(0, MassUnits.KiloGram);
        public Amount RealBatchTime => BatchTime + CurrentStarvedTime;
        public Amount CurrentStarvedTime => BatchReport.TotalStarvedTime;

        public Amount CurrentBatchTime => BatchReport.RealBatchTime;
        public Amount TheoricalPendingBatchTime => BatchTime - CurrentBatchTime;
        public Amount CurrentMixerLevel => ManufaturingEquipment.CurrentLevel;
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
        public ManufaturingEquipment ManufaturingEquipment { get; private set; } = null!;
        public ITankManufactureOrder WIPOrder { get; private set; } = null!;
        public int Order { get; set; }
        public IMaterial Material { get; private set; } = null!;
        public int TotalSteps { get; set; } = 0;
        public SKIDManufactureOrder(ManufaturingEquipment Mixer, ITankManufactureOrder WIPOrder)
        {
            this.ManufaturingEquipment = Mixer;
            this.WIPOrder = WIPOrder;
            Material = WIPOrder.Material;


        }

        public Amount BatchSize { get; set; } = new(0, MassUnits.KiloGram);
        public Amount BatchTime { get; set; } = new(0, MassUnits.KiloGram);
        public Amount RealBatchTime => BatchTime + CurrentStarvedTime;
        public Amount CurrentStarvedTime { get; set; } = new Amount(0, TimeUnits.Minute);
        public Amount CurrentBatchTime { get; set; } = new Amount(0, TimeUnits.Minute);
        public Amount TheoricalPendingBatchTime => BatchTime - CurrentBatchTime;
        public Amount CurrentMixerLevel => ManufaturingEquipment.CurrentLevel;
        public bool IsBatchFinished { get; set; } = false;
        public bool IsBatchStarved { get; set; } = false;

        public IRecipeStep CurrentStep { get; set; } = null!;
        public Queue<IRecipeStep> RecipeSteps { get; set; } = new Queue<IRecipeStep>();
        public IManufactureFeeder SelectedFeder { get; set; } = null!;
        public Amount PendingBatchTime => RealBatchTime - CurrentBatchTime;

    }

}
