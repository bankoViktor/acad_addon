using System;
using System.Drawing;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace Addon.Extensions
{
    public static class BitmapExtensions
    {
        public static BitmapSource ToBitmapSource(this Bitmap bitmap)
        {
            var hIcon = bitmap.GetHbitmap();
            var bmSize = BitmapSizeOptions.FromWidthAndHeight(bitmap.Width, bitmap.Height);
            return Imaging.CreateBitmapSourceFromHBitmap(hIcon, IntPtr.Zero, System.Windows.Int32Rect.Empty, bmSize);
        }
    }
}
