using System.Collections.Generic;
using System.Drawing;
using MedicalDiamondSearch.Core.Helpers;

namespace MedicalDiamondSearch.Wpf.Extensions
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

        public static decimal Compare(this Bitmap first, Bitmap second)
        {
            var diff = 0;
            for (int y = 0; y < first.Height; y++)
            {
                for (int x = 0; x < first.Width; x++)
                {
                    if (first.GetPixel(x, y) != second.GetPixel(x, y))
                        diff++;
                }
            }

            return (decimal) diff / (first.Height * first.Width);
        }
    }
}
