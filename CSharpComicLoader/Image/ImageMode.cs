using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpComicViewer.Configuration
{
    /// <summary>
	/// Mode of default handling when showing an image.
	/// </summary>
	public enum ImageMode
    {
        /// <summary> Show the image without any scaling. </summary>
        Normal,
        /// <summary> Align image's height to form's height. </summary>
        FitToHeight,
        /// <summary> Align image's width to form's width. </summary>
        FitToWidth,
        /// <summary> Align image's height and width to form's height and width. </summary>
        FitToScreen,
        /// <summary> Find the short side of the image, and align it to the screen. </summary>
        FitToShort,
        /// <summary> Find the short side of the image proportion to the screen, and align it to the screen. </summary>
        FitToShortScaled,
    }
}
