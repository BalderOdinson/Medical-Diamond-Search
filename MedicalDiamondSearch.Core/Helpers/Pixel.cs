using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using MedicalDiamondSearch.Core.Extensions;

namespace MedicalDiamondSearch.Core.Helpers
{
    public struct Pixel
    {
        public Pixel(Point position, Color color)
        {
            Position = position;
            Color = color;
        }

        public Point Position { get; }
        public Color Color { get; }

        public static double operator -(Pixel p1, Pixel p2) =>
            p1.Color.Cie1976Compare(p2.Color);

        public override bool Equals(object obj)
        {
            if (!(obj is Pixel pixel))
                return false;
            return pixel.Position.Equals(Position);
        }

        public override int GetHashCode()
        {
            return Position.GetHashCode();
        }

        public override string ToString()
        {
            return $"({Position.X},{Position.Y}) RGB({Color.R},{Color.G},{Color.B}))";
        }
    }
}
