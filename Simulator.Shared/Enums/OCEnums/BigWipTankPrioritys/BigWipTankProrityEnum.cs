namespace Simulator.Shared.Enums.OCEnums.BigWipTankPrioritys
{
    public class BigWipTankProrityEnum : ValueObject
    {
        public override string ToString()
        {
            return Name;
        }

        public static BigWipTankProrityEnum Create(int id, string name) => new BigWipTankProrityEnum() { Id = id, Name = name };

        public static BigWipTankProrityEnum None = Create(0, "None");
        public static BigWipTankProrityEnum High = Create(1, "High");
        public static BigWipTankProrityEnum Medium = Create(2, "Medium");
        public static BigWipTankProrityEnum Low = Create(3, "Low");

        public static List<BigWipTankProrityEnum> List = new List<BigWipTankProrityEnum>()
            {
                 None,  High, Medium, Low
            };
        public static string GetName(int id) => List.Exists(x => x.Id == id) ? List.FirstOrDefault(x => x.Id == id)!.Name : string.Empty;

        public static BigWipTankProrityEnum GetType(string type) => List.Exists(x => x.Name.Equals(type)) ? List.FirstOrDefault(x => x.Name.Equals(type))! : None;
        public static BigWipTankProrityEnum GetType(int id) => List.Exists(x => x.Id == id) ? List.FirstOrDefault(x => x.Id == id)! : None;
    }
}
