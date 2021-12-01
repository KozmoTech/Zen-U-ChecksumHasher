using Microsoft.UI.Xaml.Media.Imaging;

namespace KozmoTech.ZenUtility.ChecksumHasher;

internal sealed class BitmapPicture : IPicture
{
    public BitmapPicture(BitmapImage image) => Image = image;

    public BitmapImage Image { get; }
    public int Width => Image.PixelWidth;
    public int Height => Image.PixelHeight;
}
