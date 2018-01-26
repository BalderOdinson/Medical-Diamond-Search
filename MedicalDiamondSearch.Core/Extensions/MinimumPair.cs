using System;
using System.Collections.Generic;
using System.Text;
using MedicalDiamondSearch.Core.Helpers;

namespace MedicalDiamondSearch.Core.Extensions
{
    public struct MinimumPair
    {
        public MinimumPair(PixelBlock minimum, PixelBlock secondMinimum, double treshold)
        {
            Minimum = minimum;
            SecondMinimum = secondMinimum;
            Treshold = treshold;
        }

        public PixelBlock Minimum { get; }
        public PixelBlock SecondMinimum { get; }
        public double Treshold { get; }  
    }
}
