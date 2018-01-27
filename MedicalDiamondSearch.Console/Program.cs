using System.Diagnostics;
using System.Drawing;
using System.IO;
using MedicalDiamondSearch.Console.Extensions;
using MedicalDiamondSearch.Core;
using MedicalDiamondSearch.Core.Settings;
using Image = MedicalDiamondSearch.Core.Helpers.Image;

namespace MedicalDiamondSearch.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 6)
            {
                System.Console.WriteLine("Insufficient number of arguments.");
                return;
            }
            var refrentImage = new System.Drawing.Bitmap(args[0]);
            var currentImage = new System.Drawing.Bitmap(args[1]);
            MedicalDiamondSearchSettings.BlockSize = int.Parse(args[2]);
            MedicalDiamondSearchSettings.SearchParameterP = int.Parse(args[3]);
            MedicalDiamondSearchSettings.InitialTreshold = double.Parse(args[4]);
            MedicalDiamondSearchSettings.NumberOfThreads = int.Parse(args[5]);

            var rImage = new Image(refrentImage.GetPixels(), refrentImage.Width, refrentImage.Height);
            rImage.GenerateBlocks();
            var cImage = new Image(currentImage.GetPixels(), currentImage.Width, currentImage.Height);
            cImage.GenerateBlocks();
            var stopwatch = Stopwatch.StartNew();
            var result = Mds.CalculateVectors(rImage, cImage);
            stopwatch.Stop();
            System.Console.WriteLine($"Time elapsed: {stopwatch.Elapsed.TotalSeconds} seconds");
            foreach (var vector in result)
            {
                if (vector.Value.X != 0 || vector.Value.Y != 0)
                {
                    foreach (var pixel in rImage.Blocks[vector.Key].Pixels)
                    {
                        refrentImage.SetPixel(pixel.Position.X + vector.Value.X, pixel.Position.Y + vector.Value.Y, pixel.Color);
                    }
                }
            }
            refrentImage.Save("output.png");
            System.Console.WriteLine($"Broj blokova je: {result.Count}");
            System.Console.ReadLine();
        }
    }
}
