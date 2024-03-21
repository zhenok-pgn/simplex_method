using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace simplexMethod
{
    public class Basis
    {
        public int[] Keys { get;}
        public double[] Values { get;}
        public Basis(int[] keys, double[] values) {
            if (keys.Length != values.Length)
                throw new Exception("Diffrent size of arrays");
            Keys = keys;
            Values = values;        
        }

        public void Set(int key, double value, int position)
        {
            Keys[position] = key; Values[position] = value;
        }
    }
}
