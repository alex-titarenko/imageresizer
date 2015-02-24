using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;


namespace TAlex.ImageProxy
{
    public static class ImageHelper
    {
        public static BitmapFrame ReadBitmapFrame(Stream imageStream)
        {
            BitmapDecoder bdDecoder = BitmapDecoder.Create(imageStream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.None);
            return bdDecoder.Frames[0];
        }
    }
}