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
        private readonly Restriction startRestrictions;
        private TargetFunction targetFunction;
        private Dictionary<Crutch, double> vectorBasis;
        private double[] vectorB;
        private double[,] A;
        public SimplexSolver(TargetFunction targetFunction, Restriction restrictions) 
        {
            startTargetFunction = targetFunction;
            startRestrictions = restrictions;
            this.targetFunction = GetNewTargetFunction();
            vectorBasis = GetVectorBasis();
            vectorB = startRestrictions.FreeCoefficients;
            A = GetA();
            Solve();
        }

        private TargetFunction GetNewTargetFunction()
        {
            var newCoeffs = new List<double>();
            for(int i = 0; i < startTargetFunction.Coefficients.Length; i++)
                newCoeffs.Add(startTargetFunction.IsMaximize ? startTargetFunction.Coefficients[i] : startTargetFunction.Coefficients[i] *= -1);

            foreach(var sign in startRestrictions.Signs)
            {
                switch (sign)
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

        private Dictionary<Crutch, double> GetVectorBasis()
        {
            var result = new Dictionary<Crutch, double>();
            for (int i = 0; i < startRestrictions.BalanceCoefficients.GetLength(0); i++)
            {
                for (int j = 0; j < startRestrictions.BalanceCoefficients.GetLength(1); j++)
                    if (startRestrictions.BalanceCoefficients[i, j] == 1)
                    {
                        var posNumber = startRestrictions.Coefficients.GetLength(1) + j;
                        result.Add(new Crutch(posNumber), targetFunction.Coefficients[posNumber]);
                        break;
                    }
            }
            return result;
        }

        private double[,] GetA()
        {
            var result = new double[targetFunction.Coefficients.Length, vectorB.Length];
            var stRestrLen = startRestrictions.Coefficients.GetLength(1);
            for (int i = 0; i < result.GetLength(0); i++)
            {
                for (int j = 0; j < result.GetLength(1); j++)
                {
                    if (j < stRestrLen)
                        result[i, j] = startRestrictions.Coefficients[i, j];
                    else
                        result[i, j] = startRestrictions.BalanceCoefficients[i, j - stRestrLen];
                }
            }
            return result;
        }

        private double[] GetSimplexDiffrences()
        {
            var result = new double[targetFunction.Coefficients.Length];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = VectorMath.GetScalarProduct(vectorBasis.Values.ToArray(), A.GetColumn(i)) - targetFunction.Coefficients[i];
            }
            return result;
        }

        private bool IsBreak(double[] simplexDiffrences)
        {
            return false;
        }

        private int GetGuideColIndex(double[] simplexDiffrences)
        {
            var min = simplexDiffrences[0];
            var index = 0;
            for (int i = 0; i < simplexDiffrences.Length; i++)
                if (simplexDiffrences[i] < min)
                {
                    min = simplexDiffrences[i];
                    index = i;
                }
            return index;
        }

        private int GetGuideRowIndex(int colIndex)
        {
            var min = double.MaxValue;
            var index = 0;
            var column = A.GetColumn(colIndex);
            for (int i = 0; i < vectorB.Length; i++)
            {
                if (column[i] < 0)
                    continue;
                var div = vectorB[i] / column[i];
                if (div < min)
                {
                    min = div;
                    index = i;
                }
            }
            return index;
        }

        private void SetNewSimplexIteration(int rowIndex, int colIndex)
        {
            var newBasisElement = vectorBasis.ElementAt(rowIndex);
            newBasisElement.Key.Value = colIndex;
            vectorBasis[colIndex] = targetFunction.Coefficients[colIndex];
        }

        private void Solve()
        {
            while (true)
            {
                var z = VectorMath.GetScalarProduct(vectorBasis.Values.ToArray(), vectorB);
                var simplexDiffrences = GetSimplexDiffrences();
                if (IsBreak(simplexDiffrences))
                {
                    break;
                }

                var guideColIndex = GetGuideColIndex(simplexDiffrences);
                var guideRowIndex = GetGuideRowIndex(guideColIndex);

                table = GetNewSimplexTable(table, coefficients, guideRowIndex, guideColIndex);
            }
        }
    }
}
