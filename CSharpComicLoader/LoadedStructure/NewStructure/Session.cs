using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CSharpComicLoader.NewFileStructure
{
    /// <summary>
	/// A Bookmark
	/// </summary>
    public class Session
    {
        /// <summary>
		/// The files
		/// </summary>
		private string[] files;

        /// <summary>
        /// Current file being read.
        /// </summary>
        private int currentFile;

        /// <summary>
        /// Current page of file being read.
        /// </summary>
        private int currentPageOfFile;

        /// <summary>
        /// Initializes a new instance of the <see cref="Bookmark"/> class.
        /// </summary>
        public Session()
        {
            //Needed for serialize.
        }

        /// <summary>
        /// Initializes a new instance of the Bookmark class.
        /// </summary>
        /// <param name="files">The files.</param>
        /// <param name="currentFile">Current file being read.</param>
        /// <param name="currentPageOfFile">Current page of file being read.</param>
        public Session(string[] files, int currentFile, int currentPageOfFile)
        {
            this.Files = files;
            this.FileNumber = currentFile;
            this.PageNumber = currentPageOfFile;
        }

        /// <summary>
        /// Gets or sets the files
        /// </summary>
        [XmlArray("Files")]
        public string[] Files
        {
            get { return files; }
            set { files = value; }
        }

        /// <summary>
        /// Gets or sets current file being read.
        /// </summary>
        [XmlElement("CurrentFile")]
        public int FileNumber
        {
            get { return currentFile; }
            set { currentFile = value; }
        }

        /// <summary>
        /// Gets or sets current page of file being read.
        /// </summary>
        [XmlElement("CurrentPage")]
        public int PageNumber
        {
            get { return currentPageOfFile; }
            set { currentPageOfFile = value; }
        }
    }
}
