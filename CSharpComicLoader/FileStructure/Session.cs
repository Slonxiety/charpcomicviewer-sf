using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CSharpComicLoader.FileStructure
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

        /// <summary>
        /// Gets the name of the current file.
        /// </summary>
        [XmlIgnore]
        public string CurrentFileName
        {
            get { return GetCurrentFileName(); }
        }

        /// <summary>
        /// Gets the location of the directory of the current file.
        /// </summary>
        [XmlIgnore]
        public string CurrentFileDirectoryLocation
        {
            get { return GetCurrentFileDirectoryLocation(); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether bookmark manager should delet this bookmark.
        /// </summary>
        [XmlIgnore]
        public bool Delete { get; set; }

        /// <summary>
        /// Get the directory location of the CurrentFile.
        /// </summary>
        /// <returns>directory location of the current file.</returns>
        public string GetCurrentFileDirectoryLocation()
        {
            string filePath = files[currentFile];
            string[] filePathSplit = filePath.Split('\\');
            string directory = "";
            for (int i = 0; i < filePathSplit.Length - 1; i++)
            {
                directory += filePathSplit[i] + "\\";
            }

            return directory;
        }

        /// <summary>
        /// Get the file name of the CurrentFile.
        /// </summary>
        /// <returns>Filename of current file.</returns>
        private string GetCurrentFileName()
        {
            string filePath = files[currentFile];
            string[] filePathSplit = filePath.Split('\\');
            string fileNameWithExtension = filePathSplit[filePathSplit.Length - 1];
            filePathSplit = fileNameWithExtension.Split('.');
            string filename = "";
            for (int i = 0; i < filePathSplit.Length - 1; i++)
            {
                filename += filePathSplit[i];
            }

            if (filename == "")
                filename = fileNameWithExtension;


            return filename;
        }
    }
}
