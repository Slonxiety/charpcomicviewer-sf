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
using System.Xml;
using System.Xml.Serialization;

namespace CSharpComicLoader.NewFileStructure
{
	/// <summary>
	/// A Bookmark
	/// </summary>
	[XmlRoot("Bookmark")]
	public class Bookmark
	{
        /// <summary>
        /// Initializes a new instance of the <see cref="Bookmark"/> class.
        /// </summary>
        public Bookmark()
        {
            //Needed for serialize.
        }

        /// <summary>
        /// Initializes a new instance of the Bookmark class.
        /// </summary>
        /// <param name="files">The files.</param>
        /// <param name="currentFile">Current file being read.</param>
        /// <param name="currentPageOfFile">Current page of file being read.</param>
        public Bookmark(string name)
        {
            Name = name;
            PageDatas = new List<IPageData>();
        }


        [XmlElement("Name")]
        public string Name { get; private set; }


        [XmlArray("Pages")]
        [XmlArrayItem("PageData", typeof(PageData))]
        [XmlArrayItem("ArchivePageData", typeof(ArchivePageData))]
        public List<IPageData> PageDatas { get; private set; }
	}
}
