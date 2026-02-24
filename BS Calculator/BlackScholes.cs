using System;

namespace BS_Calculator
{
    public enum OptionType
    {
        Call,
        Put
    }

    public static class BlackScholes
    {
        public static double Calculate(
            double S,      // Underlying price
            double K,      // Strike
            double r,      // Risk-free rate
            double T,      // Time to maturity (in years)
            double sigma,  // Volatility
            OptionType type)
        {
            double d1 = (Math.Log(S / K) + (r + 0.5 * sigma * sigma) * T)
                        / (sigma * Math.Sqrt(T));

            double d2 = d1 - sigma * Math.Sqrt(T);

            if (type == OptionType.Call)
            {
                return S * CND(d1) - K * Math.Exp(-r * T) * CND(d2);
            }
            else
            {
                return K * Math.Exp(-r * T) * CND(-d2)
                       - S * CND(-d1);
            }
        }

        // Cumulative Normal Distribution (N(0,1))
        private static double CND(double x)
        {
            return 0.5 * (1.0 + Erf(x / Math.Sqrt(2.0)));
        }

        // Error function approximation
        private static double Erf(double x)
        {
            // Abramowitz & Stegun approximation
            double sign = Math.Sign(x);
            x = Math.Abs(x);

            double a1 = 0.254829592;
            double a2 = -0.284496736;
            double a3 = 1.421413741;
            double a4 = -1.453152027;
            double a5 = 1.061405429;
            double p = 0.3275911;

            double t = 1.0 / (1.0 + p * x);
            double y = 1.0 - (((((a5 * t + a4) * t)
                                + a3) * t + a2) * t + a1)
                * t * Math.Exp(-x * x);

            return sign * y;
        }
    }
}