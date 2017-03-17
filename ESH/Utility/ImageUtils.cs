// Title:  Image manipulation
// Author: Emily Heiner
//
// MIT License
// Copyright(c) 2017 Emily Heiner (emilysamantha80@gmail.com)
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using System.IO;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing;

namespace ESH.Utility
{
    public static class ImageUitls
    {
        /// <summary>
        /// Resizes images to a specific System.Drawing.Size, with the option to preserve
        /// the aspect ratio of the image.
        /// </summary>
        /// <param name="image">System.Drawing.Image to resize</param>
        /// <param name="size">System.Drawing.Size for the new image</param>
        /// <param name="preserveAspectRatio">Whether or not to preserve the aspect ratio</param>
        /// <param name="frameToSize">Whether or not to include a white frame. Can be used when the new size is not the same aspect ratio as the old size.</param>
        /// <returns>System.Drawing.Image object containing the resized image</returns>
        public static Image ResizeImage(Image image, Size size, bool preserveAspectRatio = true, bool frameToSize = false)
        {
            int newWidth;
            int newHeight;
            if (preserveAspectRatio)
            {
                int originalWidth = image.Width;
                int originalHeight = image.Height;
                float percentWidth = (float)size.Width / (float)originalWidth;
                float percentHeight = (float)size.Height / (float)originalHeight;
                float percent = percentHeight < percentWidth ? percentHeight : percentWidth;
                newWidth = (int)(originalWidth * percent);
                newHeight = (int)(originalHeight * percent);
            }
            else
            {
                newWidth = size.Width;
                newHeight = size.Height;
            }
            Image newImage;
            if (frameToSize)
            {
                newImage = new Bitmap(size.Width, size.Height);
            }
            else
            {
                newImage = new Bitmap(newWidth, newHeight);
            }
            using (Graphics graphicsHandle = Graphics.FromImage(newImage))
            {
                graphicsHandle.FillRectangle(Brushes.White, 0, 0, newImage.Width, newImage.Height);
                graphicsHandle.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphicsHandle.DrawImage(image, (newImage.Width - newWidth) / 2, (newImage.Height - newHeight) / 2, newWidth, newHeight);
            }
            return newImage;
        }

        /// <summary>
        /// Takes a System.Drawing.Image object and converts it to a new System.Drawing.Image object.
        /// Useful if the ImageToByteArray() function doesn't work.
        /// </summary>
        /// <param name="imageIn"></param>
        /// <returns>System.Drawing.Image object</returns>
        public static Image ImageToNewImage(Image imageIn)
        {
            Image newImage;
            newImage = new Bitmap(imageIn.Width, imageIn.Height);
            using (Graphics graphicsHandle = Graphics.FromImage(newImage))
            {
                graphicsHandle.DrawImage(imageIn, 0, 0, imageIn.Width, imageIn.Height);
            }
            return newImage;
        }

        /// <summary>
        /// Gets the System.Drawing.Imaging.ImageCodecInfo object for a specified System.Drawing.Imaging.ImageFormat
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        public static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }

        /// <summary>
        /// Takes a System.Drawing.Image byte array and converts it to a byte array of a different format.
        /// Can be used with File.WriteAllBytes(string, byte[]) to do image format conversion.
        /// </summary>
        /// <param name="byteArrayIn"></param>
        /// <param name="imageFormat">System.Drawing.Imaging.ImageFormat to save as</param>
        /// <param name="jpegQuality"></param>
        /// <returns>byte[] containing the converted byte array</returns>
        public static byte[] ImageByteArrayToImageByteArray(byte[] byteArrayIn, ImageFormat imageFormat, Int64 jpegQuality = 80)
        {
            using (Image tempImage = Image.FromStream(new MemoryStream(byteArrayIn)))
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    if (imageFormat == ImageFormat.Jpeg)
                    {
                        ImageCodecInfo jpgEncoder = GetEncoder(ImageFormat.Jpeg);
                        System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;
                        EncoderParameter encoderParameter = new EncoderParameter(myEncoder, jpegQuality);
                        EncoderParameters encoderParameters = new EncoderParameters(1);
                        encoderParameters.Param[0] = encoderParameter;
                        tempImage.Save(ms, jpgEncoder, encoderParameters);
                    }
                    else
                    {
                        tempImage.Save(ms, imageFormat);
                    }
                    return ms.ToArray();
                }
            }
        }

        /// <summary>
        /// Converts from a System.Drawing.Image object to a System.Drawing.Image byte array in the specified System.Drawing.Imaging.ImageFormat
        /// </summary>
        /// <param name="imageIn">System.Drawing.Image object to convert</param>
        /// <param name="imageFormat">System.Drawing.Imaging.ImageFormat to save as</param>
        /// <param name="jpegQuality">Quality of the JPEG image, as an integer 0-100. Ignored if imageFormat is not ImageFormat.Jpeg</param>
        /// <returns>byte[] containing the contents of the System.Drawing.Image object</returns>
        public static byte[] ImageToImageByteArray(Image imageIn, ImageFormat imageFormat, Int64 jpegQuality = 80)
        {

            using (MemoryStream ms = new MemoryStream())
            {
                if (imageFormat == ImageFormat.Jpeg)
                {
                    ImageCodecInfo jpgEncoder = GetEncoder(ImageFormat.Jpeg);
                    System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;
                    EncoderParameter encoderParameter = new EncoderParameter(myEncoder, jpegQuality);
                    EncoderParameters encoderParameters = new EncoderParameters(1);
                    encoderParameters.Param[0] = encoderParameter;
                    imageIn.Save(ms, jpgEncoder, encoderParameters);
                }
                else
                {
                    imageIn.Save(ms, imageFormat);
                }
                return ms.ToArray();
            }
        }

        /// <summary>
        /// Converts from a System.Drawing.Image byte array to a System.Drawing.Bitmap object
        /// </summary>
        /// <param name="byteArrayIn">Byte array of the System.Drawing.Image object to convert</param>
        /// <returns>System.Drawing.Image object</returns>
        public static Image ImageByteArrayToImage(byte[] byteArrayIn)
        {
            using (MemoryStream ms = new MemoryStream(byteArrayIn))
            {
                Image returnImage = Bitmap.FromStream(ms);
                return returnImage;
            }
        }

        /// <summary>
        /// Converts from a System.Drawing.Bitmap object to a System.Drawing.Bitmap byte array in the specified System.Drawing.Imaging.ImageFormat
        /// </summary>
        /// <param name="imageIn">System.Drawing.Bitmap object to convert</param>
        /// <param name="imageFormat">System.Drawing.Imaging.ImageFormat to save as</param>
        /// <param name="jpegQuality">Quality of the JPEG image, as an integer 0-100. Ignored if imageFormat is not ImageFormat.Jpeg</param>
        /// <returns>byte[] containing the contents of the System.Drawing.Bitmap object</returns>
        public static byte[] BitmapToBitmapByteArray(Bitmap imageIn, ImageFormat imageFormat, Int64 jpegQuality = 80)
        {

            using (MemoryStream ms = new MemoryStream())
            {
                if (imageFormat == ImageFormat.Jpeg)
                {
                    ImageCodecInfo jpgEncoder = GetEncoder(ImageFormat.Jpeg);
                    System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;
                    EncoderParameter encoderParameter = new EncoderParameter(myEncoder, jpegQuality);
                    EncoderParameters encoderParameters = new EncoderParameters(1);
                    encoderParameters.Param[0] = encoderParameter;
                    imageIn.Save(ms, jpgEncoder, encoderParameters);
                }
                else
                {
                    imageIn.Save(ms, imageFormat);
                }
                return ms.ToArray();
            }
        }

        /// <summary>
        /// Converts from a System.Drawing.Bitmap byte array to a System.Drawing.Image object
        /// </summary>
        /// <param name="byteArrayIn">Byte array of the System.Drawing.Bitmap object to convert</param>
        /// <returns>System.Drawing.Image object</returns>
        public static Image BitmapByteArrayToImage(byte[] byteArrayIn)
        {
            using (MemoryStream ms = new MemoryStream(byteArrayIn))
            {
                Image returnImage = Bitmap.FromStream(ms);
                return returnImage;
            }
        }
    }
}