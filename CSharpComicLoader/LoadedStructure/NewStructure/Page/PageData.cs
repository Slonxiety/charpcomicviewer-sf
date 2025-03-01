using SevenZip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CSharpComicLoader.NewFileStructure
{
    [XmlInclude(typeof(PageData))]
    [XmlInclude(typeof(ArchivePageData))]
    public interface IPageData
    {
        string GetPageName();
        string GetFullName();
        bool CheckFileExist();
        FileType GetFileType();
        MemoryStream GetPageStream();
    }

    public class PageData : IPageData
    {
        public PageData() { }
        public PageData(string filepath)
        {
            Filepath = filepath;
        }

        public string Filepath { get; private set; }

        public string GetPageName() => Path.GetFileName(Filepath);
        public string GetFullName() => Filepath;
        public bool CheckFileExist() => File.Exists(Filepath);
        public FileType GetFileType() => Utils.GetFileType(Filepath);
        public MemoryStream GetPageStream() => new MemoryStream(File.ReadAllBytes(Filepath));

    }

    public class ArchivePageData : IPageData
    {
        public ArchivePageData() { }
        public ArchivePageData(string archivePath, string filepath)
        {
            ArchivePath = archivePath;
            Filepath = filepath;
        }

        public string ArchivePath { get; private set; }
        public string Filepath { get; private set; }



        public string GetPageName() => Path.GetFileName(Filepath);
        public string GetFullName() => ArchivePath + ", " + Filepath;
        public bool CheckFileExist() => File.Exists(ArchivePath);
        public FileType GetFileType() => Utils.GetFileType(Filepath);
        public MemoryStream GetPageStream()
        {
            using (SevenZipExtractor extractor = new SevenZipExtractor(ArchivePath))
            {
                MemoryStream ms = new MemoryStream();
                extractor.ExtractFile(Filepath, ms);

                return ms;
            }
        }
    }
}
