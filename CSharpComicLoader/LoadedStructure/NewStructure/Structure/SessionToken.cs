using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CSharpComicLoader.NewFileStructure
{
    /// <summary>
    /// A Bookmark
    /// </summary>
    public class SessionToken
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Session"/> class.
         /// </summary>
        public SessionToken()
        {
            //Needed for serialize.
        }

        /// <summary>
        /// Initializes a new instance of the Bookmark class.
        /// </summary>
        /// <param name="rootDirectory">The root directory of the session</param>
        public SessionToken(string rootDirectory, string[] passFilter, string[] denyFilter)
        {
            RootDirectory = rootDirectory;
            PassFilter = passFilter;
            DenyFilter = denyFilter;
        }
        /// <summary>
        /// The root directory of the session
        /// </summary>
        [XmlElement("Root")]
        public string RootDirectory { get; private set; }

        /// <summary>
        /// If not empty, only files with the path containing any of the path in PassFilter would accepted.
        /// </summary>
        [XmlArray("Pass")]
        public string[] PassFilter { get; set; }

        /// <summary>
        /// Any files with the path contained in the DenyFilter would be denied.
        /// </summary>
        [XmlArray("Deny")]
        public string[] DenyFilter { get; set; }

        /// <summary>
        /// All files. 
        /// </summary>
        [XmlArray("FileList")]
        public string[] FileList { get; private set; }

    }

}
