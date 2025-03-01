using CSharpComicLoader.OldFileStructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;

namespace CSharpComicLoader.NewFileStructure
{
    public class FileViewModel
    {
        private FileStructure fileStructure;

        public Session CreateSessionFromFiles(string[] files)
        {
            string root = Directory.GetParent(files[0]).Parent.FullName; //Parent of parent

            SessionToken token = new SessionToken(root, files, new string[] {});
            StructureData structureData = new StructureData();

            return new Session(token, structureData);
        }

        public Session CreateSessionFromFolder(string path)
        {
            string root = Directory.GetParent(path).FullName; //Parent of parent

            SessionToken token = new SessionToken(root, new string[] {}, new string[] { });
            StructureData structureData = new StructureData();

            return new Session(token, structureData);
        }

        public IEnumerable<InfoData> GetAllInfo()
        {
            throw new NotImplementedException();
        }

        public Bookmark GetBookmark()
        {
            throw new NotImplementedException();
        }

        public string GetCurrentInfo()
        {
            throw new NotImplementedException();
        }

        public string GetCurrentLocation() => fileStructure.PointingPage.GetFullName();

        public IPageContent GetImage() => fileStructure.PointingContent;

        public Session GetOutOfRangeNextSession() => null;

        public Session GetOutOfRangePreviousSession() => null;

        public string GetPageInfo()
        {
            string ret = string.Format("Collection: {0} / {1}\r\n" +
                                       "Page: {2} / {3}\r\n" +
                                       "Page Location: {4}",
                                       fileStructure.CurrentCollection, fileStructure.TotalCollection,
                                       fileStructure.CurrentPage, fileStructure.TotalPage,
                                       fileStructure.PointingPage.GetFullName());
            return ret;
        }

        public Session GetSession() => fileStructure.GetSession();

        public bool IsFileLoaded() => (fileStructure != null) && (fileStructure.TotalCollection > 0);

        public bool IsFirstFile() => fileStructure.CurrentCollection == 0;

        public bool IsFirstPage() => fileStructure.CurrentPage == 0;

        public bool IsLastFile() => fileStructure.CurrentCollection == fileStructure.TotalCollection - 1;

        public bool IsLastPage() => fileStructure.CurrentPage == fileStructure.TotalPage - 1;

        public LoadResult Load(Bookmark bookmark)
        {
            throw new NotImplementedException();
        }

        public LoadResult Load(Session session) => fileStructure.Load(session);

        public void PointToHome() => fileStructure.PointToHome();
        public void PointToBegin() => fileStructure.PointToBegin();
        public void PointToLast() => fileStructure.PointToLast();
        public void PointToEnd() => fileStructure.PointToEnd();

        public void PointToNextFile() => fileStructure.PointToNextCollection();
        public void PointToNextPage() => fileStructure.PointToNextPage();
        public void PointToPreviousFile() => fileStructure.PointToPreviousCollection();
        public void PointToPreviousPage() => fileStructure.PointToPreviousPage();
    }
}
