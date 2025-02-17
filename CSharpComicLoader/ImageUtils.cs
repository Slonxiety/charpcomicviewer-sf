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
using System.Drawing;
using System.Windows.Media;
using System.Drawing.Drawing2D;
using CSharpComicViewer.Configuration;
using System.Security.Principal;


namespace CSharpComicLoader
{
	/// <summary>
	/// Utilities used on an image.
	/// </summary>
	public class ImageUtils
	{
		private Image image;
		private Point LocationImage = new Point(0, 0);
		private System.Drawing.Color BackColor;
		private MemoryStream ms;

		/// <summary>
		/// Initializes a new instance of the <see cref="ComicPage"/> class.
		/// </summary>
		public ImageUtils()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ComicPage"/> class.
		/// </summary>
		/// <param name="Image">The image.</param>
		public ImageUtils(byte[] image)
		{
			ObjectValue = image;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ComicPage"/> class.
		/// </summary>
		/// <param name="Image">The image.</param>
		public ImageUtils(Image image)
		{
			ObjectValue = image;
		}

		/// <summary>
		/// Sets the object value.
		/// </summary>
		/// <value>
		/// The object value.
		/// </value>
		public Object ObjectValue
		{
			set
			{
				if (value is Image)
				{
					image = value as Image;
				}
				else if (value is byte[])
				{
					image = ConvertByteArrayToImage(value as byte[]);
				}
			}
		}


		/// <summary>
		/// Gets the object value as bytes.
		/// </summary>
		public byte[] ObjectValueAsBytes
		{
			get
			{
				if (image != null)
				{
					return ConvertImageToByteArray(image);
				}
				else
				{
					return null;
				}
			}
		}

		/// <summary>
		/// Gets the object value as image.
		/// </summary>
		public Image ObjectValueAsImage
		{
			get
			{
				if (image != null)
				{
					return image;
				}
				else
				{
					return null;
				}
			}
		}

		/// <summary>
		/// Gets or sets the height of the screen.
		/// </summary>
		/// <value>
		/// The height of the screen.
		/// </value>
		public int ScreenHeight { get; set; }

		/// <summary>
		/// Gets or sets the width of the screen.
		/// </summary>
		/// <value>
		/// The width of the screen.
		/// </value>
		public int ScreenWidth { get; set; }

		/// <summary>
		/// Convert a byte array to an image.
		/// </summary>
		/// <param name="image">The image as byte array.</param>
		/// <returns></returns>
		private Image ConvertByteArrayToImage(byte[] image)
		{
			ms = new MemoryStream(image);
			Image returnImage = Image.FromStream(ms);
			return returnImage;
		}

		/// <summary>
		/// Convert an image to a byte array.
		/// </summary>
		/// <param name="image">The image.</param>
		/// <returns></returns>
		private byte[] ConvertImageToByteArray(System.Drawing.Image image)
		{
			MemoryStream ms = new MemoryStream();
			image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
			byte[] ReturnValue = ms.ToArray();
			ms.Close();
			return ReturnValue;
		}


		/// <summary>
		/// Gets the color of the background.
		/// </summary>
		/// <value>
		/// The color of the background.
		/// </value>
		public System.Windows.Media.Brush BackgroundColor
		{
			get
			{
				if (image != null)
				{
					Bitmap objBitmap = new Bitmap(image);

					int DividedBy = 100;
					System.Drawing.Color[] Colors = new System.Drawing.Color[DividedBy * 4];

					//get the color of a pixels at the edge of image
					int i = 0;

					//left
					for (int y = 0; y < DividedBy; y++)
					{
						Colors[i++] = objBitmap.GetPixel(0, y * (objBitmap.Height / DividedBy));
					}

					//top
					for (int x = 0; x < DividedBy; x++)
					{

						Colors[i++] = objBitmap.GetPixel(x * (objBitmap.Width / DividedBy), 0);
					}

					//right
					for (int y = 0; y < DividedBy; y++)
					{
						Colors[i++] = objBitmap.GetPixel(objBitmap.Width - 1, y * (objBitmap.Height / DividedBy));
					}

					//bottom
					for (int x = 0; x < DividedBy; x++)
					{

						Colors[i++] = objBitmap.GetPixel(x * (objBitmap.Width / DividedBy), objBitmap.Height - 1);
					}
					//get mode of colors
					int Color = GetModeOfColorArray(Colors);
					//set bgcolor

					if (Color != -1)
					{
						BackColor = Colors[Color];
					}

					System.Windows.Media.Color BackColorWPF = new System.Windows.Media.Color();
					BackColorWPF.A = BackColor.A;
					BackColorWPF.B = BackColor.B;
					BackColorWPF.G = BackColor.G;
					BackColorWPF.R = BackColor.R;
					return new SolidColorBrush(BackColorWPF);
				}
				else
				{
					return null;
				}
			}
		}

		/// <summary>
		/// Gets the mode of a Color[]
		/// </summary>
		/// <param name="colors">Array of Colors</param>
		/// <returns>Index of mode, -1 if non found</returns>
		private int GetModeOfColorArray(System.Drawing.Color[] colors)
		{
			System.Drawing.Color[] distinctcolors = colors.Distinct().ToArray();
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
		/// Disposes the of memory stream.
		/// </summary>
		public void DisposeOfMemoryStream()
		{
			if (ms != null)
			{
				ms.Close();
			}
		}


        /// <summary>
        /// Resizes the image.
        /// </summary>
        /// <param name="viewportSize">The size of viewport.</param>
        /// <param name="imageMode">The way of handling image.</param>
        public void ResizeImage(SizeF viewportSize, ImageMode imageMode)
		{
			ResizeImage(viewportSize, imageMode, InterpolationMode.HighQualityBicubic);
		}

        /// <summary>
        /// Resizes the image.
        /// </summary>
        /// <param name="viewportSize">The size of viewport.</param>
        /// <param name="imageMode">The way of handling image.</param>
        /// <param name="interpolationMode">The interpolation mode.</param>
        public void ResizeImage(SizeF viewportSize, ImageMode imageMode, InterpolationMode interpolationMode)
		{
			if (image != null)
			{
                //ImageMode imageMode
                int sourceWidth = image.Width;
				int sourceHeight = image.Height;

				float zoomW = 1, zoomH = 1;
				// Different for unison scaled or two-way scaled
				if (imageMode == ImageMode.FitToScreen)
				{
					zoomW = viewportSize.Width / (float)sourceWidth;
					zoomH = viewportSize.Height / (float)sourceHeight;
                }
				else
                {
                    // For unison scaled
                    float zoom = 1;
                    switch (imageMode)
                    {
                        case ImageMode.FitToHeight:
                            zoom = viewportSize.Height / (float)sourceHeight;
                            break;

                        case ImageMode.FitToWidth:
                            zoom = viewportSize.Width / (float)sourceWidth;
                            break;

						case ImageMode.FitToShort:
							zoom = (viewportSize.Height < viewportSize.Width) ?
									viewportSize.Height / (float)sourceHeight : viewportSize.Width / (float)sourceWidth;
							break;

						case ImageMode.FitToShortScaled:
							zoom = Math.Max(viewportSize.Height / (float)sourceHeight,
											viewportSize.Width / (float)sourceWidth);
                            break;

						case ImageMode.Normal:
							zoom = 1;
							break;
                    }
					zoomW = zoom;
					zoomH = zoom;
                }

				int destWidth = (int)(sourceWidth * zoomW);
				int destHeight = (int)(sourceHeight * zoomH);

				Bitmap b = new Bitmap(destWidth, destHeight);
				Graphics g = Graphics.FromImage((Image)b);
				g.InterpolationMode = interpolationMode;

				g.DrawImage(image, 0, 0, destWidth, destHeight);
				g.Dispose();

				image = (Image)b;
			}
		}
	}
}
