using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OCMClip.ClipHandler.Entities;

namespace OCMClip.ClipHandler
{
    public static class ConvertImage
    {
        public static Bitmap ResizeImage(byte[] imageByteArray, int width, int height)
        {
            return ResizeImage(ByteArrayToImage(imageByteArray), width, height);
        }

        public static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        public static byte[] ImageToByteArray(System.Drawing.Image imageIn, Enums.ImageFormatType formatType)
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

        internal static ImageFormat GetImageFormat(Enums.ImageFormatType format)
        {
            switch (format)
            {
                case Enums.ImageFormatType.Bmp:
                    return ImageFormat.Bmp;
                case Enums.ImageFormatType.Emf:
                    return ImageFormat.Emf;
                case Enums.ImageFormatType.Exif:
                    return ImageFormat.Exif;
                case Enums.ImageFormatType.Gif:
                    return ImageFormat.Gif;
                case Enums.ImageFormatType.Icon:
                    return ImageFormat.Icon;
                case Enums.ImageFormatType.Jpeg:
                    return ImageFormat.Jpeg;
                case Enums.ImageFormatType.Tiff:
                    return ImageFormat.Tiff;
                case Enums.ImageFormatType.Wmf:
                    return ImageFormat.Wmf;
                case Enums.ImageFormatType.Png:
                default:
                    return ImageFormat.Png;
            }
        }
    }
}
