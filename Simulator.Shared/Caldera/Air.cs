namespace Caldera
{
    public class Air : CompoundList
    {
        public Compound O2 = new("Oxigen", "O2", 32, 29.1, 1.158, -0.6076, 1.311, 2);
        public Compound N2 = new("Nitrogen", "N2", 28.02, 29, 0.2199, 0.5723, -2.871, 2);
        public CompundH2O H2O = new("Water", "H2O", 18.016, 33.46, 0.688, 0.7604, -3.593, 2);
        public Air()
        {


            List.Add(O2);
            List.Add(N2);
            List.Add(H2O);
        }
    }
}
