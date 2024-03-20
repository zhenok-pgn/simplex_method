﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace simplexMethod
{
    internal class TargetFunction
    {
        public bool IsMaximize { get; set; }
        public double[] Coefficients { get; set; }

        public TargetFunction(bool isMaximize = true, double[] coefficients) {
            IsMaximize = isMaximize;
            Coefficients = coefficients;
        }
    }
}