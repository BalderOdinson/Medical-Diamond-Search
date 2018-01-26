using System;
using System.Collections.Generic;
using System.Text;

namespace MedicalDiamondSearch.Core.Extensions
{
    public struct LabColor
    {
        public LabColor(double l, double a, double b)
        {
            L = l;
            A = a;
            B = b;
        }

        public double L { get; }
        public double A { get; }
        public double B { get; }
    }
}
