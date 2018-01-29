using System;
using System.Collections.Generic;
using System.Text;

namespace MedicalDiamondSearch.Core.Helpers
{
    /// <summary>
    /// Abstract class meant for Small diamond and Larg diamond to extend.
    /// </summary>
    public abstract class Diamond
    {
        protected Diamond(PixelBlock referentBlock, PixelBlock centerBlock)
        {
            ReferentBlock = referentBlock;
            CenterBlock = centerBlock;
        }

        /// <summary>
        /// Block according who minimum block distortion will be calculated.
        /// </summary>
        public PixelBlock ReferentBlock { get; }
        /// <summary>
        /// Center block of diamond
        /// </summary>
        public PixelBlock CenterBlock { get; }
        /// <summary>
        /// Collection of other blocks contained in diamond.
        /// </summary>
        public List<PixelBlock> Blocks { get; protected set; }
        public bool IsCenterBlock(PixelBlock block) => CenterBlock.Equals(block);

        public override string ToString()
        {
            return $"Ref: {ReferentBlock}, Center: {CenterBlock}";
        }
    }
}
