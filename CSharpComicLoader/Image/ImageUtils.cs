//-------------------------------------------------------------------------------------
//  Copyright 2012 Rutger Spruyt
//
//  This file is part of C# Comicviewer.
//
//  csharp comicviewer is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  csharp comicviewer is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with csharp comicviewer.  If not, see <http://www.gnu.org/licenses/>.
//-------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows;


namespace CSharpComicLoader
{
    /// <summary>
    /// Utilities used on an image.
    /// </summary>
    public static class ImageUtils
    {

        /// <summary>
        /// Convert byte array to a bitmapimage.
        /// </summary>
        /// <param name="imageAsBytes">The byte array</param>
        /// <returns></returns>
        public static BitmapImage ConverToBitmapImage (byte[] imageAsBytes)
		{
            BitmapImage bi = new BitmapImage();

            try
            {
                bi.CacheOption = BitmapCacheOption.OnLoad;
                MemoryStream ms = new MemoryStream(imageAsBytes);
                ms.Position = 0;
                bi.BeginInit();
                bi.StreamSource = ms;
                bi.EndInit();
                bi.Freeze();
            }
            catch
            {
                try
                {
                    //If it fails the normal way try it again with a convert, possible quality loss.
                    System.Drawing.ImageConverter ic = new System.Drawing.ImageConverter();
                    System.Drawing.Image img = (System.Drawing.Image)ic.ConvertFrom(imageAsBytes);
                    System.Drawing.Bitmap bitmap1 = new System.Drawing.Bitmap(img);
                    MemoryStream ms = new MemoryStream();
                    bitmap1.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                    ms.Position = 0;
                    bi = new BitmapImage();
                    bi.CacheOption = BitmapCacheOption.OnLoad;
                    bi.BeginInit();
                    bi.StreamSource = ms;
                    bi.EndInit();
                }
                catch
                {
					throw new Exception("Could not load image.");
                }
            }

            return bi;
        }


        /// <summary>
        /// Gets the mode of a Color[]
        /// </summary>
        /// <param name="colors">Array of Colors</param>
        /// <returns>Index of mode, -1 if non found</returns>
        private static int GetModeOfColorArray(Color[] colors)
        {
            Color[] distinctcolors = colors.Distinct().ToArray();
            int[] countcolors = new int[distinctcolors.Length];
            int highest = 1;
            int highestindex = -1;
            Boolean mode = false;

            //count how many time distinct values are in colors
            for (int i = 0; i < distinctcolors.Length; i++)
            {
                for (int x = 0; x < colors.Length; x++)
                {
                    if (colors[x] == distinctcolors[i])
                        countcolors[i]++;
                }
            }
            //check what the highest value is
            for (int i = 0; i < countcolors.Length; i++)
            {
                if (countcolors[i] > highest)
                {
                    highest = countcolors[i];
                    highestindex = i;
                    mode = true;
                }
            }


            if (mode)
                return Array.IndexOf(colors, distinctcolors[highestindex]);
            else
                return -1;
        }

        /// <summary> 
        /// <para> Get the color of a certain pixel from a bitmapsource. </para>
        /// <para> copied from: </para>
        /// <para> https://stackoverflow.com/questions/1176910/finding-specific-pixel-colors-of-a-bitmapimage </para>
        /// </summary>
        /// <param name="source"> The bitmapsource</param>
        /// <param name="x"> The pixel-coordinate of x </param>
        /// <param name="y"> The pixel-coordinate of y </param>
        /// <returns></returns>
        private static Color GetPixelColor(BitmapSource source, int x, int y)
        {
            Color c = Colors.White;
            if (source != null)
            {
                try
                {
                    CroppedBitmap cb = new CroppedBitmap(source, new Int32Rect(x, y, 1, 1));
                    var pixels = new byte[4];
                    cb.CopyPixels(pixels, 4, 0);
                    c = Color.FromRgb(pixels[2], pixels[1], pixels[0]);
                }
                catch (Exception) { }
            }
            return c;
        }

        /// <summary>
		/// Gets the color of the background.
		/// </summary>
		/// <value>
		/// The color of the background.
		/// </value>
        /// <param name="image">The image.</param>
        public static Brush GetBackgroundColor(BitmapSource image)
		{
			if (image == null) return null;


            //Bitmap objBitmap = new Bitmap(image);

            int DividedBy = 100;
            Color[] colors = new Color[DividedBy * 4];

            //get the color of a pixels at the edge of image
            int i = 0;

            //left
            for (int y = 0; y < DividedBy; y++)
            {
                //Colors[i++] = objBitmap.GetPixel(0, y * (objBitmap.Height / DividedBy));
                colors[i++] = GetPixelColor(image, 0, y * (image.PixelHeight / DividedBy));
            }

            //top
            for (int x = 0; x < DividedBy; x++)
            {

                //Colors[i++] = objBitmap.GetPixel(x * (objBitmap.Width / DividedBy), 0);
                colors[i++] = GetPixelColor(image, x * (image.PixelWidth / DividedBy), 0);
            }

            //right
            for (int y = 0; y < DividedBy; y++)
            {
                //Colors[i++] = objBitmap.GetPixel(objBitmap.Width - 1, y * (objBitmap.Height / DividedBy));
                colors[i++] = GetPixelColor(image, image.PixelWidth - 1, y * (image.PixelHeight / DividedBy));
            }

            //bottom
            for (int x = 0; x < DividedBy; x++)
            {

               //Colors[i++] = objBitmap.GetPixel(x * (objBitmap.Width / DividedBy), objBitmap.Height - 1);
                colors[i++] = GetPixelColor(image, x * (image.PixelWidth / DividedBy), image.PixelHeight - 1);
            }
            //get mode of colors
            int Color = GetModeOfColorArray(colors);


            //set bgcolor
            Color BackColor = Colors.White;
            if (Color != -1)
            {
                BackColor = colors[Color];
            }

            return new SolidColorBrush(BackColor);
        }



        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="viewportSize">The size of viewport.</param>
        /// <param name="imageMode">The way of handling image.</param>
        /// <param name="interpolationMode">The interpolation mode.</param>
        public static TransformedBitmap ResizeImage(BitmapSource image, Size viewportSize, ImageMode imageMode)
        {
            if (image == null) return null;


            //ImageMode imageMode
            double sourceWidth = image.Width;
            double sourceHeight = image.Height;

            double zoomW = 1, zoomH = 1;
            // Different for unison scaled or two-way scaled
            if (imageMode == ImageMode.FitToScreen)
            {
                zoomW = viewportSize.Width / sourceWidth;
                zoomH = viewportSize.Height / sourceHeight;
            }
            else
            {
                // For unison scaled
                double zoom = 1;
                switch (imageMode)
                {
                    case ImageMode.FitToHeight:
                        zoom = viewportSize.Height / sourceHeight;
                        break;

                    case ImageMode.FitToWidth:
                        zoom = viewportSize.Width / sourceWidth;
                        break;

                    case ImageMode.FitToShort:
                        zoom = (viewportSize.Height < viewportSize.Width) ?
                                viewportSize.Height / sourceHeight : viewportSize.Width / sourceWidth;
                        break;

                    case ImageMode.FitToShortScaled:
                        zoom = Math.Max(viewportSize.Height / sourceHeight,
                                        viewportSize.Width / sourceWidth);
                        break;

                    case ImageMode.Normal:
                        zoom = 1;
                        break;
                }
                zoomW = zoom;
                zoomH = zoom;
            }

            return new TransformedBitmap(image, new ScaleTransform(zoomW, zoomH));
        }

    }
}
