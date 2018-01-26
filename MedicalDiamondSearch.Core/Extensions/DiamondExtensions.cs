using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using MedicalDiamondSearch.Core.Helpers;
using MedicalDiamondSearch.Core.Settings;

namespace MedicalDiamondSearch.Core.Extensions
{
    public static class DiamondExtensions
    {
        public static MinimumPair GetMinimums(this Diamond diamond)
        {
            var result = diamond.Blocks.Select(d => new{Block = d, Mbd = d.BlockDistortion(diamond.ReferentBlock)}).OrderBy(d => d.Mbd).Take(2).ToList();
            return result.Count == 1 ? 
                new MinimumPair(result[0].Block,new PixelBlock(), double.MaxValue) 
                : new MinimumPair(result[0].Block, result[1].Block, (result[1].Mbd - result[0].Mbd) / result[1].Mbd);
        }

        public static MinimumPair GetMinimums(this IEnumerable<PixelBlock> picxelBlocks, PixelBlock referentBlock)
        {
            var result = picxelBlocks.Select(d => new { Block = d, Mbd = d.BlockDistortion(referentBlock) }).OrderBy(d => d.Mbd).Take(2).ToList();
            return result.Count == 1 ?
                new MinimumPair(result[0].Block, new PixelBlock(), double.MaxValue)
                : new MinimumPair(result[0].Block, result[1].Block, (result[1].Mbd - result[0].Mbd) / result[1].Mbd);
        }

        public static Point FindFlippedBlock(this PixelBlock block, PixelBlock center)
        {
            var vector = new Vector(center.Position, block.Position);
            return new Point(block.Position.X + vector.X, block.Position.Y + vector.Y);
        }

        public static PixelBlock? FindFlippedAndAddToVectorDictionary(this PixelBlock block, PixelBlock center,
            PixelBlock referentBlock, Image image, IDictionary<PixelBlock, Vector> dictionary)
        {
            var flip = block.FindFlippedBlock(center);
            var vector = new Vector(center.Position, block.Position);
            if (referentBlock.IsInSearchWindow(flip, MedicalDiamondSearchSettings.SearchParameterP,
                MedicalDiamondSearchSettings.BlockSize, image.Width, image.Height))
            {
                var pixelBlock = image.Blocks[flip];
                if (pixelBlock.Equals(block))
                    return null;
                if (!dictionary.ContainsKey(pixelBlock))
                    dictionary.Add(pixelBlock, vector);
                return pixelBlock;
            }
            return null;
        }

        public static PixelBlock? FindFlippedAndAddToVectorDictionary(this PixelBlock block,
            PixelBlock referentBlock, Image image, IDictionary<PixelBlock, Vector> dictionary)
        {
            if (!dictionary.ContainsKey(block))
                return null;
            var vector = dictionary[block];
            var flip = new Point(block.Position.X + vector.X, block.Position.Y + vector.Y);
            if (referentBlock.IsInSearchWindow(flip, MedicalDiamondSearchSettings.SearchParameterP,
                MedicalDiamondSearchSettings.BlockSize, image.Width, image.Height))
            {
                var pixelBlock = image.Blocks[flip];
                if (pixelBlock.Equals(block))
                    return null;
                if (!dictionary.ContainsKey(pixelBlock))
                    dictionary.Add(pixelBlock, vector);
                return pixelBlock;
            }
            return null;
        }
    }
}
