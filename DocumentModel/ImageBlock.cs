using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace DocumentModel
{
	public sealed class ImageBlock : TextBlock
    {
        private byte[] _imageBytes;

        public ImageFormat ImageFormat { get; private set; }

        public int? Height { get; set; }
        public int? Width { get; set; }

        public int ImageHeight { get; private set; }
        public int ImageWidth { get; private set; }

        public ImageBlock(Image image) : this(image, string.Empty)
        {
        }

        public ImageBlock(Image image, string text) : base(text)
        {
            if (image == null) throw new ArgumentNullException("image");
            
            using (image)
            {
                ImageHeight = image.Height;
                ImageWidth = image.Width;

                using (var ms = new MemoryStream())
                {
                    image.Save(ms, image.RawFormat);
                    _imageBytes = ms.ToArray();
                    ImageFormat = image.RawFormat;
                }
            }
        }

        public ImageBlock(Image image, FormatedText text)
            : base(text)
        {
            if (image == null) throw new ArgumentNullException("image");

            using (image)
            {
                ImageHeight = image.Height;
                ImageWidth = image.Width;

                using (var ms = new MemoryStream())
                {
                    image.Save(ms, image.RawFormat);
                    _imageBytes = ms.ToArray();
                    ImageFormat = image.RawFormat;
                }
            }
        }

        public byte[] GetImageBuffer()
        {
            return _imageBytes;
        }
    }
}