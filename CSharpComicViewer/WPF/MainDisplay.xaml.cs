﻿//-------------------------------------------------------------------------------------
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
using System.Collections;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Xml.Serialization;
using Microsoft.Win32;

using CSharpComicLoader;
using CSharpComicLoader.OldFileStructure;
using System.Windows.Media;

namespace CSharpComicViewer.WPF
{
    /// <summary>
    /// Interaction logic for Window2.xaml
    /// </summary>
    public partial class MainDisplay : Window
    {
        #region Properties

        /// <summary>
        /// Gets or sets the opening file.
        /// </summary>
        /// <value>
        /// The opening file.
        /// </value>
        private string _openingFile;
        private FileViewModel _fileViewModel;

        /// <summary>
        /// Gets or sets the next page count.
        /// </summary>
        /// <value>
        /// The next page count.
        /// </value>
        private int _nextPageCount = 2;

        /// <summary>
        /// Gets or sets the previous page count.
        /// </summary>
        /// <value>
        /// The previous page count.
        /// </value>
        private int _previousPageCount;

        /// <summary>
        /// The information Text
        /// </summary>
        private InformationText _informationText;

        /// <summary>
        /// The show message timer.
        /// </summary>
        private DispatcherTimer _showMessageTimer;

        /// <summary>
        /// The page information timer.
        /// </summary>
        private DispatcherTimer _pageInformationTimer;

        /// <summary>
        /// The last mouse move.
        /// </summary>
        private DateTime _lastMouseMove;

        /// <summary>
        /// Gets or sets a value indicating whether mouse is hidden.
        /// </summary>
        /// <value>
        ///   <c>true</c> if mouse is hidden; otherwise, <c>false</c>.
        /// </value>
        private bool _mouseIsHidden;

        /// <summary>
        /// Gets or sets the scroll value vertical.
        /// </summary>
        /// <value>
        /// The scroll value vertical.
        /// </value>
        private int _scrollValueVertical;

        /// <summary>
        /// Gets or sets the scroll value horizontal.
        /// </summary>
        /// <value>
        /// The scroll value horizontal.
        /// </value>
        private int _scrollValueHorizontal;

        /// <summary>
        /// Gets or sets the current mouse position.
        /// </summary>
        /// <value>
        /// The current mouse position.
        /// </value>
        private Point _currentMousePosition;

        /// <summary>
        /// Gets or sets the mouse X.
        /// </summary>
        /// <value>
        /// The mouse X.
        /// </value>
        private double _mouseX;

        /// <summary>
        /// Gets or sets the mouse Y.
        /// </summary>
        /// <value>
        /// The mouse Y.
        /// </value>
        private double _mouseY;

        /// <summary>
        /// Gets or sets a value indicating whether mouse drag.
        /// </summary>
        /// <value>
        ///   <c>true</c> if mouse drag; otherwise, <c>false</c>.
        /// </value>
        private bool _mouseDrag;

        /// <summary>
        /// Gets or sets a value indicating whether going to next page is allowed.
        /// </summary>
        /// <value>
        ///   <c>true</c> if going to next page is allowed; otherwise, <c>false</c>.
        /// </value>
        private bool _nextPageBoolean;

        /// <summary>
        /// Gets or sets a value indicating whether going to previous page is allowed.
        /// </summary>
        /// <value>
        ///   <c>true</c> if going to previous page is allowed; otherwise, <c>false</c>.
        /// </value>
        private bool _previousPageBoolean;

        /// <summary>
        /// Gets or sets the timeout to hide.
        /// </summary>
        /// <value>
        /// The timeout to hide.
        /// </value>
        private TimeSpan _timeoutToHide;

        /// <summary>
        /// Gets or sets the mouse idle.
        /// </summary>
        /// <value>
        /// The mouse idle.
        /// </value>
        public DispatcherTimer MouseIdle
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the configuration.
        /// </summary>
        public Configuration.Configuration Configuration
        {
            get;
            private set;
        }

        /// <summary>
        /// Start position on the image
        /// </summary>
        public enum ImageStartPosition
        {
            /// <summary>
            /// Start at the start of the page.
            /// </summary>
            Top,

            /// <summary>
            /// Start at the bottom of the page.
            /// </summary>
            Bottom
        }

        /// <summary>
        /// The window mode.
        /// </summary>
        public enum WindowMode
        {
            /// <summary>
            /// Display in a fullscreen.
            /// </summary>
            Fullscreen,

            /// <summary>
            /// Display in a window.
            /// </summary>
            Windowed
        }
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="MainDisplay"/> class.
        /// </summary>
        public MainDisplay()
        {
            _nextPageCount = 2;
            _previousPageCount = 2;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MainDisplay"/> class.
        /// </summary>
        /// <param name="openingFile">The opening file.</param>
        public MainDisplay(string openingFile)
            : this()
        {
            InitializeComponent();
            _openingFile = openingFile;
        }

        /// <summary>
        /// Handles the Loaded event of the MainDisplay control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void MainDisplay_Loaded(object sender, RoutedEventArgs e)
        {
            //Ensure that the window is active on start
            this.Activate();

            _fileViewModel = new FileViewModel();

            //set mouse idle timer
            _timeoutToHide = TimeSpan.FromSeconds(2);
            MouseIdle = new DispatcherTimer();
            MouseIdle.Interval = TimeSpan.FromSeconds(1);
            MouseIdle.Tick += new EventHandler(MouseIdleChecker);
            MouseIdle.Start();

            //Load config
            LoadConfiguration();
            SetBookmarkMenus();

            if (Configuration.Windowed)
            {
                SetWindowMode(WindowMode.Windowed);
            }

            //gray out resume last file if the files dont't exist
            if (Configuration.Resume != null)
            {
                foreach (string file in Configuration.Resume.Files)
                {
                    if (!System.IO.File.Exists(file) && !Directory.Exists(file))
                    {
                        ResumeFile_MenuBar.IsEnabled = false;
                        ResumeFile_RightClick.IsEnabled = false;
                    }
                }
            }
            else
            {
                ResumeFile_MenuBar.IsEnabled = false;
                ResumeFile_RightClick.IsEnabled = false;
            }

            _scrollValueHorizontal = (int)(ScrollField.ViewportHeight * 0.05);
            _scrollValueVertical = (int)(ScrollField.ViewportWidth * 0.05);

            //open file (when opening associated by double click) //OR drag file/folder to the application
            if (_openingFile != null)
            {
                FileAttributes attr = File.GetAttributes(_openingFile);

                if (attr.HasFlag(FileAttributes.Directory))
                {
                    var session = _fileViewModel.CreateSessionFromFolder(_openingFile);
                    LoadFile(session);
                }
                else
                {
                    var session = _fileViewModel.CreateSessionFromFiles(new string[] { _openingFile });
                    LoadFile(session);
                }

                    
            }
        }

        /// <summary>
        /// Exit the applications.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void ApplicationExit(object sender, EventArgs e)
        {
            SaveResumeToConfiguration();
            SaveConfiguration();
            Application.Current.Shutdown();
        }

        /// <summary>
        /// Loads the configuration.
        /// </summary>
        private void LoadConfiguration()
        {
            //xml config load
            try
            {
                string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                string userFilePath = Path.Combine(localAppData, "C# Comicviewer");

                XmlSerializer mySerializer = new XmlSerializer(typeof(Configuration.Configuration));
                if (System.IO.File.Exists(userFilePath + "\\Configuration.xml"))
                {
                    System.IO.FileStream myFileStream = new System.IO.FileStream(userFilePath + "\\Configuration.xml", System.IO.FileMode.Open);
                    Configuration = (Configuration.Configuration)mySerializer.Deserialize(myFileStream);
                    myFileStream.Close();
                }

                if (Configuration == null)
                {
                    Configuration = new Configuration.Configuration();
                }
            }
            catch (Exception)
            {
                Configuration = new Configuration.Configuration();
            }
        }

        /// <summary>
        /// Saves the configuration.
        /// </summary>
        /// <returns><c>True</c> if succes, otherwise returns <c>false</c>.</returns>
        private bool SaveConfiguration()
        {
            //xml config save
            try
            {
                string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                string userFilePath = Path.Combine(localAppData, "C# Comicviewer");

                if (!Directory.Exists(userFilePath))
                {
                    Directory.CreateDirectory(userFilePath);
                }

                XmlSerializer mySerializer = new XmlSerializer(typeof(Configuration.Configuration));
                System.IO.StreamWriter myWriter = new System.IO.StreamWriter(userFilePath + "\\Configuration.xml");
                mySerializer.Serialize(myWriter, Configuration);
                myWriter.Close();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Saves the resume to configuration.
        /// </summary>
        private void SaveResumeToConfiguration()
        {
            if (_fileViewModel.IsFileLoaded())
            {
                var data = _fileViewModel.GetSession();
                Configuration.Resume = data;
            }
        }

        #region Bookmarks
        /// <summary>
        /// Sets the bookmark menus.
        /// </summary>
        private void SetBookmarkMenus()
        {
            Bookmarks_MenuRightClick.Items.Clear();
            Bookmarks_MenuRightClick.Items.Add(AddBookmark_MenuRightClick);
            Bookmarks_MenuRightClick.Items.Add(ManageBookmarks_MenuRightClick);
            Bookmarks_MenuRightClick.Items.Add(new Separator());

            Bookmarks_MenuBar.Items.Clear();
            Bookmarks_MenuBar.Items.Add(AddBookmark_MenuBar);
            Bookmarks_MenuBar.Items.Add(ManageBookmarks_MenuBar);
            Bookmarks_MenuBar.Items.Add(new Separator());

            if (Configuration != null)
            {
                if (Configuration.Bookmarks != null)
                {
                    if (Configuration.Bookmarks.Count > 0)
                    {
                        foreach (var currentBookmark in Configuration.Bookmarks)
                        {
                            string[] files = currentBookmark.Files;
                            MenuItem bookmark = new MenuItem();
                            bookmark.Header = currentBookmark.CurrentFileName;
                            bookmark.ToolTip = files[currentBookmark.FileNumber];
                            bookmark.Click += new RoutedEventHandler(LoadBookmark_Click);
                            Bookmarks_MenuRightClick.Items.Add(bookmark);

                            MenuItem bookmark_bar = new MenuItem();
                            bookmark_bar.Header = currentBookmark.CurrentFileName;
                            bookmark_bar.ToolTip = files[currentBookmark.FileNumber];
                            bookmark_bar.Click += new RoutedEventHandler(LoadBookmark_Click);
                            Bookmarks_MenuBar.Items.Add(bookmark_bar);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Handles the Click event of the LoadBookmark control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void LoadBookmark_Click(object sender, EventArgs e)
        {
            //right click menu
            ArrayList data = new ArrayList();
            for (int i = 0; i < Bookmarks_MenuRightClick.Items.Count; i++)
            {
                if ((MenuItem)sender == Bookmarks_MenuRightClick.Items[i])
                {
                    var bookmark = Configuration.Bookmarks[i - 3];

                    LoadFile(bookmark);
                }
            }

            //the bar
            for (int i = 0; i < Bookmarks_MenuBar.Items.Count; i++)
            {
                if ((MenuItem)sender == Bookmarks_MenuBar.Items[i])
                {
                    var bookmark = Configuration.Bookmarks[i - 3];

                    LoadFile(bookmark);
                }
            }
        }
        #endregion

        #region Keyboard & Mouse
        /// <summary>
        /// Called when [key down].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.KeyEventArgs"/> instance containing the event data.</param>
        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.X)
            {
                ApplicationExit(null, e);
            }

            if (e.Key == Key.R)
            {
                if (ResumeFile_RightClick.IsEnabled)
                {
                    Resume_Click(sender, e);
                }
                else
                {
                    ShowMessage("No archive to resume");
                }
            }

            if (e.Key == Key.I)
            {
                ShowPageInformation();
            }

            if (e.Key == Key.L)
            {
                _lastMouseMove = DateTime.Now;

                if (_mouseIsHidden)
                {
                    Mouse.OverrideCursor = Cursors.Arrow;
                    _mouseIsHidden = false;
                }

                ShowFileDialogAndLoad();
            }

            if (e.Key == Key.M)
            {
                WindowState = System.Windows.WindowState.Minimized;
            }

            if (e.Key == Key.T)
            {
                ToggleImageOptions();
            }

            if (e.Key == Key.N)
            {
                
                if (_fileViewModel.IsFileLoaded())
                {
                    if (string.IsNullOrEmpty(_fileViewModel.GetCurrentInfo()))
                    {
                        ShowMessage("No information text");
                    }
                    else
                    {
                        _informationText = new InformationText(_fileViewModel.GetCurrentLocation(), _fileViewModel.GetCurrentInfo());
                        _informationText.ShowDialog();
                    }
                }
                else
                {
                    ShowMessage("No archive loaded");
                }
            }

            if (e.Key == Key.W)
            {
                if (Configuration.Windowed)
                {
                    //go full screen if windowed
                    Configuration.Windowed = false;

                    SetWindowMode(WindowMode.Fullscreen);
                }
                else
                {
                    //go windowed if fullscreen
                    Configuration.Windowed = true;
                    SetWindowMode(WindowMode.Windowed);
                }

                _scrollValueHorizontal = (int)(ScrollField.ViewportHeight * 0.05);
                _scrollValueVertical = (int)(ScrollField.ViewportWidth * 0.05);

                if (DisplayedImage.Source != null)
                {
                    DisplayImage(_fileViewModel.GetImage(), ImageStartPosition.Top);
                }
            }

            if (e.Key == Key.PageDown)
            {
                //prevent default action from occurring.
                e.Handled = true;
            }

            if (e.Key == Key.PageUp)
            {
                //prevent default action from occurring.
                e.Handled = true;
            }
        }

        /// <summary>
        /// Called when [preview key down].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.KeyEventArgs"/> instance containing the event data.</param>
        private void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (_fileViewModel.IsFileLoaded())
            {
                if (e.Key == Key.Home && !Keyboard.Modifiers.HasFlag(ModifierKeys.Alt)) _fileViewModel.PointToHome();
                if (e.Key == Key.Home && Keyboard.Modifiers.HasFlag(ModifierKeys.Alt))  _fileViewModel.PointToBegin();
                if (e.Key == Key.End && !Keyboard.Modifiers.HasFlag(ModifierKeys.Alt))  _fileViewModel.PointToEnd();
                if (e.Key == Key.End && Keyboard.Modifiers.HasFlag(ModifierKeys.Alt))   _fileViewModel.PointToLast();

                var image = _fileViewModel.GetImage();
                if (image != null)
                {
                    DisplayImage(image, ImageStartPosition.Top);
                }
            }

            if (e.Key == Key.PageDown)
            {
                NextPage();

                //prevent default action from occurring.
                e.Handled = true;
            }

            if (e.Key == Key.PageUp)
            {
                PreviousPage();

                //prevent default action from occurring.
                e.Handled = true;
            }

            if (e.SystemKey == Key.PageDown && Keyboard.Modifiers.HasFlag(ModifierKeys.Alt))
            {
                NextFile();
            }

            if (e.SystemKey == Key.PageUp && Keyboard.Modifiers.HasFlag(ModifierKeys.Alt))
            {
                PreviousFile();
            }

            if (e.Key == Key.Down)
            {
                OnArrowKey(Key.Down);
            }

            if (e.Key == Key.Up)
            {
                OnArrowKey(Key.Up);
            }

            if (e.Key == Key.Right)
            {
                OnArrowKey(Key.Right);
            }

            if (e.Key == Key.Left)
            {
                OnArrowKey(Key.Left);
            }
        }

        /// <summary>
        /// Called when [arrow key].
        /// </summary>
        /// <param name="e">The e.</param>
        private void OnArrowKey(Key e)
        {
            int scrollAmmount = 50;

            //scroll down
            if (e == Key.Down && DisplayedImage.Source != null)
            {
                ScrollField.ScrollToVerticalOffset(ScrollField.VerticalOffset + scrollAmmount);
            }

            //scroll up
            if (e == Key.Up && DisplayedImage.Source != null)
            {
                ScrollField.ScrollToVerticalOffset(ScrollField.VerticalOffset - scrollAmmount);
            }

            //scroll right
            if (e == Key.Right && DisplayedImage.Source != null)
            {
                ScrollField.ScrollToHorizontalOffset(ScrollField.HorizontalOffset + scrollAmmount);
            }

            //scroll left
            if (e == Key.Left && DisplayedImage.Source != null)
            {
                ScrollField.ScrollToHorizontalOffset(ScrollField.HorizontalOffset - scrollAmmount);
            }

            if (ScrollField.VerticalOffset > ScrollField.ScrollableHeight || ScrollField.VerticalOffset < 0)
            {
                ScrollField.ScrollToVerticalOffset(ScrollField.ScrollableHeight);
            }

            if (ScrollField.HorizontalOffset > ScrollField.ScrollableWidth || ScrollField.HorizontalOffset < 0)
            {
                ScrollField.ScrollToHorizontalOffset(ScrollField.ScrollableWidth);
            }
        }

        /// <summary>
        /// Called when mouse wheel scrolls.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseWheelEventArgs"/> instance containing the event data.</param>
        private void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            //scroll down
            if (e.Delta < 0 && DisplayedImage.Source != null)
            {
                _previousPageBoolean = false;
                _previousPageCount = 2;
                if (DisplayedImage.Width > ScrollField.ViewportWidth)
                {
                    //image widther then screen
                    if (ScrollField.HorizontalOffset == ScrollField.ScrollableWidth && ScrollField.VerticalOffset == ScrollField.ScrollableHeight)
                    {
                        //Can count down for next page
                        _nextPageBoolean = true;
                        _nextPageCount--;
                    }
                    else if (ScrollField.VerticalOffset == ScrollField.ScrollableHeight)
                    {
                        //scroll horizontal
                        ScrollField.ScrollToHorizontalOffset(ScrollField.HorizontalOffset + _scrollValueHorizontal);
                    }
                }
                else if (ScrollField.VerticalOffset == ScrollField.ScrollableHeight)
                {
                    //Can count down for next page
                    _nextPageBoolean = true;
                    _nextPageCount--;
                }
            }
            else if (e.Delta > 0 && DisplayedImage.Source != null)
            {
                //scroll up
                _nextPageBoolean = false;
                _nextPageCount = 2;
                if (DisplayedImage.Width > ScrollField.ViewportWidth)
                {
                    //image widther then screen
                    if (ScrollField.HorizontalOffset == 0 && ScrollField.VerticalOffset == 0)
                    {
                        //Can count down for previous page
                        _previousPageBoolean = true;
                        _previousPageCount--;
                    }
                    else if (ScrollField.VerticalOffset == 0)
                    {
                        //scroll horizontal
                        ScrollField.ScrollToHorizontalOffset(ScrollField.HorizontalOffset - _scrollValueHorizontal);
                    }
                }
                else if (ScrollField.VerticalOffset == 0)
                {
                    //Can count down for previous page
                    _previousPageBoolean = true;
                    _previousPageCount--;
                }
            }

            if (_nextPageBoolean && _nextPageCount <= 0)
            {
                NextPage();
                _nextPageBoolean = false;
                _nextPageCount = 2;
            }
            else if (_previousPageBoolean && _previousPageCount <= 0)
            {
                PreviousPage();
                _previousPageBoolean = false;
                _previousPageCount = 2;
            }
        }

        /// <summary>
        /// Called when [preview mouse wheel].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseWheelEventArgs"/> instance containing the event data.</param>
        private void OnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            OnMouseWheel(sender, e);
        }

        /// <summary>
        /// Called when [mouse move].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            _lastMouseMove = DateTime.Now;

            if (_mouseIsHidden && (Mouse.GetPosition(this) != _currentMousePosition))
            {
                Mouse.OverrideCursor = Cursors.Arrow;
                _mouseIsHidden = false;
            }

            _currentMousePosition = Mouse.GetPosition(this);

            int speed = 2; //amount by with mouse_x/y - MousePosition.X/Y is divided, determines drag speed
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                //I am mouse dragging.
                if (_mouseDrag == false)
                {
                    //If changed position
                    _mouseX = _currentMousePosition.X;
                    _mouseY = _currentMousePosition.Y;
                    _mouseDrag = true;
                }
                else
                {
                    //Did not change position
                    if (_currentMousePosition.X < _mouseX && DisplayedImage.Source != null)
                    {
                        //Drag left
                        ScrollField.ScrollToHorizontalOffset(ScrollField.HorizontalOffset + (_mouseX - _currentMousePosition.X) / speed);
                        _mouseDrag = false;
                    }
                    else if (_currentMousePosition.X > _mouseX && DisplayedImage.Source != null)
                    {
                        //Drag right
                        ScrollField.ScrollToHorizontalOffset(ScrollField.HorizontalOffset + (_mouseX - _currentMousePosition.X) / speed);
                        _mouseDrag = false;
                    }

                    if (_currentMousePosition.Y < _mouseY && DisplayedImage.Source != null)
                    {
                        //Drag up
                        ScrollField.ScrollToVerticalOffset(ScrollField.VerticalOffset + (_mouseY - _currentMousePosition.Y) / speed);
                        _mouseDrag = false;
                    }
                    else if (_currentMousePosition.Y > _mouseY && DisplayedImage.Source != null)
                    {
                        //Drag down
                        ScrollField.ScrollToVerticalOffset(ScrollField.VerticalOffset + (_mouseY - _currentMousePosition.Y) / speed);
                        _mouseDrag = false;
                    }
                }
            }
            else
            {
                //make it possible to drag on next check
                _mouseDrag = false;
            }
        }

        /// <summary>
        /// Called when [right button down].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void OnRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Arrow;
            _mouseIsHidden = false;
        }

        /// <summary>
        /// Mouses the idle checker.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void MouseIdleChecker(object sender, EventArgs e)
        {
            TimeSpan elaped = DateTime.Now - _lastMouseMove;
            if (elaped >= _timeoutToHide && !_mouseIsHidden)
            {
                if (this.IsActive && !MenuRightClick.IsOpen)
                {
                    Mouse.OverrideCursor = Cursors.None;
                    _mouseIsHidden = true;
                }
                else if (this.IsActive && MenuRightClick.IsOpen)
                {
                    _lastMouseMove = DateTime.Now;
                }
            }
        }

        #endregion

        #region Menus
        /// <summary>
        /// Handles the Click event of the Resume control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void Resume_Click(object sender, RoutedEventArgs e)
        {
            if (Configuration.Resume != null)
            {
                LoadFile(Configuration.Resume);
            }
        }

        /// <summary>
        /// Handles the Click event of the Load control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void Load_Click(object sender, RoutedEventArgs e)
        {
            ShowFileDialogAndLoad();
        }

        /// <summary>
        /// Handles the Click event of the Load Folder control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void Load_Folder_Click(object sender, RoutedEventArgs e)
        {
            ShowFolderDialogAndLoad();
        }

        /// <summary>
        /// Handles the Click event of the NextPage control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void NextPage_Click(object sender, RoutedEventArgs e)
        {
            NextPage();
        }

        /// <summary>
        /// Handles the Click event of the PreviousPage control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void PreviousPage_Click(object sender, RoutedEventArgs e)
        {
            PreviousPage();
        }

        /// <summary>
        /// Handles the Click event of the NextFile control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void NextFile_Click(object sender, RoutedEventArgs e)
        {
            NextFile();
        }

        /// <summary>
        /// Handles the Click event of the PreviousFile control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void PreviousFile_Click(object sender, RoutedEventArgs e)
        {
            PreviousFile();
        }

        /// <summary>
        /// Handles the Click event of the ShowPageInformation control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void ShowPageInformation_Click(object sender, RoutedEventArgs e)
        {
            ShowPageInformation();
        }

        /// <summary>
        /// Handles the Click event of the Exit control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            ApplicationExit(sender, e);
        }

        /// <summary>
        /// Handles the Click event of the AddBookmark control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void AddBookmark_Click(object sender, RoutedEventArgs e)
        {
            if (_fileViewModel.IsFileLoaded())
            {
                Configuration.Bookmarks.Add(_fileViewModel.GetBookmark());
                SetBookmarkMenus();
                ShowMessage("Bookmark added.");
            }
        }

        /// <summary>
        /// Handles the Click event of the ManageBookmarks control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void ManageBookmarks_Click(object sender, RoutedEventArgs e)
        {
            ItemManager.ItemManager bookmarkManager = new ItemManager.ItemManager(new ItemManager.BookmarkItemCollection(Configuration.Bookmarks));
            bookmarkManager.ShowDialog();
            SetBookmarkMenus();
        }

        /// <summary>
        /// Handles the Click event of the About control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void About_Click(object sender, RoutedEventArgs e)
        {
            About about = new About();
            about.ShowDialog();
        }
        #endregion

        #region Messages and Page information
        /// <summary>
        /// Shows the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void ShowMessage(string message)
        {
            MessageBox.Text = message;
            MessageBox.Visibility = System.Windows.Visibility.Visible;

            if (_showMessageTimer != null)
            {
                _showMessageTimer.Stop();
            }

            _showMessageTimer = new DispatcherTimer();
            _showMessageTimer.Tick += new EventHandler(HideMessage);
            _showMessageTimer.Interval = new TimeSpan(0, 0, 2);
            _showMessageTimer.Start();
        }

        /// <summary>
        /// Hides the message.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void HideMessage(object sender, EventArgs e)
        {
            MessageBox.Visibility = System.Windows.Visibility.Hidden;
        }

        /// <summary>
        /// Shows the page information.
        /// </summary>
        public void ShowPageInformation()
        {
            if (_fileViewModel.IsFileLoaded())
            {
                PageInfoBox.Text = _fileViewModel.GetPageInfo();

                PageInfoBox.Visibility = System.Windows.Visibility.Visible;

                if (_pageInformationTimer != null)
                {
                    _pageInformationTimer.Stop();
                }

                _pageInformationTimer = new DispatcherTimer();
                _pageInformationTimer.Tick += new EventHandler(HidePageInformation);
                _pageInformationTimer.Interval = new TimeSpan(0, 0, 5);
                _pageInformationTimer.Start();
            }
        }

        /// <summary>
        /// Hides the page information.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void HidePageInformation(object sender, EventArgs e)
        {
            PageInfoBox.Visibility = System.Windows.Visibility.Hidden;
        }

        /// <summary>
        /// Updates the page information.
        /// </summary>
        private void UpdatePageInformation()
        {
            PageInfoBox.Text = _fileViewModel.GetPageInfo();
        }
        #endregion

        #region Load and Display
        /// <summary>
        /// Displays the image.
        /// </summary>
        /// <param name="bitmapSource">The image source.</param>
        /// <param name="scrollTo">The scroll to.</param>
        public void DisplayImage(BitmapSource bitmapSource, ImageStartPosition scrollTo)
        {
            // If page information is displayed update it with new information
            if (PageInfoBox.Visibility == System.Windows.Visibility.Visible)
            {
                UpdatePageInformation();
            }

            switch (scrollTo.ToString())
            {
                case "Top":
                    {
                        ScrollField.ScrollToTop();
                        ScrollField.ScrollToLeftEnd();
                        break;
                    }

                case "Bottom":
                    {
                        ScrollField.ScrollToBottom();
                        ScrollField.ScrollToRightEnd();
                        break;
                    }

                default:
                    {
                        break;
                    }
            }


            if (Configuration.ImageMode != ImageMode.Normal)
            {
                bitmapSource = ImageUtils.ResizeImage(bitmapSource,
                        new Size(ScrollField.ViewportWidth, ScrollField.ViewportHeight),
                        Configuration.ImageMode);
            }

            this.Background = ImageUtils.GetBackgroundColor(bitmapSource);

            DisplayedImage.Source = bitmapSource;
            DisplayedImage.Width = bitmapSource.Width;
            DisplayedImage.Height = bitmapSource.Height;

            if (DisplayedImage.Width < ScrollField.ViewportWidth)
            {
                DisplayedImage.HorizontalAlignment = HorizontalAlignment.Center;
            }
            else if (DisplayedImage.Width >= ScrollField.ViewportWidth)
            {
                DisplayedImage.HorizontalAlignment = HorizontalAlignment.Left;
            }

            if (DisplayedImage.Height < ScrollField.ViewportHeight)
            {
                DisplayedImage.VerticalAlignment = VerticalAlignment.Center;
            }
            else if (DisplayedImage.Height >= ScrollField.ViewportHeight)
            {
                DisplayedImage.VerticalAlignment = VerticalAlignment.Top;
            }

            ShowPageInformation();
        }


        /// <summary>
        /// Show folder dialog, load archive(s) and display first page
        /// </summary>
        public void ShowFolderDialogAndLoad()
        {
            var openFolderDialog = new System.Windows.Forms.FolderBrowserDialog();
            if (_fileViewModel.IsFileLoaded())
            {
                var bookmark = _fileViewModel.GetBookmark();
                openFolderDialog.RootFolder = Environment.SpecialFolder.Desktop;
                openFolderDialog.SelectedPath = bookmark.GetCurrentFileDirectoryLocation();
            }

            if (openFolderDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var session = _fileViewModel.CreateSessionFromFolder(openFolderDialog.SelectedPath);
                LoadFile(session);
            }
        }

        /// <summary>
        /// Show file dialog, load archive(s) and display first page
        /// </summary>
        public void ShowFileDialogAndLoad ()
        {

            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (_fileViewModel.IsFileLoaded())
            {
                var bookmark = _fileViewModel.GetBookmark();
                openFileDialog.InitialDirectory = bookmark.GetCurrentFileDirectoryLocation();
            }

            openFileDialog.Filter = Utils.FileLoaderFilter;
            openFileDialog.Multiselect = true;
            openFileDialog.ShowDialog();

            if (openFileDialog.FileNames.Length <= 0)
            {
                return;
            }

            var session = _fileViewModel.CreateSessionFromFiles(openFileDialog.FileNames);
            LoadFile(session);
        }

        /// <summary>
        /// Load archive from a bookmark
        /// </summary>
        /// <param name="bookmark"></param>
        public void LoadFile(Bookmark bookmark)
        {
            Mouse.OverrideCursor = Cursors.Wait;

            var result = _fileViewModel.Load(bookmark);
            ParseResult(result);

            Mouse.OverrideCursor = Cursors.Arrow;
        }

        /// <summary>
        /// Load archive from a session
        /// </summary>
        /// <param name="session"></param>
        public void LoadFile(Session session)
        {
            Mouse.OverrideCursor = Cursors.Wait;

            var result = _fileViewModel.Load(session);
            ParseResult(result);
            
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        /// <summary>
        /// Parse a LoadResult and show error or image.
        /// </summary>
        /// <param name="result">result after load</param>
        public void ParseResult(LoadResult result)
        {
            if (result.HasFile)
            {
                foreach (var info in _fileViewModel.GetAllInfo())
                {
                    Mouse.OverrideCursor = Cursors.Arrow;
                    _informationText = new InformationText(info.Title, info.Content);
                    _informationText.ShowDialog();
                    Mouse.OverrideCursor = Cursors.Wait;
                }

                DisplayImage(_fileViewModel.GetImage(), ImageStartPosition.Top);
                if (result.HasError)
                {
                    ShowMessage(result.Error);
                }
            }
            else if (result.HasError)
            {
                ShowMessage(result.Error);
            }
            else
            {
                ShowMessage("No supported files found.");
            }
        }


        /// <summary>
        /// Toggle the images options (fit to screen etc.)
        /// </summary>
        public void ToggleImageOptions()
        {
            int option_count = Enum.GetNames(typeof(ImageMode)).Length;
            Configuration.ImageMode = (ImageMode)(((int)Configuration.ImageMode + 1) % option_count);

            if (Configuration.ImageMode == ImageMode.FitToWidth)       ShowMessage("Fit to width.");
            if (Configuration.ImageMode == ImageMode.FitToHeight)      ShowMessage("Fit to height.");
            if (Configuration.ImageMode == ImageMode.FitToScreen)      ShowMessage("Fit to screen.");
            if (Configuration.ImageMode == ImageMode.FitToShort)       ShowMessage("Fit to short side.");
            if (Configuration.ImageMode == ImageMode.FitToShortScaled) ShowMessage("Fit to short side by proportion.");
            if (Configuration.ImageMode == ImageMode.Normal)           ShowMessage("Normal mode.");

            if (DisplayedImage.Source != null)
            {
                DisplayImage(_fileViewModel.GetImage(), ImageStartPosition.Top);
            }
        }

        /// <summary>
        /// Loads the first file found after the current one and displays the first image if possible.
        /// </summary>
        private void NextFile()
        {
            if (_fileViewModel.IsFileLoaded())
            {
                if (_fileViewModel.IsLastFile())
                {
                    var loadNext = _fileViewModel.GetOutOfRangeNextSession();
                    if (loadNext != null) LoadFile(loadNext);
                }
                else
                {
                    _fileViewModel.PointToNextFile();
                    var image = _fileViewModel.GetImage();

                    DisplayImage(image, ImageStartPosition.Top);
                }
            }
        }

        /// <summary>
        /// Previouses the file.
        /// </summary>
        private void PreviousFile()
        {
            if (_fileViewModel.IsFileLoaded())
            {
                if (_fileViewModel.IsFirstFile())
                {
                    var loadPrevious = _fileViewModel.GetOutOfRangePreviousSession();
                    if (loadPrevious != null) LoadFile(loadPrevious);
                }
                else
                {
                    _fileViewModel.PointToPreviousFile();
                    var image = _fileViewModel.GetImage();

                    DisplayImage(image, ImageStartPosition.Bottom);
                }
            }
        }

        /// <summary>
        /// Go to next page
        /// </summary>
        public void NextPage()
        {
            if (_fileViewModel.IsFileLoaded())
            {
                if (_fileViewModel.IsLastFile() && _fileViewModel.IsLastPage())
                    return;

                _fileViewModel.PointToNextPage();
                var image = _fileViewModel.GetImage();
                if (image != null)
                {
                    DisplayImage(image, ImageStartPosition.Top);
                }
            }
        }


        /// <summary>
        /// Go to previous page
        /// </summary>
        public void PreviousPage()
        {
            if (_fileViewModel.IsFileLoaded())
            {
                if (_fileViewModel.IsFirstFile() && _fileViewModel.IsFirstPage())
                    return;

                _fileViewModel.PointToPreviousPage();
                var image = _fileViewModel.GetImage();
                if (image != null)
                {
                    DisplayImage(image, ImageStartPosition.Bottom);
                }
            }

        }

        /// <summary>
        /// Sets the window mode.
        /// </summary>
        /// <param name="windowMode">The window mode.</param>
        public void SetWindowMode(WindowMode windowMode)
        {
            //set window mode
            if (windowMode == WindowMode.Windowed)
            {
                //go hidden first to fix size bug
                MenuBar.Visibility = Visibility.Visible;
                WindowStyle = System.Windows.WindowStyle.SingleBorderWindow;
                ResizeMode = System.Windows.ResizeMode.CanResize;

                //Add small delay to prevent bug where window size isn't set correctly.
                System.Threading.Thread.Sleep(100);

                WindowState = System.Windows.WindowState.Maximized;
            }
            else
            {
                MenuBar.Visibility = Visibility.Collapsed;
                WindowStyle = System.Windows.WindowStyle.None;
                ResizeMode = System.Windows.ResizeMode.NoResize;

                //go minimized first to hide taskbar
                WindowState = System.Windows.WindowState.Minimized;

                //Add small delay to prevent bug where window size isn't set correctly.
                System.Threading.Thread.Sleep(100);

                WindowState = System.Windows.WindowState.Maximized;
                Focus();
            }
        }
        #endregion
    }
}