using System.Drawing;
using MedicalDiamondSearch.Core.Helpers;

namespace MedicalDiamondSearch.Core.Extensions
{
    public static class SearchWindow
    {
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
