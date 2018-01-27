using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using MedicalDiamondSearch.Core.Extensions;
using MedicalDiamondSearch.Core.Helpers;
using MedicalDiamondSearch.Core.Settings;

namespace MedicalDiamondSearch.Core
{
    public class Ds
    {
        private static double _treshold;

        public static IDictionary<Point, Vector> CalculateVectors(Image referentImage, Image currentImage)
        {
            _treshold = MedicalDiamondSearchSettings.InitialTreshold;
            ConcurrentDictionary<Point, Vector> dictionary = new ConcurrentDictionary<Point, Vector>();
            Parallel.ForEach(referentImage.Blocks, new ParallelOptions { MaxDegreeOfParallelism = MedicalDiamondSearchSettings.NumberOfThreads }, (block) =>
            {
                dictionary.GetOrAdd(block.Key, CalculateVector(block.Value, currentImage.Blocks[block.Key], currentImage));
            });
            return dictionary;
        }

        private static Vector CalculateVector(PixelBlock referentBlock, PixelBlock centerBlock, Image image)
        {
            while (true)
            {
                Diamond diamond = new LargeDiamond(referentBlock, centerBlock, image);
                var mins = diamond.GetMinimums();
                if (diamond.IsCenterBlock(mins.Minimum))
                {
                    diamond = new SmallDiamond(referentBlock,centerBlock,image);
                    mins = diamond.GetMinimums();
                    return new Vector(referentBlock.Position, mins.Minimum.Position);
                }
                centerBlock = mins.Minimum;
            }
        }
    }
}
