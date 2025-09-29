using Simulator.Shared.NuevaSimlationconQwen.Equipments.Mixers;
using Simulator.Shared.NuevaSimlationconQwen.Equipments.Operators;
using Simulator.Shared.NuevaSimlationconQwen.Equipments.Pumps;
using Simulator.Shared.NuevaSimlationconQwen.Equipments.Tanks;
using Simulator.Shared.NuevaSimlationconQwen.ManufacturingOrders;
using Simulator.Shared.NuevaSimlationconQwen.Materials;
using Simulator.Shared.NuevaSimlationconQwen.Reports;

namespace Simulator.Shared.NuevaSimlationconQwen.Equipments.Skids
{
    public class ProcessContinuousSystem : ManufaturingEquipment, ILiveReportable
    {


        public Amount ActualFlow { get; set; } = new Amount(0, MassFlowUnits.Kg_min);
        public Amount Capacity { get; set; } = new Amount(0, MassFlowUnits.Kg_min);

        public List<IManufactureFeeder> InletFeeder => InletEquipments.OfType<IManufactureFeeder>().ToList();


        public List<ProcessWipTankForLine> WIPForProducts => OutletEquipments.OfType<ProcessWipTankForLine>().ToList();
        public List<ProcessRecipedRawMaterialTank> WIPForRawMaterialProducts => OutletEquipments.OfType<ProcessRecipedRawMaterialTank>().ToList();


        public override Amount CurrentLevel { get; set; } = new Amount(0, MassUnits.KiloGram);
        public override void ReceiveManufactureOrderFromWIP(IVesselManufactureOrder order)
        {
            CurrentManufactureOrder = new SKIDManufactureOrder(this, order);
            OutletState = new SKIDOutletWaitingNewOrderState(this);
            OutletNewOrderReceived = true;
        }


        bool OutletNewOrderReceived = false;
        bool InletNewOrderReceived = false;
        public bool IsOutletNewOrderReceived()
        {
            if (OutletNewOrderReceived)
            {
                OutletNewOrderReceived = false;
                return true;
            }
            return false;
        }
        public bool IsInitInletStateInit()
        {
            InletState = new SKIDInletStateWaitingNewOrderState(this);
            InletNewOrderReceived = true;
            return true;
        }
        public bool IsInletNewOrderReceived()
        {
            if (InletNewOrderReceived)
            {
                InletNewOrderReceived = false;
                return true;
            }
            return false;
        }
        public bool IsMustWashTank()
        {
            if (CurrentManufactureOrder == null) return false;


            if (LastMaterial == null)
            {

                LastMaterial = CurrentManufactureOrder.Material;
                return false;
            }
            if (CurrentManufactureOrder.Material == null) return false;
            if (CurrentManufactureOrder.Material.Id == LastMaterial.Id) return false;

            var washDef = WashoutTimes
                .FirstOrDefault(x => x.ProductCategoryCurrent == CurrentManufactureOrder.Material?.ProductCategory &&
                                   x.ProductCategoryNext == LastMaterial.ProductCategory);


            if (washDef != null)
            {

                return true;
            }

            return false;
        }
        
        public Amount GetWashoutTime()
        {
            if (LastMaterial != null)
            {
                var washDef = WashoutTimes
                .FirstOrDefault(x => x.ProductCategoryCurrent == CurrentManufactureOrder.Material?.ProductCategory &&
                                   x.ProductCategoryNext == LastMaterial.ProductCategory);
                if (washDef != null)
                {
                    LastMaterial = CurrentManufactureOrder.Material;
                    return washDef.MixerWashoutTime;
                }
            }

            return new Amount(0, TimeUnits.Second);
        }
        



        bool InletStartCommandReceived = false;
        bool OutletStartCommandReceived = false;
        bool InletStopCommandReceived = false;
        bool OutletStopCommandReceived = false;
        public void Produce()
        {
            InletStartCommandReceived = true;

            InletStopCommandReceived = false;

        }
        public void Stop()
        {
            ActualFlow = ZeroFlow;
            InletStartCommandReceived = false;

            InletStopCommandReceived = true;

        }
        public bool IsOutletStartCommandReceived()
        {
            if (OutletStartCommandReceived)
            {
                OutletStartCommandReceived = false;
                return true;
            }
            return false;
        }
        public bool IsInletStartCommandReceived()
        {
            if (InletStartCommandReceived)
            {
                InletStartCommandReceived = false;
                return true;
            }
            return false;
        }
        public bool IsOutletStopCommandReceived()
        {
            if (OutletStopCommandReceived)
            {
                ActualFlow = ZeroFlow;
                OutletStopCommandReceived = false;
                return true;
            }
            return false;
        }
        public bool IsInletStopCommandReceived()
        {
            if (InletStopCommandReceived)
            {
                InletStopCommandReceived = false;
                return true;
            }
            return false;
        }
        public bool IsSkidStarved()
        {
            if (InletState is ISKIDStarvedInletState)
            {
                ActualFlow = ZeroFlow;
                return true;
            }
            return false;
        }
        public bool IsSkidStarvedReleased()
        {
            if (InletState is not ISKIDStarvedInletState)
            {

                return true;
            }
            return false;
        }
        public void SendProductToWIPS()
        {
            if (InletState is not ISKIDStarvedInletState)
            {
                WIPForProducts.ForEach(x => x.ReceiveProductFromSKID(Capacity));
            }
        }
        public bool IsRawMaterialFeedersStarved()
        {
            var feeders = InletFeeder.Where(x =>
            x.OutletState is IFeederStarved
            ).ToList();
            if (feeders.Any())
            {
                var feeder = feeders.First();
                StartCriticalReport(feeder, "No available", feeder.OutletState?.StateLabel ?? "UnKnown");
                return true;
            }


            return false;
        }

        public bool IsRawMaterialFeedersReleaseStarved()
        {
            var feedersAvailable = InletFeeder.All(x =>
            x.OutletState is not IFeederStarved);
            if (feedersAvailable)
            {
                EndCriticalReport();
                return true;
            }


            return false;
        }


        public bool IsRawMaterialFeederReleased()
        {
            foreach (var feed in FeedersCatched)
            {
                ReleaseFeeder(feed);
            }
            OutletStartCommandReceived = false;
            OutletStopCommandReceived = true;
            return true;
        }
        public bool IsRawMaterialFeederCurrentOrderReleased()
        {
            foreach (var feed in FeedersCatched)
            {
                ReleaseFeeder(feed);
            }
            OutletStartCommandReceived = false;
            OutletStopCommandReceived = true;
            OutletTotalStopReceived = true;
            CurrentManufactureOrder = null!;
            return true;

          
        }
        List<IManufactureFeeder> FeedersCatched = new List<IManufactureFeeder>();
        public bool IsFeederCatched()
        {
            if (CurrentManufactureOrder == null || CurrentManufactureOrder.Material == null) return false;
            if (CurrentManufactureOrder.Material is IRecipedMaterial material)
            {
                if (material?.RecipeSteps == null) return false;

                ActualFlow = Capacity;

                foreach (var step in material.RecipeSteps)
                {
                    var feeder = AssignMaterialFeeder(step.RawMaterialId!.Value);


                    if (feeder != null)
                    {
                        FeedersCatched.Add(feeder);

                        feeder.ActualFlow = Capacity * step.Percentage / 100;
                    }
                    else
                    {
                        foreach(var feed in FeedersCatched)
                        {
                            ReleaseFeeder(feed);
                        }

                       
                        return false;
                    }
                }
                OutletStartCommandReceived = true;
                OutletStopCommandReceived = false;

                return true;
            }
            // Validación defensiva


            return false;


        }
        bool CommandFromQueueThatFeederIsRealesed = false;
        public bool IsFeederReleasedFromQueue()
        {
            if (CommandFromQueueThatFeederIsRealesed)
            {
                CommandFromQueueThatFeederIsRealesed = false;
                return true;
            }
            return false;
        }

        public void ReceiveTotalStop()
        {
            InletTotalStopReceived = true;


        }
        bool InletTotalStopReceived = false;
        bool OutletTotalStopReceived = false;
        public bool IsInletSkidTotalStopReceived()
        {
            if (InletTotalStopReceived)
            {
                InletTotalStopReceived = false;

                return true;
            }
            return false;
        }
        public bool IsOutletSkidTotalStopReceived()
        {
            if (OutletTotalStopReceived)
            {
                OutletTotalStopReceived = false;
                return true;
            }
            return false;
        }


    }


}
