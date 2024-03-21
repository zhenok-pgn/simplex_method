using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace simplexMethod
{
    internal class SimplexSolver
    {
        public event EventHandler<PrintIterationEventArgs> OnPrintIteration;
        public double FunctionValue { get; private set; }
        public double[] X { get; private set; }

        private readonly TargetFunction startTargetFunction;
        private readonly Restriction startRestrictions;
        private TargetFunction targetFunction;
        private Basis vectorBasis;
        private double[] vectorB;
        private double[,] A;
        public SimplexSolver(TargetFunction targetFunction, Restriction restrictions) 
        {
            startTargetFunction = targetFunction;
            startRestrictions = restrictions;
            this.targetFunction = GetNewTargetFunction();
            vectorB = startRestrictions.FreeCoefficients;
            vectorBasis = GetVectorBasis();
            A = GetA();
        }

        public void StartSolving()
        {
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

        private Basis GetVectorBasis()
        {
            var result = new Basis(new int[vectorB.Length], new double[vectorB.Length]);
            for (int i = 0; i < startRestrictions.BalanceCoefficients.GetLength(0); i++)
            {
                for (int j = 0; j < startRestrictions.BalanceCoefficients.GetLength(1); j++)
                    if (startRestrictions.BalanceCoefficients[i, j] == 1)
                    {
                        var posNumber = startRestrictions.Coefficients.GetLength(1) + j;
                        result.Set(posNumber, targetFunction.Coefficients[posNumber], i);
                        break;
                    }
            }
            return result;
        }

        private double[,] GetA()
        {
            var result = new double[vectorB.Length, targetFunction.Coefficients.Length];
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
                result[i] = VectorMath.GetScalarProduct(vectorBasis.Values, A.GetColumn(i)) - targetFunction.Coefficients[i];
            }
            return result;
        }

        private ExitStatus GetExitStatus(double[] simplexDiffrences)
        {
            for (int i = 0; i < simplexDiffrences.Length; i++)
            { 
                if (simplexDiffrences[i] > 0)
                    continue;

                bool isNoAnswer = true;
                for (int j = 0; j < A.GetLength(0); j++)
                    if (A[j, i] > 0)
                    { isNoAnswer = false; break; }
                if (isNoAnswer)
                    return ExitStatus.ExitWithNoAnswer;
                else if (simplexDiffrences[i] != 0)
                    return ExitStatus.NoExit;
            } 
            return ExitStatus.ExitOK;
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
            vectorBasis.Set(colIndex, targetFunction.Coefficients[colIndex], rowIndex);

            var newVecB = new double[vectorB.Length];
            var newA = new double[A.GetLength(0), A.GetLength(1)];

            for(int i = 0; i < newVecB.Length;i++)
            {
                newVecB[i] = i == rowIndex ? vectorB[i] / A[i, colIndex] : vectorB[i] - (vectorB[rowIndex] * A[i, colIndex] / A[rowIndex, colIndex]);
            }

            for (int i = 0; i < newA.GetLength(0); i++)
            {
                for (int j = 0; j < newA.GetLength(1); j++)
                {
                    if (j == colIndex)
                        newA[i, j] = i == rowIndex ? 1.0 : 0.0;
                    else
                        newA[i, j] = i == rowIndex ?
                            A[i, j] / A[rowIndex, colIndex] :
                            A[i, j] - A[rowIndex, j] * A[i, colIndex] / A[rowIndex, colIndex];
                }
            }

            vectorB = newVecB;
            A = newA;
        }

        private void SetFunctionValueAndParameters(double funcValue)
        {
            FunctionValue = funcValue;
            X = new double[vectorBasis.Keys.Max() + 1];
            for (int i = 0; i < X.Length; i++)
            { 
                int pos = -1;
                for (int j = 0; j < vectorBasis.Keys.Length; j++)
                    if (vectorBasis.Keys[j] == i)
                        pos = j;
                X[i] = pos == -1 ?  0.0 : vectorB[pos]; 
            }
        }

        private void Solve()
        {
            while (true)
            {
                var z = VectorMath.GetScalarProduct(vectorBasis.Values, vectorB);
                var simplexDiffrences = GetSimplexDiffrences();
                var exitStatus = GetExitStatus(simplexDiffrences);

                SetFunctionValueAndParameters(z);

                var args = new PrintIterationEventArgs()
                {
                    TargetFunction = targetFunction,
                    VectorBasis = vectorBasis,
                    VectorB = vectorB,
                    A = A,
                    SimplexDiffrences = simplexDiffrences,
                    ExitStatus = exitStatus
                };
                if (exitStatus == ExitStatus.ExitOK || exitStatus == ExitStatus.ExitWithNoAnswer)
                {
                    PrintIteration(args);
                    break;
                }

                var guideColIndex = GetGuideColIndex(simplexDiffrences);
                var guideRowIndex = GetGuideRowIndex(guideColIndex);

                args.GuideColIndex = guideColIndex;
                args.GuideRowIndex = guideRowIndex;
                PrintIteration(args);
                SetNewSimplexIteration(guideRowIndex, guideColIndex);
            }
        }

        protected virtual void PrintIteration(PrintIterationEventArgs e)
        {
            EventHandler<PrintIterationEventArgs> handler = OnPrintIteration;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }

    public class PrintIterationEventArgs : EventArgs
    {
        public TargetFunction TargetFunction { set; get; }
        public Basis VectorBasis { set; get; }
        public double[] VectorB { set; get; }
        public double[,] A { set; get; }
        public double[] SimplexDiffrences { set; get; }
        public ExitStatus ExitStatus { set; get; }
        public int GuideColIndex { set; get; }
        public int GuideRowIndex { set; get; }
    }
}
