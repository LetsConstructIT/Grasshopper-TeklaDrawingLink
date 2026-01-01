using System.Collections.Generic;
using System;
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

        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            var keys = new HashSet<TKey>();
            foreach (var element in source)
            {
                if (keys.Contains(keySelector(element))) continue;
                keys.Add(keySelector(element));
                yield return element;
            }
        }
    }
}
