using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net.Mime;
using System.Threading.Tasks;
using MedicalDiamondSearch.Console.Extensions;
using MedicalDiamondSearch.Core;
using MedicalDiamondSearch.Core.Helpers;
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

            var totalStopwatch = Stopwatch.StartNew();

            System.Console.WriteLine("Loading images...");

            var resultImage = new Bitmap(refrentImage.Width, refrentImage.Height, PixelFormat.Format32bppRgb);
            Image cImage = null;
            var cImageTask = Task.Run(() =>
            {
                cImage = new Image(currentImage.GetPixels(), currentImage.Width, currentImage.Height);
                cImage.GenerateBlocks();
            });
            var rImage = new Image(refrentImage.GetPixels(), refrentImage.Width, refrentImage.Height);
            rImage.GenerateBlocks();
            cImageTask.Wait();

            IDictionary<Point, Vector> result = null;


            System.Console.WriteLine("Executing Medical Diamond Search...");

            var stopwatch = Stopwatch.StartNew();
            result = Mds.CalculateVectors(rImage, cImage);
            stopwatch.Stop();

            System.Console.WriteLine("Calculating output image...");
            var errorCount = 0;
            foreach (var vector in result)
            {
                //Counts in only changed blocks.
                if (vector.Value.X != 0 || vector.Value.Y != 0)
                {
                    System.Console.WriteLine(
                        $"Block({vector.Key.X},{vector.Key.Y}), vector({vector.Value.X},{vector.Value.Y})");
                    foreach (var pixel in rImage.Blocks[vector.Key].Pixels)
                    {
                        if (cImage.Pixels[
                                new System.Drawing.Point(pixel.Position.X + vector.Value.X,
                                    pixel.Position.Y + vector.Value.Y)].Color != pixel.Color)
                            errorCount++;
                        resultImage.SetPixel(pixel.Position.X + vector.Value.X, pixel.Position.Y + vector.Value.Y,
                            pixel.Color);
                        refrentImage.SetPixel(pixel.Position.X + vector.Value.X, pixel.Position.Y + vector.Value.Y,
                            pixel.Color);
                    }
                }
            }
            System.Console.WriteLine("Saving output image...");
            if (!Directory.Exists("output"))
                Directory.CreateDirectory("output");
            var output = $"output/{Guid.NewGuid().ToString()}.png";
            var output1 = $"output/{Guid.NewGuid().ToString()}.png";
            resultImage.Save(output);
            refrentImage.Save(output1);

            var motionError = (decimal)errorCount / (result.Count * MedicalDiamondSearchSettings.BlockSize * MedicalDiamondSearchSettings.BlockSize);

            System.Console.WriteLine("Calculating relative error...");
            var error = refrentImage.Compare(currentImage);

            totalStopwatch.Stop();

            System.Console.WriteLine($"Time elapsed: {stopwatch.Elapsed.TotalSeconds} seconds");
            System.Console.WriteLine($"Total time elapsed: {totalStopwatch.Elapsed.TotalSeconds} seconds");
            System.Console.WriteLine($"Motion relative error: {motionError.ToString("P")}");
            System.Console.WriteLine($"Output relative error: {error.ToString("P")}");
        }
    }
}
