using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace simplexMethod
{
    internal class VectorMath
    {
        public static double GetScalarProduct(double[] v1, double[] v2)
        {
            double result = 0;
            for (int i = 0; i < v1.Length; i++)
                result += v1[i] * v2[i];
            return result;
        }
    }
}
