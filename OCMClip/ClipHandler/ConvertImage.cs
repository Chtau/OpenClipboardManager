using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCMClip.ClipHandler
{
    internal static class ConvertImage
    {
        public enum ImageFormatType
        {
            Png = 0,
            Bmp = 1,
            Emf = 2,
            Exif = 3,
            Gif = 4,
            Icon = 5,
            Jpeg = 6,
            Tiff = 7,
            Wmf = 8
        }

        public static byte[] ImageToByteArray(System.Drawing.Image imageIn, ImageFormatType formatType)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                imageIn.Save(stream, GetImageFormat(formatType));
                return stream.ToArray();
            }
        }

        private static string GetMimeType(Image i)
        {
            var imgguid = i.RawFormat.Guid;
            foreach (ImageCodecInfo codec in ImageCodecInfo.GetImageDecoders())
            {
                if (codec.FormatID == imgguid)
                    return codec.MimeType;
            }
            return "image/unknown";
        }

        private static ImageFormat GetImageFormatFromGuid(Guid id)
        {
            if (id == ImageFormat.Bmp.Guid)
                return ImageFormat.Bmp;
            else if (id == ImageFormat.Emf.Guid)
                return ImageFormat.Emf;
            else if (id == ImageFormat.Exif.Guid)
                return ImageFormat.Exif;
            else if (id == ImageFormat.Gif.Guid)
                return ImageFormat.Gif;
            else if (id == ImageFormat.Icon.Guid)
                return ImageFormat.Icon;
            else if (id == ImageFormat.Jpeg.Guid)
                return ImageFormat.Jpeg;
            else if (id == ImageFormat.MemoryBmp.Guid)
                return ImageFormat.MemoryBmp;
            else if (id == ImageFormat.Png.Guid)
                return ImageFormat.Png;
            else if (id == ImageFormat.Tiff.Guid)
                return ImageFormat.Tiff;
            else if (id == ImageFormat.Wmf.Guid)
                return ImageFormat.Wmf;

            return ImageFormat.Png;
        }

        public static Image ByteArrayToImage(byte[] byteArrayIn)
        {
            using (MemoryStream ms = new MemoryStream(byteArrayIn))
            {
                Image returnImage = Image.FromStream(ms);
                return returnImage;
            }
        }

        public static ImageFormat GetImageFormat(ImageFormatType format)
        {
            switch (format)
            {
                case ImageFormatType.Bmp:
                    return ImageFormat.Bmp;
                case ImageFormatType.Emf:
                    return ImageFormat.Emf;
                case ImageFormatType.Exif:
                    return ImageFormat.Exif;
                case ImageFormatType.Gif:
                    return ImageFormat.Gif;
                case ImageFormatType.Icon:
                    return ImageFormat.Icon;
                case ImageFormatType.Jpeg:
                    return ImageFormat.Jpeg;
                case ImageFormatType.Tiff:
                    return ImageFormat.Tiff;
                case ImageFormatType.Wmf:
                    return ImageFormat.Wmf;
                case ImageFormatType.Png:
                default:
                    return ImageFormat.Png;
            }
        }
    }
}
