using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using CSharpComicLoader.OldFileStructure;

namespace CSharpComicLoader
{
    public interface IFileViewModel
    {
        Session CreateSessionFromFiles(string[] files);
        Session CreateSessionFromFolder(string path);
        IEnumerable<InfoData> GetAllInfo();
        Bookmark GetBookmark();
        string GetCurrentInfo();
        string GetCurrentLocation();
        BitmapImage GetImage();
        Session GetOutOfRangeNextSession();
        Session GetOutOfRangePreviousSession();
        string GetPageInfo();
        Session GetSession();
        bool IsFileLoaded();
        bool IsFirstFile();
        bool IsFirstPage();
        bool IsLastFile();
        bool IsLastPage();
        LoadResult Load(Bookmark bookmark);
        LoadResult Load(Session session);
        void PointToBegin();
        void PointToEnd();
        void PointToHome();
        void PointToLast();
        void PointToNextFile();
        void PointToNextPage();
        void PointToPreviousFile();
        void PointToPreviousPage();
    }
}
