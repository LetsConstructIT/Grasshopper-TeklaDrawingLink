using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;

namespace DrawingLink.UI
{
    public static class Extensions
    {
        public static BitmapImage ToBitmapImage(this Bitmap bitmap)
        {
            using var memory = new MemoryStream();
            bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Png);
            memory.Position = 0;
            var bitmapimage = new BitmapImage();
            bitmapimage.BeginInit();
            bitmapimage.StreamSource = memory;
            bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapimage.EndInit();

            return bitmapimage;
        }

        public static bool TryDequeue<T>(this Queue<T> queue, out T result)
        {
            if (queue.Count == 0)
            {
                result = default;
                return false;
            }

            result = queue.Dequeue();
            return true;
        }
    }
}
