using SevenZip;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CSharpComicLoader.OldFileStructure;
using System.Collections.ObjectModel;

namespace CSharpComicLoader.NewFileStructure
{

    public class FileStructure
    {
        public FileStructure(Session session)
        {
            this.session = session;
            pageCache = new Dictionary<IPageData, Page>();
        }


        private Dictionary<IPageData, Page> pageCache;
        private IPageContent GetPageContent(IPageData pageData)
        {
            if (pageCache.ContainsKey(pageData)) return pageCache[pageData];
            else                                 return pageCache[pageData] = new Page(pageData);
        }

        private Session session;
        public Session GetSession() => session.Clone();
        public LoadingState GetLoadingState(IPageData pageData)
        {
            if (pageCache.ContainsKey(pageData)) return pageCache[pageData].State;
            else                                 return LoadingState.Unloaded;
        }

        public LoadResult Load(Session session) => session.Reload();

        public CollectionData PointingCollection { get => session.Structure.Pointing; }
        public IPageData PointingPage { get => PointingCollection.Pointing; }
        public IPageContent PointingContent { get => GetPageContent(PointingPage); }


        public int TotalCollection { get => session.Structure.Collections.Count; }
        public int TotalPage { get => PointingCollection.Pages.Count; }
        public int CurrentCollection { get => session.Structure.PointingIndex; }
        public int CurrentPage { get => PointingCollection.PointingIndex; }
        


        public void SetCurrentCollection(int index)
        {

            session.Structure.PointingIndex = index;

            // Load cache

            foreach (IPageData pageData in PointingCollection.Pages)
                GetPageContent(pageData); // load all pages in collection

            //
        }

        public void SetCurrentPage (int index)
        {
            PointingCollection.PointingIndex = index;

            // Load cache


            //
        }




        public void SortCollection (Comparison<CollectionData> cmp)
        {
            CollectionData pointing = PointingCollection;
            session.Structure.Collections.Sort(cmp);
            if (pointing != null)
                session.Structure.PointingIndex = session.Structure.Collections.IndexOf(pointing);
        }
        
        


        public void PointToHome()
        {
            SetCurrentCollection(0);
            SetCurrentPage(0);
        }
        public void PointToBegin() => SetCurrentPage(0);
        public void PointToLast() => SetCurrentPage(TotalPage - 1);

        public void PointToEnd()
        {
            SetCurrentCollection(TotalCollection - 1);
            SetCurrentPage(TotalPage - 1);
        }

        public void PointToNextCollection() => SetCurrentCollection(CurrentCollection + 1);
        public void PointToPreviousCollection() => SetCurrentCollection(CurrentCollection - 1);
        public void PointToNextPage()
        {
            if (CurrentPage < TotalPage - 1) SetCurrentPage(CurrentPage + 1);
            else                             SetCurrentCollection(CurrentCollection + 1);
        }
        public void PointToPreviousPage()
        {
            if (CurrentPage > 1) SetCurrentPage(CurrentPage - 1);
            else                 SetCurrentCollection(CurrentCollection - 1);
        }


        public Bookmark GetBookmark()
        {
            Bookmark result = null;

            return result;
            //var collection = PointingCollection;
            //List<string> fileLocations = new List<string>();
            //foreach (var file in collection.GetElements())
            //{
            //    fileLocations.Add(comicFile.Location);
            //}
            //
            //result = new Bookmark(fileLocations.ToArray(), this.IndexOf(CurrentFile), CurrentFile.CurrentPageNumber - 1);
            //return result;
        }
    }

    
}
