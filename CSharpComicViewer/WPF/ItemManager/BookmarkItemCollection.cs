using CSharpComicLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpComicViewer.WPF.ItemManager
{
    public class BookmarkItemCollection : AbstractItemCollection<Bookmark>
    {
        public BookmarkItemCollection(List<Bookmark> bookmarks) : base(bookmarks) { }

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
