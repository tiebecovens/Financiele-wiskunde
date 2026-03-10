using System;

namespace Pricer.Numerics
{
    public static class StochasticProcesses
    {
        /// <summary>
        /// Genereert een gesimuleerd pad op basis van een random walk met diffusive scaling.
        /// </summary>
        /// <param name="numberOfSteps">Het aantal tijdsintervallen.</param>
        /// <param name="scalingExponent">De alpha parameter voor de schaling.</param>
        /// <param name="scaleFactor">Vermenigvuldigingsfactor voor de zichtbaarheid (bijv. 100).</param>
        /// <returns>Een array met de y-waarden van het gegenereerde pad.</returns>
        public static double[] GenerateBrownianMotionPath(int numberOfSteps, double scalingExponent, double scaleFactor = 100.0)
        {
            var random = new Random();
            
            double dt = 1.0 / numberOfSteps;
            
            // Hier vermenigvuldigen we de ruimtelijke stapgrootte met de schaalfactor (100)
            double dx = Math.Pow(dt, scalingExponent) * scaleFactor; 

            double[] data = new double[numberOfSteps + 1];
            data[0] = 0.0; // Startpunt

            for (int i = 1; i <= numberOfSteps; i++)
            {
                double stepDirection = random.NextDouble() >= 0.5 ? 1.0 : -1.0;
                data[i] = data[i - 1] + stepDirection * dx;
            }

            return data;
        }
    }
}