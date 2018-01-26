using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using MedicalDiamondSearch.Core.Settings;

namespace MedicalDiamondSearch.Core.Helpers
{
    public class Image
    {
        public int Width { get; }
        public int Height { get; }

        public Image(IEnumerable<Pixel> pixels, int width, int height)
        {
            Width = width;
            Height = height;
            Pixels = pixels.ToDictionary(pixel => pixel.Position, pixel => pixel);
            Blocks = new Dictionary<Point, PixelBlock>();
        }

        public IDictionary<Point,Pixel> Pixels { get; }
        public IDictionary<Point,PixelBlock> Blocks { get; }

        public void GenerateBlocks()
        {
            var pixselsToAdd = new List<Pixel>();
            for (var x = 0; x < Width; x += MedicalDiamondSearchSettings.BlockSize)
            {
                for (var y = 0; y < Height; y += MedicalDiamondSearchSettings.BlockSize)
                {
                    var point = new Point(x, y);
                    for (var x1 = 0; x1 < MedicalDiamondSearchSettings.BlockSize; x1++)
                    {
                        for (var y1 = 0; y1 < MedicalDiamondSearchSettings.BlockSize; y1++)
                        {
                            var innerPoint = new Point(x1 + x, y + y1);
                            if (x1 + x >= Width || y + y1 >= Height)
                            {
                                pixselsToAdd.Add(new Pixel(innerPoint, Color.Black));
                                continue;
                            }
                            pixselsToAdd.Add(Pixels[innerPoint]);
                        }
                    }
                    Blocks.Add(point, new PixelBlock(point, pixselsToAdd));
                    pixselsToAdd = new List<Pixel>();
                }
            }
        }
    }
}
