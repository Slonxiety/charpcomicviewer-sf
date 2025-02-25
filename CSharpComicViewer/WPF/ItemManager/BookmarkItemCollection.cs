using CSharpComicLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSharpComicLoader.OldFileStructure;

namespace CSharpComicViewer.WPF.ItemManager
{
    /// <summary>
    /// Implementation of <see cref="AbstractItemCollection{T}"/>, used to manage items of <see cref="Bookmark"/>
    /// </summary>
    public class BookmarkItemCollection : AbstractItemCollection<Bookmark>
    {
        /// <summary>
        /// Initiates an instance of BookmarkItemCollection.
        /// </summary>
        /// <param name="bookmarks"></param>
        public BookmarkItemCollection(List<Bookmark> bookmarks) : base(bookmarks) { }

        /// <summary>
        /// The list of propertybinds of <see cref="Bookmark"/>
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<PropertyBind> GetPropertyBinds()
        {
            return new List<PropertyBind>()
            {
                new PropertyBind(binding: "CurrentFileName", displayName: "Current filename"),
                new PropertyBind(binding: "PageNumber", displayName: "Page number"),
                new PropertyBind(binding: "Files.Length", displayName: "Number of files"),
                new PropertyBind(binding: "CurrentFileDirectoryLocation", displayName: "Current file location"),
            };
        }
    }
}
