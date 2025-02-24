﻿using CSharpComicLoader.Comic;
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
        private ComicBook _comicBook;
        private byte[] _imageCache;
        
        public LoadResult Load(Session session)
        {
            _fileLoader.Load(session.Files);
            _comicBook = _fileLoader.LoadedFileData.ComicBook;
            LoadResult result = new LoadResult(error: _fileLoader.Error, hasFile: (_comicBook != null && _comicBook.Count > 0));

            if (!result.HasError)
                _imageCache = _comicBook.GetPage(session.FileNumber, session.PageNumber);

            return result;
        }

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

        public byte[] GetImage() => _imageCache;

        //public void PointTo(int FileNumber, int PageNumber) => _imageCache = _comicBook.GetPage(FileNumber, PageNumber);

        public void PointToHome() => _imageCache = _comicBook.GetPage(0, 0);
        public void PointToBegin() => _imageCache = _comicBook.GetPage(0);
        public void PointToLast() => _imageCache = _comicBook.GetPage(_comicBook.CurrentFile.TotalPages - 1);
        public void PointToEnd() => _imageCache = _comicBook.GetPage(_comicBook.TotalFiles - 1, _comicBook[_comicBook.TotalFiles - 1].TotalPages - 1);
        public void PointToNextPage() => _imageCache = _comicBook.NextPage();
        public void PointToNextFile() => _imageCache = _comicBook.NextFile();
        public void PointToPreviousPage() => _imageCache = _comicBook.PreviousPage();
        public void PointToPreviousFile() => _imageCache = _comicBook.PreviousFile();


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
