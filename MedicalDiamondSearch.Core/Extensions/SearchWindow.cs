using System.Drawing;
using MedicalDiamondSearch.Core.Helpers;

namespace MedicalDiamondSearch.Core.Extensions
{
    public static class SearchWindow
    {
        /// <summary>
        /// Calculates if given block is inside search window(according to given center block) and if it is inside image.
        /// </summary>
        /// <param name="center"></param>
        /// <param name="pixelBlock"></param>
        /// <param name="parametar"></param>
        /// <param name="blockSize"></param>
        /// <param name="pictureWidth"></param>
        /// <param name="pictureHeight"></param>
        /// <returns></returns>
        public static bool IsInSearchWindow(this PixelBlock center, PixelBlock pixelBlock, int parametar, int blockSize, int pictureWidth, int pictureHeight)
        {
            return pixelBlock.Position.X >= 0 &&
                   pixelBlock.Position.X >= center.Position.X - parametar * blockSize &&
                   pixelBlock.Position.X <= center.Position.X + parametar * blockSize &&
                   pixelBlock.Position.X <= pictureWidth - blockSize &&
                   pixelBlock.Position.Y >= 0 &&
                   pixelBlock.Position.Y >= center.Position.Y - parametar * blockSize &&
                   pixelBlock.Position.Y <= center.Position.Y + parametar * blockSize &&
                   pixelBlock.Position.Y <= pictureHeight - blockSize;
        }

        /// <summary>
        /// Calculates if given block(calculated from point) is inside search window(according to given center block) and if it is inside image.
        /// </summary>
        /// <param name="center"></param>
        /// <param name="pixelBlockPoint"></param>
        /// <param name="parametar"></param>
        /// <param name="blockSize"></param>
        /// <param name="pictureWidth"></param>
        /// <param name="pictureHeight"></param>
        /// <returns></returns>
        public static bool IsInSearchWindow(this PixelBlock center, Point pixelBlockPoint, int parametar, int blockSize, int pictureWidth, int pictureHeight)
        {
            return pixelBlockPoint.X >= 0 &&
                   pixelBlockPoint.X >= center.Position.X - parametar * blockSize &&
                   pixelBlockPoint.X <= center.Position.X + parametar * blockSize &&
                   pixelBlockPoint.X <= pictureWidth - blockSize &&
                   pixelBlockPoint.Y >= 0 &&
                   pixelBlockPoint.Y >= center.Position.Y - parametar * blockSize &&
                   pixelBlockPoint.Y <= center.Position.Y + parametar * blockSize &&
                   pixelBlockPoint.Y <= pictureHeight - blockSize;
        }
    }
}
