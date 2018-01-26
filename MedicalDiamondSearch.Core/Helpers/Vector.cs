using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace MedicalDiamondSearch.Core.Helpers
{
    public struct Vector
    {
        public Vector(int x, int y)
        {
            X = x;
            Y = y;
        }

        public Vector(Point p1, Point p2)
        {
            X = p2.X - p1.X;
            Y = p2.Y - p1.Y;
        }

        public int X { get; }
        public int Y { get; }

        public static Vector operator -(Vector v1, Vector v2) => new Vector(v1.X - v2.X, v1.Y - v2.Y);
        public static Vector operator +(Vector v1, Vector v2) => new Vector(v1.X + v2.X, v1.Y + v2.Y);
    }
}
