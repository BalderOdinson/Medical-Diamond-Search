using System;
using System.Collections.Generic;
using System.Text;

namespace MedicalDiamondSearch.Core.Extensions
{
    public struct XyzColor
    {
        public XyzColor(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public double X { get; }
        public double Y { get; }
        public double Z { get; }
    }
}
