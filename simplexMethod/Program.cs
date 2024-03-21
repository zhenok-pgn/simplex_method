using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace simplexMethod
{
    internal class Program
    {
        static void Main(string[] args)
        {
            /*Console.WriteLine("Максимизация или мимнимизация (1 - максимизация, 2 - минимизация)");
            
            bool isMaximize;
            while (true)
            {
                if (int.TryParse(Console.ReadLine(), out int userInput) && userInput < 3 && userInput > 0)
                {
                    isMaximize = userInput == 1;
                    break;
                }
                Console.WriteLine("Некорректное значение");
            }

            Console.WriteLine("Введите коэффициенты целевой функции. Для завершения нажмите клавишу E");
            
            var funcParams = new List<double>();
            while (true)
            {
                var input = Console.ReadLine();
                if (double.TryParse(input, out double userInput))
                {
                    funcParams.Add(userInput);
                }
                else if(input == "E" || input == "e" || input == "у" || input == "У")
                    break;
                else
                    Console.WriteLine("Некорректное значение");
            }

            Console.WriteLine("Введите коэффициенты целевой функции. Для завершения нажмите клавишу E");

            var restrictParams = new List<double[]>();
            while (true)
            {
                var arr = new double[funcParams.Count + 1];
                int i = 0;
                bool isBreak = false;
                Console.WriteLine($"{restrictParams.Count + 1}-е ограничение");
             
                while (i < arr.Length)
                {
                    var input = Console.ReadLine();
                    if (input == "E" || input == "e" || input == "у" || input == "У")
                    { 
                        isBreak = true;
                        break;
                    }
                    if (double.TryParse(input, out double userInput))
                    {
                        arr[i] = userInput;
                        i++;
                        continue;
                    }
                    Console.WriteLine("Некорректное значение");
                }
                if (isBreak)
                    break;
                restrictParams.Add(arr);
            }

            var isMaximize = true;
            var funcParams = new List<double> { 1, -3 / 4 };
            var restrictParams = new List<double[]> {
                new double[] { 1, 2, 10 },
                new double[] { 3, 2, 18 },
                new double[] { -1, 1, 13.0/2 },
                new double[] { 1.0/2, -1, 7 }
            };*/

            var targetFunc = new TargetFunction(new double[] { -1.0, 2.0 }, false);
            var restrictions = new Restriction(
                new double[,] { 
                    {2.0, -3.0 },
                    {1.0, -1.0 },
                    {2.0, -1.0 }
                }, 
                new double[] {0.0, 3.0, 4.0}, 
                new ComparisonSigns[] { 
                    ComparisonSigns.GreaterOrEqual, 
                    ComparisonSigns.LessOrEqual, 
                    ComparisonSigns.Equal}
                );

            var simplexSolver = new SimplexSolver( targetFunc, restrictions );

            /*double[,] table = new double[restrictParams.Count, funcParams.Count + 2 + restrictParams.Count];
            for(int i = 0; i < table.GetLength(0); i++)
            {
                for(int j = 0; j < table.GetLength(1); j++)
                {
                    if (j == 0)
                        table[i, j] = 0;
                    else if (j == 1)
                        table[i, j] = restrictParams[i][restrictParams[i].Length - 1];
                    else if (j > 1 && j < 2 + funcParams.Count)
                        table[i, j] = restrictParams[i][j - 2];
                    else
                    {
                        var newJ = j - (2 + funcParams.Count);
                        table[i, j] = i == newJ ? 1 : 0;
                    }
                }
            }

            var coefficients = GetFuncCoefficeints(funcParams, restrictParams);
            while (true)
            {
                var z = GetScalarProduct(table, 0, 1);
                var simplexDiffrences = GetSimplexDiffrences(table, coefficients);
                if(IsBreak(simplexDiffrences)) 
                { 
                    break; 
                }

                var guideColIndex = GetGuideColIndex(simplexDiffrences);
                var guideRowIndex = GetGuideRowIndex(table, guideColIndex);
                
                table = GetNewSimplexTable(table, coefficients, guideRowIndex, guideColIndex);
            }*/
        }

        /*static double GetScalarProduct(double[,] table, int j1, int j2)
        {
            double result = 0;
            for(int i = 0; i < table.GetLength(0); i++)
                result += table[i, j1] * table[i, j2];
            return result;
        }

        static bool IsBreak(double[] simplexDiffrences)
        {
            return false;
        }

        static double[] GetFuncCoefficeints(List<double> funcParams, List<double[]> restrictParams)
        {
            var result = new double[funcParams.Count + restrictParams.Count];
            for(int i = 0; i < funcParams.Count; i++)
                result[i] = funcParams[i];
            return result;
        }

        static double[] GetSimplexDiffrences(double[,] table, double[] coefficients)
        {
            var result = new double[coefficients.Length];
            for(int i = 0;i < coefficients.Length;i++)
            {
                result[i] = GetScalarProduct(table, 0, i) - coefficients[i];
            }
            return result;
        }

        static int GetGuideColIndex(double[] simplexDiffrences)
        {
            var min = simplexDiffrences[0];
            var index = 0;
            for(int i = 0; i < simplexDiffrences.Length; i++)
                if (simplexDiffrences[i] < min)
                {
                    min = simplexDiffrences[i];
                    index = i;
                }
            return index;
        }

        static int GetGuideRowIndex(double[,] table, int colIndex)
        {
            var min = double.MaxValue;
            var index = 0;
            for (int i = 0; i < table.GetLength(0); i++)
            {
                if (table[i, colIndex + 2] < 0)
                    continue;
                var div = table[i, 1] / table[i, colIndex + 2];
                if (div < min)
                {
                    min = div;
                    index = i;
                }
            }       
            return index;
        }

        static double[,] GetNewSimplexTable(double[,] table, double[] coefficients, int guideRowIndex, int guideColIndex)
        {
            double[,] newTable = new double[table.GetLength(0), table.GetLength(1)];
            for (int i = 0; i < newTable.GetLength(0); i++)
            {
                for (int j = 0; j < newTable.GetLength(1); j++)
                {
                    if (j == 0)
                        newTable[i, j] = i == guideRowIndex ? coefficients[guideColIndex] : table[i, j];
                    else if (j == 1)
                        newTable[i, j] = i == guideRowIndex ?
                            table[i, j] / table[guideRowIndex, guideColIndex + 2] :
                            table[i, j] - table[guideRowIndex, j] * table[i, guideColIndex + 2] / table[guideRowIndex, guideColIndex + 2];
                    else if (j == guideColIndex + 2)
                        newTable[i, j] = i == guideRowIndex ? 1 : 0;
                    else
                        newTable[i, j] = i == guideRowIndex ?
                            table[i, j] / table[guideRowIndex, guideColIndex + 2] :
                            table[i, j] - table[guideRowIndex, j] * table[i, guideColIndex + 2] / table[guideRowIndex, guideColIndex + 2];
                }
            }
            return newTable;
        }*/
    }
}
