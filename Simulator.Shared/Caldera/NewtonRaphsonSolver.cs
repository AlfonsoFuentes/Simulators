namespace Caldera
{
    public static class NewtonRaphsonSolver
    {
        public static void IterarParaTemperatura(
            Func<double, double> funcionObjetivo,
            double tempInicialK,
            out double tempSolucionK,
            out double errorFinal)
        {
            double tolerancia = 1e-4;
            int maxIteraciones = 100;
            double temp = tempInicialK;
            double ftemp=0;

            for (int i = 0; i < maxIteraciones; i++)
            {
                ftemp = funcionObjetivo(temp);
                if (Math.Abs(ftemp) < tolerancia) break;

                double df = funcionObjetivo(temp + 1) - ftemp; // Derivada numérica simple
                if (df == 0) throw new InvalidOperationException("Derivada cero. No se puede continuar.");

                temp -= ftemp / df;
            }

            tempSolucionK = temp;
            errorFinal = ftemp;
        }
    }
}
