using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using MedicalDiamondSearch.Core.Extensions;
using MedicalDiamondSearch.Core.Settings;

namespace MedicalDiamondSearch.Core.Helpers
{
    /// <summary>
    /// Represents small diamond shape(manhattan distance of one from center)
    /// </summary>
    public class SmallDiamond : Diamond
    {
        public SmallDiamond(PixelBlock referentBlock, PixelBlock centerBlock, Image image) : base(referentBlock, centerBlock)
        {
            Blocks = new List<PixelBlock> { CenterBlock };
            for (var i = -1; i < 2; i++)
            {
                for (var j = -1; j < 2; j++)
                {
                    //Checks if distances is one and if block is inside search window.
                    if (Math.Abs(i) + Math.Abs(j) != 1) continue;
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
