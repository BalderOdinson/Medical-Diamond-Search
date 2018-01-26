using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using MedicalDiamondSearch.Core.Extensions;
using MedicalDiamondSearch.Core.Helpers;
using MedicalDiamondSearch.Core.Settings;

namespace MedicalDiamondSearch.Core
{
    public class Mds
    {
        private static double _treshold;

        public static IDictionary<Point, Vector> CalculateVectors(Image referentImage, Image currentImage)
        {
            _treshold = MedicalDiamondSearchSettings.InitialTreshold;
            return referentImage.Blocks.ToDictionary(block => block.Key, block => CalculateVector(block.Value, block.Value, currentImage));
        }

        private static Vector CalculateVector(PixelBlock referentBlock, PixelBlock centerBlock, Image image)
        {
            var diamond = new LargeDiamond(referentBlock, centerBlock, image);
            var mins = diamond.GetMinimums();
            if (mins.Treshold > _treshold)
            {
                if (diamond.IsCenterBlock(mins.Minimum))
                {
                    return new Vector(referentBlock.Position, new SmallDiamond(referentBlock, diamond.CenterBlock, image).GetMinimums().Minimum.Position);
                }
                var flippedPoint = mins.Minimum.FindFlippedBlock(centerBlock);
                if (referentBlock.IsInSearchWindow(flippedPoint, MedicalDiamondSearchSettings.SearchParameterP,
                    MedicalDiamondSearchSettings.BlockSize, image.Width, image.Height))
                {
                    var flippedBlock = image.Blocks[flippedPoint];
                    return flippedBlock.BlockDistortion(referentBlock) < mins.Minimum.BlockDistortion(referentBlock)
                        ? new Vector(referentBlock.Position, flippedPoint)
                        : new Vector(referentBlock.Position, mins.Minimum.Position);
                }
            }

            var dict = new Dictionary<PixelBlock, Vector>();
            var list = new List<PixelBlock>
            { mins.Minimum, mins.SecondMinimum };

            var firstAddValue =
                mins.Minimum.FindFlippedAndAddToVectorDictionary(centerBlock, referentBlock, image, dict);
            if(firstAddValue.HasValue) list.Add(firstAddValue.Value);

            var secondAddValue = mins.SecondMinimum.FindFlippedAndAddToVectorDictionary(centerBlock, referentBlock, image, dict);
            if (secondAddValue.HasValue) list.Add(secondAddValue.Value);


            for (int i = 0; i < 3; i++)
            {
                mins = list.GetMinimums(referentBlock);
                if(mins.Treshold > _treshold)
                    return new Vector(referentBlock.Position, mins.Minimum.Position);
                list = new List<PixelBlock>
                { mins.Minimum, mins.SecondMinimum };
                firstAddValue =
                    mins.Minimum.FindFlippedAndAddToVectorDictionary(referentBlock, image, dict);
                if (firstAddValue.HasValue) list.Add(firstAddValue.Value);

                secondAddValue = mins.SecondMinimum.FindFlippedAndAddToVectorDictionary(referentBlock, image, dict);
                if (secondAddValue.HasValue) list.Add(secondAddValue.Value);
            }
            _treshold += 0.05;
            return new Vector(referentBlock.Position, mins.Minimum.Position);
        }
    }
}
