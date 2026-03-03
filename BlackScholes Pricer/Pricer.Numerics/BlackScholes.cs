using System;
using MathNet.Numerics.Distributions;

namespace Pricer.Numerics;

public enum OptionType
{
    Call,
    Put
}

public class BlackScholes
{
    private OptionType option;
    private double r; // Risk-free interest rate
    private double T; // Time to maturity
    private double sigma; // Volatility
    private double K; // Strike price
    private double S; // Underlying asset price
    private double q; // Dividend yield

    public BlackScholes(
        OptionType optionType,
        double riskFreeRate,
        double timeToMaturity,
        double volatility,
        double strike,
        double underlyingPrice,
        double dividendYield = 0.0)
    {
        option = optionType;
        r = riskFreeRate;
        T = timeToMaturity;
        sigma = volatility;
        K = strike;
        S = underlyingPrice;
        q = dividendYield;
    }

    // Cumulative normal distribution
    private static double N(double x)
        => Normal.CDF(0.0, 1.0, x);

    // Normal probability density function
    private static double n(double x)
        => Normal.PDF(0.0, 1.0, x);

    // Black–Scholes price
    public double Price()
    {
        if (T <= 0.0)
        {
            return option == OptionType.Call ? Math.Max(0.0, S - K) : Math.Max(0.0, K - S);
        }

        double d1 = (Math.Log(S / K) + (r - q + 0.5 * sigma * sigma) * T) / (sigma * Math.Sqrt(T));
        double d2 = d1 - sigma * Math.Sqrt(T);

        if (option == OptionType.Call)
        {
            return S * Math.Exp(-q * T) * N(d1) - K * Math.Exp(-r * T) * N(d2);
        }
        else
        {
            return K * Math.Exp(-r * T) * N(-d2) - S * Math.Exp(-q * T) * N(-d1);
        }
    }

    // Vega is the same for both call and put
    public double Vega()
    {
        if (T <= 0.0) return 0.0;

        double d1 = (Math.Log(S / K) + (r - q + 0.5 * sigma * sigma) * T) / (sigma * Math.Sqrt(T));
        return S * Math.Exp(-q * T) * Math.Sqrt(T) * n(d1);
    }
}

public static class ImpliedVolatilityCalculator
{
    // Black-Scholes IV via Newton-Raphson
    public static double Compute(
        OptionType optionType,
        double marketPrice,
        double interestRate,
        double timeToMaturity,
        double strike,
        double underlyingPrice,
        double initialGuess = 0.2,
        double tolerance = 1e-8,
        int maxIterations = 100)
    {
        double sigma = initialGuess;

        for (int i = 0; i < maxIterations; i++)
        {
            // Instantiëer een BlackScholes object met de huidige schatting van sigma
            var bs = new BlackScholes(optionType, interestRate, timeToMaturity, sigma, strike, underlyingPrice);
            
            double price = bs.Price();
            double vega = bs.Vega();

            // Voorkom delen door nul (wanneer vega te klein wordt)
            if (Math.Abs(vega) < 1e-8)
            {
                break;
            }

            double diff = marketPrice - price;

            // Als we binnen de tolerantie vallen, is dit onze implied volatility
            if (Math.Abs(diff) < tolerance)
            {
                return sigma;
            }

            // Newton-Raphson stap
            sigma += diff / vega;

            // Bescherming tegen negatieve of onrealistisch lage volatility tijdens iteraties
            if (sigma < 1e-6) sigma = 1e-6; 
        }

        return sigma;
    }

    // Exacte gesloten formule voor Bachelier At-The-Money Forward (F = K)
    public static double BachelierImpliedVolATM(
        double optionPrice,
        double underlyingPrice,
        double timeToMaturity,
        double interestRate)
    {
        // Voor een ATM Forward optie in het Bachelier model (bij afwezigheid van d)
        // en met een risicovrije rente r, geldt de volgende gesloten vorm:
        return optionPrice * Math.Exp(interestRate * timeToMaturity) * Math.Sqrt(2 * Math.PI / timeToMaturity);
    }
}