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
        public List<ProcessRecipedTank> WIPForRawMaterialProducts => OutletEquipments.OfType<ProcessRecipedTank>().ToList();


        public override Amount CurrentLevel { get; set; } = new Amount(0, MassUnits.KiloGram);
        public override void ReceiveManufactureOrderFromWIP(ITankManufactureOrder order)
        {
            CurrentManufactureOrder = new SKIDManufactureOrder(this, order);
            OutletState = new SKIDOutletWaitingNewOrderState(this);
            InletState = new SKIDInletStateWaitingNewOrderState(this);
        }


        public void Produce()
        {
            InletState = new SKIDInletReviewPumpsAvailableState(this);
           
        }
        public void Stop()
        {
            ActualFlow = ZeroFlow;
            foreach (var feed in FeedersCatched)
            {
                ReleaseFeeder(feed);
            }
            FeedersCatched.Clear();
            InletState = new SKIDInletStopState(this);
            OutletState = new SKIDOutletStopState(this);
        }

        
        
        public void SendProductToWIPS()
        {
            WIPForProducts.ForEach(x => x.ReceiveProductFromSKID(Capacity));
        }
        public bool IsRawMaterialFeedersStarved()
        {
            var feeder = InletFeeder.FirstOrDefault(x =>
            x.OutletState is FeederStarvedByInletState || x.OutletState is FeederPlannedDownTimeState
            );
            if (feeder != null)
            {

                StartCriticalReport(feeder, "No available", feeder.OutletState?.StateLabel ?? "UnKnown");
                OutletState = new SKIDOutletStarvedbyInletState(this);
                EnqueueForMaterialFeeder(feeder.Material!.Id);
                return true;
            }


            return false;
        }






        List<IManufactureFeeder> FeedersCatched = new List<IManufactureFeeder>();
        public bool IsFeederCatched()
        {
            if (CurrentManufactureOrder == null || CurrentManufactureOrder.Material == null) return false;
            if (CurrentManufactureOrder.Material is IRecipedMaterial material)
            {
                if (material?.RecipeSteps == null) return false;

                foreach (var step in material.RecipeSteps)
                {
                    if (IsMaterialFeederAvailable(step.RawMaterialId!.Value))                    
                    {
                        var feeder = AssignMaterialFeeder(step.RawMaterialId.Value);
                        if (feeder != null)
                        {
                            FeedersCatched.Add(feeder);

                            feeder.ActualFlow = Capacity * step.Percentage / 100;

                        }
                        else
                        {
                            ActualFlow = ZeroFlow;
                            
                            
                        }
                    }
                    else
                    {
                        foreach (var feed in FeedersCatched)
                        {
                            ReleaseFeeder(feed);
                        }
                        EnqueueForMaterialFeeder(step.RawMaterialId.Value);
                        OutletState = new SKIDOutletStarvedbyInletState(this);
                        return false;
                    }
                }
                ActualFlow = Capacity;
                OutletState = new SKIDOutletProducingState(this);
                return true;
            }

            return false;


        }
        

        public void ReceiveTotalStop()
        {
            foreach (var feed in FeedersCatched)
            {
                ReleaseFeeder(feed);
            }
            InletState = new SKIDInletStateWaitingNewOrderState(this);
            OutletState = new SKIDOutletWaitingNewOrderState(this);
            CurrentManufactureOrder = null!;
        }




    }


}
