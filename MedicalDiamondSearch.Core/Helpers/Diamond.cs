using System;
using System.Collections.Generic;
using System.Text;

namespace MedicalDiamondSearch.Core.Helpers
{
    public abstract class Diamond
    {
        protected Diamond(PixelBlock referentBlock, PixelBlock centerBlock)
        {
            ReferentBlock = referentBlock;
            CenterBlock = centerBlock;
        }

        public PixelBlock ReferentBlock { get; }
        public PixelBlock CenterBlock { get; }
        public List<PixelBlock> Blocks { get; protected set; }
        public bool IsCenterBlock(PixelBlock block) => CenterBlock.Equals(block);
    }
}
