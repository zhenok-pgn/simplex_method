using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace simplexMethod
{
    internal class Restriction
    {
        public readonly double[] Coefficients;
        public readonly ComparisonSigns Sign;
        public readonly double FreeCoefficient;
        public double[] BalanceCoefficients { get {return balanceCoefficients; } }
        private double[] balanceCoefficients;
        public Restriction(double[] coefficients, double freeCoefficient,  ComparisonSigns sign) 
        { 
            Coefficients = coefficients;
            Sign = sign;
            FreeCoefficient = freeCoefficient;
            GetBalanceCoefficients();
        }

        private void GetBalanceCoefficients()
        {
            switch (Sign)
            {
                case ComparisonSigns.Equal:
                    balanceCoefficients = new double[] { 1.0 };
                    break;
                case ComparisonSigns.GreaterOrEqual:
                    balanceCoefficients = new double[] { -1.0, 1.0 };
                    break;
                case ComparisonSigns.LessOrEqual:
                    balanceCoefficients = new double[] { 1.0 };
                    break;
            }
        }
    }
}
