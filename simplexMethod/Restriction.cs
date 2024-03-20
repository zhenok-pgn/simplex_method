using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace simplexMethod
{
    internal class Restriction
    {
        public readonly double[,] Coefficients;
        public readonly ComparisonSigns[] Signs;
        public readonly double[] FreeCoefficients;
        public double[,] BalanceCoefficients { get {return balanceCoefficients; } }
        private double[,] balanceCoefficients;
        public Restriction(double[,] coefficients, double[] freeCoefficients,  ComparisonSigns[] signs) 
        { 
            Coefficients = coefficients;
            Signs = signs;
            FreeCoefficients = freeCoefficients;
            SetBalanceCoefficients();
        }

        private void SetBalanceCoefficients()
        {
            int equalsCount = 0;
            foreach (var sign in Signs)
                if(sign == ComparisonSigns.GreaterOrEqual)
                    equalsCount++;

            balanceCoefficients = new double[FreeCoefficients.Length + equalsCount, FreeCoefficients.Length];
            int currentPosition = 0;
            for (int i = 0; i < Signs.Length; i++)
                switch (Signs[i])
                {
                    case ComparisonSigns.Equal:
                        balanceCoefficients[i,currentPosition] = 1.0;
                        currentPosition++;
                        break;
                    case ComparisonSigns.GreaterOrEqual:
                        balanceCoefficients[i, currentPosition] = -1.0;
                        balanceCoefficients[i, currentPosition + 1] = 1.0;
                        currentPosition += 2;
                        break;
                    case ComparisonSigns.LessOrEqual:
                        balanceCoefficients[i, currentPosition] = 1.0;
                        currentPosition++;
                        break;
                }
        }
    }
}
