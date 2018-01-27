using System.Collections.Generic;
using System.Drawing;
using Windows.UI.Xaml.Media.Imaging;
using MedicalDiamondSearch.Core.Helpers;

namespace MedicalDiamondSearch.Uwp.Extensions
{
    public static class BitmapExtensions
    {
        public static IEnumerable<Pixel> GetPixels(this WriteableBitmap bitmap)
        {
            for (int y = 0; y < bitmap.PixelHeight; y++)
            {
                for (int x = 0; x < bitmap.PixelWidth; x++)
                {
                    
                    yield return new Pixel(new Point(x,y), bitmap.GetPixel(x,y).ToDrawingColor());
                }
            }
        }

        public static decimal Compare(this WriteableBitmap first, WriteableBitmap second)
        {
            var diff = 0;
            for (int y = 0; y < first.PixelHeight; y++)
            {
                for (int x = 0; x < first.PixelWidth; x++)
                {
                    if (first.GetPixel(x, y) != second.GetPixel(x, y))
                        diff++;
                }
            }

            return (decimal) diff / (first.PixelHeight * first.PixelWidth);
        }

        public static Color ToDrawingColor(this Windows.UI.Color winColor)
        {
            return Color.FromArgb(winColor.A,winColor.R,winColor.G, winColor.B);
        }

        public static Windows.UI.Color ToWinColor(this Color drawingColor)
        {
            return Windows.UI.Color.FromArgb(drawingColor.A, drawingColor.R, drawingColor.G, drawingColor.B);
        }
    }
}
