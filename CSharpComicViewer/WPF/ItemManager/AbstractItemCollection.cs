using CSharpComicLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpComicViewer.WPF.ItemManager
{
    public class PropertyBind
    {
        public string Binding { get; }
        public string DisplayName { get; }
        public PropertyBind(string binding, string displayName)
        {
            Binding = binding;
            DisplayName = displayName;
        }
    }
    public class ToggleDeleteItem<T>
    {
        public bool Delete { get; set; }
        public T Item { get; }
        public ToggleDeleteItem(T item)
        {
            Delete = false;
            Item = item;
        }
    }
    public interface IItemCollectionBinding
    {
        IEnumerable<PropertyBind> GetPropertyBinds();
        object DataContext { get; }
        void RemoveDelete();
    }
    public abstract class AbstractItemCollection<T> : IItemCollectionBinding
    {
        public AbstractItemCollection(List<T> bookmarks)
        {
            deletableList = new List<ToggleDeleteItem<T>>();
            foreach (T bookmark in bookmarks)
            {
                deletableList.Add(new ToggleDeleteItem<T>(bookmark));
            }

            this.bookmarks = bookmarks;
        }

        private List<T> bookmarks;
        private List<ToggleDeleteItem<T>> deletableList;

        public object DataContext { get => deletableList; }
        public void RemoveDelete()
        {
            foreach (ToggleDeleteItem<T> item in deletableList)
                if (item.Delete)
                    bookmarks.Remove(item.Item);
        }

        public abstract IEnumerable<PropertyBind> GetPropertyBinds();
    }
}
