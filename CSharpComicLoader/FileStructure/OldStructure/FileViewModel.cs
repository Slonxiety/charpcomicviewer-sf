using CSharpComicLoader.Comic;
using CSharpComicLoader.File;
using CSharpComicViewer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using static System.Collections.Specialized.BitVector32;

namespace CSharpComicLoader.FileStructure.OldStructure
{
    public class FileViewModel : IFileViewModel
    {
        private FileLoader _fileLoader = new FileLoader();
        private FileNextPrevious _fileNextPrevious = new FileNextPrevious();
        private byte[] _imageCache;
        
        public ComicBook ComicBook { get; private set; }

        public LoadResult Load(string[] files)
        {
            return Load(new Session(files, 0, 0));
        }
        public LoadResult Load(Session session)
        {
            _fileLoader.Load(session.Files);
            ComicBook = _fileLoader.LoadedFileData.ComicBook;
            LoadResult result = new LoadResult(error: _fileLoader.Error, hasFile: (ComicBook != null && ComicBook.Count > 0));

            if (!result.HasError)
                _imageCache = ComicBook.GetPage(session.FileNumber, session.PageNumber);

            return result;
        }

        public string GetPageInfo()
        {
            string ret;

            if (_fileLoader.PageType == PageType.Archive)
            {
                ret = "Archive " + ComicBook.CurrentFileNumber + "/" + ComicBook.TotalFiles + 
                        "\r\nArchive name: " + ComicBook.CurrentFile.FileName + 
                        "\r\nPage: " + ComicBook.CurrentPageNumber + "/" + ComicBook.TotalPages;
            }
            else
            {
                ret = "File name: " + ComicBook.CurrentFile.FileName + 
                        "\r\nPage: " + ComicBook.CurrentPageNumber + "/" + ComicBook.TotalPages;
            }

            return ret;
        }

        public bool IsFileLoaded() => (ComicBook != null) && (ComicBook.TotalFiles != 0);

        public Session GetSession() => ComicBook.GetSession();
        public Bookmark GetBookmark() => ComicBook.GetBookmark();

        public byte[] GetImage() => _imageCache;

        public void PointTo(int FileNumber, int PageNumber) => _imageCache = ComicBook.GetPage(FileNumber, PageNumber);

        public void PointToHome() => _imageCache = ComicBook.GetPage(0, 0);
        public void PointToBegin() => _imageCache = ComicBook.GetPage(0);
        public void PointToLast() => _imageCache = ComicBook.GetPage(ComicBook.CurrentFile.TotalPages - 1);
        public void PointToEnd() => _imageCache = ComicBook.GetPage(ComicBook.TotalFiles - 1, ComicBook[ComicBook.TotalFiles - 1].TotalPages - 1);
        public void PointToNextPage() => _imageCache = ComicBook.NextPage();
        public void PointToNextFile() => _imageCache = ComicBook.NextFile();
        public void PointToPreviousPage() => _imageCache = ComicBook.PreviousPage();
        public void PointToPreviousFile() => _imageCache = ComicBook.PreviousFile();


        public Session GetOutOfRangeNextSession()
        {
            string file = _fileNextPrevious.GetNextFileInDirectory(ComicBook.CurrentFile.Location);
            return new Session(new string[] { file }, 0, 0);
        }

        public Session GetOutOfRangePreviousSession()
        {
            string file = _fileNextPrevious.GetPreviousFileInDirectory(ComicBook.CurrentFile.Location);
            return new Session(new string[] { file }, 0, 0);
        }

        public string GetCurrentInfo() => ComicBook.CurrentFile.InfoText;
        public string GetCurrentLocation() => ComicBook.CurrentFile.Location;

        public IEnumerable<InfoData> GetAllInfo()
        {
            foreach (ComicFile comicFile in ComicBook)
            {
                if (!string.IsNullOrEmpty(comicFile.InfoText))
                {
                    yield return new InfoData(comicFile.Location, comicFile.InfoText);
                }
            }
        }
    }
}
