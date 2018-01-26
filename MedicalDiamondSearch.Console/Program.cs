using MedicalDiamondSearch.Console.Extensions;
using MedicalDiamondSearch.Core;
using MedicalDiamondSearch.Core.Helpers;

namespace MedicalDiamondSearch.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var refrentImage = new System.Drawing.Bitmap(args[0]);
            var currentImage = new System.Drawing.Bitmap(args[1]);

            var rImage = new Image(refrentImage.GetPixels(), refrentImage.Width, refrentImage.Height);
            rImage.GenerateBlocks();
            var cImage = new Image(currentImage.GetPixels(), currentImage.Width, currentImage.Height);
            cImage.GenerateBlocks();

            foreach (var keyValuePair in Mds.CalculateVectors(rImage, cImage))
            {
                if (keyValuePair.Value.X != 0 || keyValuePair.Value.Y != 0)
                    System.Console.WriteLine($"Block at: ({keyValuePair.Key.X},{keyValuePair.Key.Y}) has vector ({keyValuePair.Value.X},{keyValuePair.Value.Y})");
            }
            System.Console.ReadLine();
        }
    }
}
