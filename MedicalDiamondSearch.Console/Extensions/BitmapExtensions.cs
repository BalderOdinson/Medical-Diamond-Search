using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using MedicalDiamondSearch.Core.Helpers;

namespace MedicalDiamondSearch.Console.Extensions
{
    public static class BitmapExtensions
    {
        public static IEnumerable<Pixel> GetPixels(this Bitmap bitmap)
        {
            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    yield return new Pixel(new Point(x,y), bitmap.GetPixel(x,y));
                }
            }
        }
    }
}
