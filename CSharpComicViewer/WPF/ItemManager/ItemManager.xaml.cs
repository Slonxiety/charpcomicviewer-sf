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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace CSharpComicViewer.WPF.ItemManager
{
    /// <summary>
    /// Interaction logic for ItemManager.xaml
    /// </summary>
    public partial class ItemManager : Window 
	{
		private IItemCollectionBinding itemCollectionBinding;

        /// <summary>
        ///  Initializes a new instance of the ItemManager class.
        /// </summary>
        /// <param name="itemCollectionBinding">
		/// <para> Implement a warp up class for binding the list of items to manager. </para>
		/// <para> Suggested to inherit from <see cref="AbstractItemCollection{T}"/>. </para>
		/// </param>
        public ItemManager(IItemCollectionBinding itemCollectionBinding)
		{
			InitializeComponent();

			foreach (var property in itemCollectionBinding.GetPropertyBinds())
			{
				this.DataGridBookmarks.Columns.Add(new DataGridTextColumn()
				{
					Binding = new Binding("Item." + property.Binding),
					Header = property.DisplayName,
					IsReadOnly = true,
					CanUserReorder = false
				});
			}

			this.DataGridBookmarks.DataContext = itemCollectionBinding.DataContext;
			this.itemCollectionBinding = itemCollectionBinding;
		}

		/// <summary>
		/// Find visual parent.
		/// </summary>
		/// <typeparam name="T">Object that is returned.</typeparam>
		/// <param name="element">Element of wich the visual parent is requested.</param>
		/// <returns>Object wich is the visual parent.</returns>
		/// <remarks>Needed for one click editing.</remarks>
		private static T FindVisualParent<T>(UIElement element) where T : UIElement
		{
			UIElement parent = element;
			while (parent != null)
			{
				T correctlyTyped = parent as T;
				if (correctlyTyped != null)
				{
					return correctlyTyped;
				}

				parent = System.Windows.Media.VisualTreeHelper.GetParent(parent) as UIElement;
			}

			return null;
		}

		/// <summary>
		/// Allows one click editing.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
		private void DataGridCell_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			DataGridCell cell = sender as DataGridCell;
			if (cell != null && !cell.IsEditing && !cell.IsReadOnly)
			{
				if (!cell.IsFocused)
				{
					cell.Focus();
				}

				DataGrid dataGrid = FindVisualParent<DataGrid>(cell);
				if (dataGrid != null)
				{
					if (dataGrid.SelectionUnit != DataGridSelectionUnit.FullRow)
					{
						if (!cell.IsSelected)
						{
							cell.IsSelected = true;
						}
					}
					else
					{
						DataGridRow row = FindVisualParent<DataGridRow>(cell);
						if (row != null && !row.IsSelected)
						{
							row.IsSelected = true;
						}
					}
				}
			}
        }

        /// <summary>
        /// Save and delete bookmarks that are selected for deletion.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void Ok_btn_Click(object sender, RoutedEventArgs e)
        {
            itemCollectionBinding.RemoveDelete();
            this.Close();
        }

        /// <summary>
        /// Close the form when cancelled.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        /// <remarks>Delete boolean for bookmarks that are selected for deletion.</remarks>
        private void Cancel_btn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

    }
}
