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
    /// <summary>
    /// Diamond search
    /// </summary>
    public class Ds
    {
        /// <summary>
        /// Executes Diamond Search Algorithm for each block in image.
        /// </summary>
        /// <param name="referentImage"></param>
        /// <param name="currentImage"></param>
        /// <returns></returns>
        public static IDictionary<Point, Vector> CalculateVectors(Image referentImage, Image currentImage)
        {
            ConcurrentDictionary<Point, Vector> dictionary = new ConcurrentDictionary<Point, Vector>();
            Parallel.ForEach(referentImage.Blocks, new ParallelOptions { MaxDegreeOfParallelism = MedicalDiamondSearchSettings.NumberOfThreads }, (block) =>
            {
                dictionary.GetOrAdd(block.Key, CalculateVector(block.Value, currentImage.Blocks[block.Key], currentImage));
            });
            return dictionary;
        }

        /// <summary>
        /// Diamond Search Algorithm.
        /// </summary>
        /// <param name="referentBlock"></param>
        /// <param name="centerBlock"></param>
        /// <param name="image"></param>
        /// <returns></returns>
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
