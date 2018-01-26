using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using MedicalDiamondSearch.Core.Extensions;
using MedicalDiamondSearch.Core.Settings;

namespace MedicalDiamondSearch.Core.Helpers
{
    public class SmallDiamond : Diamond
    {
        public SmallDiamond(PixelBlock referentBlock, PixelBlock centerBlock, Image image) : base(referentBlock, centerBlock)
        {
            Blocks = new List<PixelBlock> { CenterBlock };
            for (var i = -1; i < 2; i++)
            {
                for (var j = -1; j < 2; j++)
                {
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
