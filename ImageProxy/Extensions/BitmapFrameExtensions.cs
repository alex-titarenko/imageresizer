using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Windows.Media;
using System.Windows.Media.Imaging;


namespace TAlex.ImageProxy.Extensions
{
    public static class BitmapFrameExtensions
    {
        public static BitmapFrame ResizeImage(this BitmapFrame bitmap, int maxWidth, int maxHeight)
        {
            int width = bitmap.PixelWidth;
            int height = bitmap.PixelHeight;

            if ((width <= maxWidth) && (height <= maxHeight))
            {
                return bitmap;
            }

            float scaleX = (maxWidth / (float)width);
            float scaleY = (maxHeight / (float)height);
            float scale = Math.Min(scaleX, scaleY);

            float newWidth = (int)(width * scale);
            float newHeight = (int)(height * scale);

            scaleY = newHeight / (float)height;
            scaleX = newWidth / (float)width;

            return Resize(bitmap, scaleX, scaleY);
        }

        public static void SaveToFile(this BitmapFrame frame, string path)
        {
            try
            {
                BitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(frame);

                using (Stream filestream = new FileStream(path, FileMode.Create, FileAccess.ReadWrite, FileShare.Read))
                {
                    encoder.Save(filestream);
                }
            }
            catch (Exception exc)
            {
                Trace.TraceError("An error was occured during saving frame. Error: {0}", exc);
            }
        }

        private static BitmapFrame Resize(BitmapFrame bitmap, double scaleX, double scaleY)
        {
            TransformedBitmap tbBitmap = new TransformedBitmap(bitmap, new ScaleTransform(scaleX, scaleY));
            return BitmapFrame.Create(tbBitmap);
        }
    }
}