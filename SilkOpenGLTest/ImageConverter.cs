using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
namespace SilkOpenGLTest
{
    internal class ImageConverter
    {
        public static byte[] ImageToByteArray(string filePath)
        {
            //using (MemoryStream ms = new MemoryStream())
            //{
            //    using (Image image = Image.FromFile(filePath))
            //    {
            //        image.Save(ms, image.RawFormat);
            //        return ms.ToArray();
            //    }
            //}
            using (Bitmap bitmap = new Bitmap(filePath))
            {
                // Lock the bitmap data
                BitmapData raw = bitmap.LockBits(
                    new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                    ImageLockMode.ReadOnly,
                    PixelFormat.Format24bppRgb); // Specify the desired raw pixel format

                try
                {
                    // Calculate size of the raw data
                    int size = raw.Height * raw.Stride;
                    byte[] rawImage = new byte[size];

                    // Copy the image data into the byte array
                    Marshal.Copy(raw.Scan0, rawImage, 0, size);

                    for (int i = 0; i < size; i += 3)
                    {
                        byte temp = rawImage[i];
                        rawImage[i] = rawImage[i + 2];
                        rawImage[i + 2] = temp;
                    }

                   
                    return rawImage;
                }
                finally
                {
                    // Unlock the bits
                    bitmap.UnlockBits(raw);
                }
            }
        }

        public static (int, int) GetImageDimensions(string filePath)
        {
            using (Image image = Image.FromFile(filePath))
            {
                int width = image.Width;
                int height = image.Height;

                return (width, height);
            }
        }
    }
}
