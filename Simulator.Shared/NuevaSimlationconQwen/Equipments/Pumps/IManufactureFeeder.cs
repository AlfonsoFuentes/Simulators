namespace Simulator.Shared.NuevaSimlationconQwen.Equipments.Pumps
{
   

    public interface IManufactureFeeder:IEquipment
    { 
        Amount Flow { get; set; }
        Amount ActualFlow { get; set; }
        bool IsForWashout {  get; set; }

        bool IsAnyTankInletStarved();
        bool IsInUse();
        string OcuppiedBy {  get; set; }
    }
}
