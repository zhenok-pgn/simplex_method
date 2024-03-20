using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace simplexMethod
{
    internal class SimplexSolver
    {
        private readonly TargetFunction startTargetFunction;
        private readonly Restriction[] startRestrictions;
        private TargetFunction targetFunction;
        private double[] vectorBasis;
        private double[] vectorB;
        private double[] vectorC;
        private double[][] A;
        public SimplexSolver(TargetFunction targetFunction, Restriction[] restrictions) 
        {
            startTargetFunction = targetFunction;
            startRestrictions = restrictions;
            targetFunction = GetNewTargetFunction();
        }

        private TargetFunction GetNewTargetFunction()
        {
            var newCoeffs = new List<double>();
            for(int i = 0; i < startTargetFunction.Coefficients.Length; i++)
                newCoeffs.Add(startTargetFunction.IsMaximize ? startTargetFunction.Coefficients[i] : startTargetFunction.Coefficients[i] *= -1);

            foreach(var restriction in startRestrictions)
            {
                switch (restriction.Sign)
                {
                    case ComparisonSigns.Equal:
                        newCoeffs.Add(double.MinValue);
                        break;
                    case ComparisonSigns.GreaterOrEqual:
                        newCoeffs.Add(0.0);
                        newCoeffs.Add(double.MinValue);
                        break;
                    case ComparisonSigns.LessOrEqual:
                        newCoeffs.Add(0.0);
                        break;
                }
            }

            return new TargetFunction(coefficients : newCoeffs.ToArray());
        }
    }
}
