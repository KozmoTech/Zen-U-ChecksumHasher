using KozmoTech.ZenUtility.ChecksumHasher.Core;
using Microsoft.UI.Xaml.Media.Imaging;

namespace KozmoTech.ZenUtility.ChecksumHasher.UI;

internal sealed class BitmapPicture : IPicture
{
    public BitmapPicture(BitmapImage image) => Image = image;

    public BitmapImage Image { get; }
    public int Width => Image.PixelWidth;
    public int Height => Image.PixelHeight;
}
