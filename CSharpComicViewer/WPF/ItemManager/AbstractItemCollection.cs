using CSharpComicLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpComicViewer.WPF.ItemManager
{
    /// <summary>
    /// Used to indicate binding of a field from an type, and the displayed name.
    /// </summary>
    public class PropertyBind
    {
        /// <summary>
        /// Get the field name
        /// </summary>
        public string Binding { get; }
        /// <summary>
        /// Get the displayed name
        /// </summary>
        public string DisplayName { get; }
        /// <summary>
        /// Initializes a new instance of PropertyBind class.
        /// </summary>
        /// <param name="binding">The field name</param>
        /// <param name="displayName">The displayed name</param>
        public PropertyBind(string binding, string displayName)
        {
            Binding = binding;
            DisplayName = displayName;
        }
    }

    /// <summary>
    /// A wrap up class to attach a boolean field "Delete" to an item
    /// </summary>
    /// <typeparam name="T">The item type</typeparam>
    public class ToggleDeleteItem<T>
    {
        /// <summary>
        /// Get or set the boolean field Delete
        /// </summary>
        public bool Delete { get; set; }
        /// <summary>
        /// Get the item
        /// </summary>
        public T Item { get; }
        /// <summary>
        /// Initializes an instance of ToggleDeleteItem.
        /// </summary>
        /// <param name="item">The item to be attached</param>
        public ToggleDeleteItem(T item)
        {
            Delete = false;
            Item = item;
        }
    }

    /// <summary>
    /// <para> An interface defined to use for <see cref="ItemManager"/>. It defines the required functions for the manager to work. </para> 
    /// <para> NOTE: It's suggested to inherit <see cref="AbstractItemCollection{T}"/> instead of inherit from this interface. </para>
    /// </summary>
    public interface IItemCollectionBinding
    {
        /// <summary>
        /// Enumerable of all the properties binded. <see cref="ItemManager"/> would create a column of each property.
        /// </summary>
        /// <returns></returns>
        IEnumerable<PropertyBind> GetPropertyBinds();
        /// <summary>
        /// The data context for xaml binding.
        /// </summary>
        object DataContext { get; }
        /// <summary>
        /// Define the action to apply removal to the actual data.
        /// </summary>
        void RemoveDelete();
    }

    /// <summary>
    /// <para> An abstract class with almost all requirement for <see cref="IItemCollectionBinding"/> fulfilled. </para>
    /// <para> Inherit from this and override the remaining functions.</para>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class AbstractItemCollection<T> : IItemCollectionBinding
    {
        /// <summary>
        /// Initializes the item collection.
        /// </summary>
        /// <param name="items">The list of items to be managed.</param>
        public AbstractItemCollection(List<T> items)
        {
            deletableList = new List<ToggleDeleteItem<T>>();
            foreach (T item in items)
            {
                deletableList.Add(new ToggleDeleteItem<T>(item));
            }

            this.bookmarks = items;
        }

        private List<T> bookmarks;
        private List<ToggleDeleteItem<T>> deletableList;

        /// <summary>
        /// Get the object used for managing.
        /// </summary>
        public object DataContext { get => deletableList; }
        /// <summary>
        /// Implement the actual delete process.
        /// </summary>
        public void RemoveDelete()
        {
            foreach (ToggleDeleteItem<T> item in deletableList)
                if (item.Delete)
                    bookmarks.Remove(item.Item);
        }

        /// <summary>
        /// Used to indicate the bindings and the displayed names.
        /// </summary>
        /// <returns> The list of properybinds. </returns>
        public abstract IEnumerable<PropertyBind> GetPropertyBinds();
    }
}
