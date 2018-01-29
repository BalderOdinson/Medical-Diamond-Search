using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using MedicalDiamondSearch.Core.Helpers;
using MedicalDiamondSearch.Core.Settings;
using Image = System.Drawing.Image;

namespace MedicalDiamondSearch.Console.Extensions
{
    public static class BitmapExtensions
    {
        /// <summary>
        /// Gets pixels from image. Used unsafe context and parallel.for for best performance.
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public static IEnumerable<Pixel> GetPixels(this Bitmap bitmap)
        {
            Pixel[] result = null;
            unsafe
            {
                BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, bitmap.PixelFormat);

                int bytesPerPixel = Image.GetPixelFormatSize(bitmap.PixelFormat) / 8;
                int heightInPixels = bitmapData.Height;
                int widthInBytes = bitmapData.Width * bytesPerPixel;
                int widthInPixels = bitmapData.Width;
                byte* ptrFirstPixel = (byte*)bitmapData.Scan0;
                result = new Pixel[heightInPixels * widthInPixels];

                Parallel.For(0, heightInPixels, new ParallelOptions { MaxDegreeOfParallelism = MedicalDiamondSearchSettings.NumberOfThreads / 2 }, y =>
                {
                    byte* currentLine = ptrFirstPixel + (y * bitmapData.Stride);
                    for (int x = 0; x < widthInBytes; x = x + bytesPerPixel)
                    {
                        lock (result)
                        {
                            result[y * widthInPixels + x / bytesPerPixel] = new Pixel(new Point(x / bytesPerPixel, y), Color.FromArgb(currentLine[x + 2], currentLine[x + 1], currentLine[x]));
                        }
                    }
                });
                bitmap.UnlockBits(bitmapData);
            }
            return result;
        }

        /// <summary>
        /// Compares two images pixel by pixel.
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static decimal Compare(this Bitmap first, Bitmap second)
        {
            var diff = 0;
            for (var y = 0; y < first.Height; y++)
            {
                for (var x = 0; x < first.Width; x++)
                {
                    if (first.GetPixel(x, y) != second.GetPixel(x, y))
                        diff++;
                }
            }

            return (decimal)diff / (first.Height * first.Width);
        }
    }
}

