using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace CSharpComicLoader.OldFileStructure
{

    public class FileViewModel : IFileViewModel
    {
        private FileLoader _fileLoader = new FileLoader();
        private FileNextPrevious _fileNextPrevious = new FileNextPrevious();
        private ComicBook _comicBook;
        private byte[] _imageCache;

        public LoadResult Load(Session session)
        {
            foreach (string file in session.Files)
            {
                if (!System.IO.File.Exists(file) && !Directory.Exists(file))
                {
                    return new LoadResult("One or more archives not found", false);
                }
            }

            _fileLoader.Load(session.Files);
            _comicBook = _fileLoader.LoadedFileData.ComicBook;
            LoadResult result = new LoadResult(error: _fileLoader.Error, hasFile: (_comicBook != null && _comicBook.Count > 0));

            if (!result.HasError)
                _imageCache = _comicBook.GetPage(session.FileNumber, session.PageNumber);

            return result;
        }

        public LoadResult Load(Bookmark bookmark) => Load(new Session(bookmark.Files, bookmark.FileNumber, bookmark.PageNumber));

        public Session CreateSessionFromFiles(string[] files) => new Session(files, 0, 0);
        public Session CreateSessionFromFolder(string path) => new Session(new string[] { path }, 0, 0);

        public string GetPageInfo()
        {
            string ret;

            if (_fileLoader.PageType == PageType.Archive)
            {
                ret = "Archive " + _comicBook.CurrentFileNumber + "/" + _comicBook.TotalFiles +
                        "\r\nArchive name: " + _comicBook.CurrentFile.FileName +
                        "\r\nPage: " + _comicBook.CurrentPageNumber + "/" + _comicBook.TotalPages;
            }
            else
            {
                ret = "File name: " + _comicBook.CurrentFile.FileName +
                        "\r\nPage: " + _comicBook.CurrentPageNumber + "/" + _comicBook.TotalPages;
            }

            return ret;
        }

        public bool IsFileLoaded() => (_comicBook != null) && (_comicBook.TotalFiles != 0);

        public Session GetSession() => _comicBook.GetSession();
        public Bookmark GetBookmark() => _comicBook.GetBookmark();

        public BitmapImage GetImage()
        {
            if (_imageCache == null) return null;
            return ImageUtils.ConverToBitmapImage(_imageCache);
        }

        //public void PointTo(int FileNumber, int PageNumber) => _imageCache = _comicBook.GetPage(FileNumber, PageNumber);

        public void PointToHome() => _imageCache = _comicBook.GetPage(0, 0);
        public void PointToBegin() => _imageCache = _comicBook.GetPage(0);
        public void PointToLast() => _imageCache = _comicBook.GetPage(_comicBook.CurrentFile.TotalPages - 1);
        public void PointToEnd() => _imageCache = _comicBook.GetPage(_comicBook.TotalFiles - 1, _comicBook[_comicBook.TotalFiles - 1].TotalPages - 1);
        public void PointToNextPage() => _imageCache = _comicBook.NextPage() ?? _imageCache;
        public void PointToNextFile() => _imageCache = _comicBook.NextFile() ?? _imageCache;
        public void PointToPreviousPage() => _imageCache = _comicBook.PreviousPage() ?? _imageCache;
        public void PointToPreviousFile() => _imageCache = _comicBook.PreviousFile() ?? _imageCache;

        public bool IsFirstPage() => _comicBook.CurrentPageNumber == 1;
        public bool IsLastPage() => _comicBook.CurrentPageNumber == _comicBook.TotalPages;
        public bool IsFirstFile() => _comicBook.CurrentFileNumber == 1;
        public bool IsLastFile() => _comicBook.CurrentFileNumber == _comicBook.TotalFiles;


        public Session GetOutOfRangeNextSession()
        {
            string file = _fileNextPrevious.GetNextFileInDirectory(_comicBook.CurrentFile.Location);
            return new Session(new string[] { file }, 0, 0);
        }

        public Session GetOutOfRangePreviousSession()
        {
            string file = _fileNextPrevious.GetPreviousFileInDirectory(_comicBook.CurrentFile.Location);
            return new Session(new string[] { file }, 0, 0);
        }

        public string GetCurrentInfo() => _comicBook.CurrentFile.InfoText;
        public string GetCurrentLocation() => _comicBook.CurrentFile.Location;

        public IEnumerable<InfoData> GetAllInfo()
        {
            foreach (ComicFile comicFile in _comicBook)
            {
                if (!string.IsNullOrEmpty(comicFile.InfoText))
                {
                    yield return new InfoData(comicFile.Location, comicFile.InfoText);
                }
            }
        }
    }
}
