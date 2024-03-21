using ConsoleTables;
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
                new double[] { 0.0, 3.0, 4.0 },
                new ComparisonSigns[] {
                    ComparisonSigns.GreaterOrEqual,
                    ComparisonSigns.LessOrEqual,
                    ComparisonSigns.Equal}
                );

            var simplexSolver = new SimplexSolver(targetFunc, restrictions);
            simplexSolver.OnPrintIteration += PrintIterationHandler;
            simplexSolver.StartSolving();
        }

        private static void PrintIterationHandler(object sender, PrintIterationEventArgs e)
        {
            var firstLine = new List<string>() { "basis", "cb", "c/b" };
            for(int i = 0; i < e.TargetFunction.Coefficients.Length; i++)
            {
                firstLine.Add($"{e.TargetFunction.Coefficients[i]}/A{i + 1}");
            }

            var table = new ConsoleTable(firstLine.ToArray());
            for(int i = 0; i < e.VectorB.Length; i++)
            {
                var curLine = new List<string>
                {
                    $"x{e.VectorBasis.Keys[i]}",
                    $"{e.VectorBasis.Values[i]}",
                    $"{e.VectorB[i]}"
                };
                for(int j = 0; j < e.A.GetLength(1); j++)
                {
                    curLine.Add(e.A[i, j].ToString());
                }
                table.AddRow(curLine.ToArray());
            }

            var lastLine = new List<string>() { " ", "z=", (sender as SimplexSolver).FunctionValue.ToString() };
            for (int i = 0; i < e.SimplexDiffrences.Length; i++)
            {
                lastLine.Add($"{e.SimplexDiffrences[i]}");
            }
            table.AddRow(lastLine.ToArray());

            table.Write();
            Console.WriteLine();
        }
    }
}
