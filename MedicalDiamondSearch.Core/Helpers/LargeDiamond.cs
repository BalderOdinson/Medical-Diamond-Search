using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using MedicalDiamondSearch.Core.Extensions;
using MedicalDiamondSearch.Core.Settings;

namespace MedicalDiamondSearch.Core.Helpers
{
    /// <summary>
    /// Represents large diamond shape(manhattan distance of two from center)
    /// </summary>
    public class LargeDiamond : Diamond
    {
        public LargeDiamond(PixelBlock referentBlock, PixelBlock centerBlock, Image image) : base(referentBlock, centerBlock)
        {
            Blocks = new List<PixelBlock> {CenterBlock};
            for (var i = -2; i < 3; i++)
            {
                for (var j = -2; j < 3; j++)
                {
                    //Checks if distances is two and if block is inside search window.
                    if (Math.Abs(i) + Math.Abs(j) != 2) continue;
                    var point = new Point(CenterBlock.Position.X + i * MedicalDiamondSearchSettings.BlockSize, CenterBlock.Position.Y + j * MedicalDiamondSearchSettings.BlockSize);
                    if (referentBlock.IsInSearchWindow(point, MedicalDiamondSearchSettings.SearchParameterP,
                        MedicalDiamondSearchSettings.BlockSize, image.Width, image.Height))
                    {
                        Blocks.Add(image.Blocks[point]);
                    }
                }
            }
        }
    }
}
