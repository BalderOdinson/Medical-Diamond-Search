using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace MedicalDiamondSearch.Core.Extensions
{
    public static class ColorExtensions
    {
        public static double Cie1976Compare(this Color color1, Color color2)
        {
            var a = color1.ConvertToXyz().ConvertToLab();
            var b = color2.ConvertToXyz().ConvertToLab();
            var differences = Distance(a.L, b.L) + Distance(a.A, b.A) + Distance(a.B, b.B);
            return Math.Sqrt(differences);
        }

        private static double Distance(double a, double b)
        {
            return (a - b) * (a - b);
        }

        public static XyzColor ConvertToXyz(this Color color)
        {
            var r = PivotRgb(color.R / 255.0);
            var g = PivotRgb(color.G / 255.0);
            var b = PivotRgb(color.B / 255.0);
            
            var x = r * 0.4124 + g * 0.3576 + b * 0.1805;
            var y = r * 0.2126 + g * 0.7152 + b * 0.0722;
            var z = r * 0.0193 + g * 0.1192 + b * 0.9505;

            return new XyzColor(x, y, z);
        }

        public static LabColor ConvertToLab(this XyzColor color)
        {
            var x = PivotXyz(color.X / 95.047);
            var y = PivotXyz(color.Y / 100.000);
            var z = PivotXyz(color.Z / 108.883);

            var l = Math.Max(0, 116 * y - 16);
            var a = 500 * (x - y);
            var b = 200 * (y - z);

            return new LabColor(l, a, b);
        }

        private static double PivotXyz(double n)
        {
            return n > 0.008856 ? CubicRoot(n) : (903.3 * n + 16) / 116;
        }

        private static double CubicRoot(double n)
        {
            return Math.Pow(n, 1.0 / 3.0);
        }

        private static double PivotRgb(double n)
        {
            return (n > 0.04045 ? Math.Pow((n + 0.055) / 1.055, 2.4) : n / 12.92) * 100.0;
        }
    }
}
