using SevenZip;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CSharpComicLoader.OldFileStructure;

namespace CSharpComicLoader.NewFileStructure
{
    public class FileStructure
    {
        public FileStructure()
        {
            Collections = new List<FileCollection>();
            filter = new string[] { };
        }

        private string[] filter;
        private List<FileCollection> Collections { get; }

        private Comparison<IImageFile> default_file_cmp = (x, y) => string.Compare(x.GetName(), y.GetName());
        private Comparison<FileCollection> default_collection_cmp = (x, y) => string.Compare(x.GetName(), y.GetName());

        public string RootDirectory { get; private set; }
        public int indexCollection { get; private set; }
        public int indexFile { get; private set; }
        public string Error { get; private set; }
        public bool IsEmpty { get => Collections.Count == 0; }
        public IReadOnlyList<IReadOnlyFileCollection> GetCollections() => Collections.AsReadOnly();
        public IReadOnlyImageFile PointingFile
        { 
            get
            {
                if (Collections.Count == 0 || Collections[indexCollection].Elements.Count == 0) return null;
                return Collections[indexCollection].Elements[indexFile];
            }
        }
        public IReadOnlyFileCollection PointingCollection
        {
            get
            {
                if (Collections.Count == 0) return null;
                return Collections[indexCollection];
            }
        }


        
        private bool IsSubDirectory (string main, string sub)
        {
            string[] mainpaths = main.Split(new char[] { '\\', '/' }, StringSplitOptions.RemoveEmptyEntries);
            string[] subpaths = sub.Split(new char[] { '\\', '/' }, StringSplitOptions.RemoveEmptyEntries);
            
            for (int i = 0; i < subpaths.Length; i++)
                if (subpaths[i] != mainpaths[i])
                    return false;

            return true;
        }

        public void Sort(Comparison<FileCollection> collection_cmp = null, Comparison<IImageFile> file_cmp = null)
        {
            if (collection_cmp == null) collection_cmp = default_collection_cmp;
            if (file_cmp == null) file_cmp = default_file_cmp;

            IImageFile pointing = (PointingFile == null)? null : Collections[indexCollection].Elements[indexFile];

            Collections.Sort(collection_cmp);

            foreach (var collection in Collections)
                collection.Elements.Sort(file_cmp);

            if (pointing != null)
            {
                indexCollection = Collections.FindIndex(x => x.Elements.Contains(pointing));
                indexFile = Collections[indexCollection].Elements.IndexOf(pointing);
            }
            else
            {
                indexCollection = 0;
                indexFile = 0;
            }
        }
        public void Clear()
        {
            foreach (var collection in Collections)
                collection.Dispose();
            Collections.Clear();
        }
        public void Load (string rootDirectory, string[] filter_allow = null)
        {
            // Clear loaded files
            if (filter.Length > 0 || rootDirectory != RootDirectory)
                Clear();
            RootDirectory = rootDirectory;

            // Clear error
            Error = string.Empty;

            // Unify empty filter cases
            if (filter_allow == null)
                filter_allow = new string[] { };

            // Set filter
            filter_allow.CopyTo(filter, 0);

            // Load files
            string[] allFiles = Directory.GetFiles(rootDirectory, "*.*", SearchOption.AllDirectories);

            var filterFiles = allFiles.Where(x =>
            {
                if (filter_allow.Length == 0) return true;

                foreach (string filter in filter_allow)
                    if (IsSubDirectory(x, filter))
                        return true;

                return false;
            });

            // Used for later getting the collection path
            int subpathFrom = rootDirectory.Split(new char[] { '\\', '/' }, StringSplitOptions.RemoveEmptyEntries).Length;

            foreach (string filepath in filterFiles)
            {
                // Get the collection

                string[] fileCutPaths = filepath.Split(new char[] { '\\', '/' }, StringSplitOptions.RemoveEmptyEntries);
                string collectionPath = Path.Combine(rootDirectory, fileCutPaths[subpathFrom]);

                int id = Collections.FindIndex(x => x.CollectionPath == collectionPath);
                if (id == -1)
                {
                    id = Collections.Count;
                    Collections.Add(new FileCollection(collectionPath));
                }

                var collection = Collections[id];

                // Add file/files into collection

                //FileAttributes attr = System.IO.File.GetAttributes(file);

                if (Utils.ValidateArchiveFileExtension(filepath))
                {
                    // Load all files in zip
                    try
                    {
                        SevenZip.LoadSevenZip();
                        using (SevenZipExtractor extractor = new SevenZipExtractor(filepath))
                        {
                            string[] fileNames = extractor.ArchiveFileNames.ToArray();

                            foreach (string zipfile in fileNames)
                                if (Utils.ValidateImageFileExtension(zipfile) || (Utils.ValidateTextFileExtension(zipfile)))
                                {
                                    collection.Elements.Add(new ArchiveImageFile(filepath, zipfile));
                                }
                        }
                    }
                    catch (Exception exception)
                    {
                        if (Error != "") Error += "\n";
                        Error += exception;
                    }
                }
                else if (Utils.ValidateImageFileExtension(filepath) || (Utils.ValidateTextFileExtension(filepath)))
                {
                    collection.Elements.Add(new ImageFile(filepath));
                }

                if (collection.Elements.Count == 0)
                {
                    Collections.Remove(collection);
                    collection.Dispose();
                }
            }

            Sort();
        }
    
        public void PointTo (string path)
        {
            if (Collections.Count == 0) return;

            int newIndexCollection = Collections.FindIndex(x => IsSubDirectory(path, x.CollectionPath));
            if (newIndexCollection == -1) return;
            indexCollection = newIndexCollection;

            string name = Path.GetFileName(path);
            int newIndexFile = Collections[indexCollection].Elements.FindIndex(x => x.GetName() == name);
            if (newIndexFile != -1)
                indexFile = newIndexFile;
        }

        public void PointTo (int _indexCollection, int _indexFile = 0)
        {
            // Check valid
            if (_indexCollection < 0 || _indexCollection >= Collections.Count) return;
            if (_indexFile < 0 || _indexFile >= Collections[_indexCollection].Elements.Count) return;

            indexCollection = _indexCollection;
            indexFile = _indexFile;
        }

        public void PointToNextCollection() => PointTo(indexCollection + 1);
        public void PointToPreviousCollection() => PointTo(indexCollection - 1);
        public void PointToNextFile() => PointTo(indexCollection, indexFile + 1);
        public void PointToPreviousFile() => PointTo(indexCollection, indexFile - 1);

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

    public static class SevenZip
    {
        private static bool sevenziploaded = false;
        public static void LoadSevenZip()
        {
            if (sevenziploaded) return;
            //Get the location of the 7z dll (location .EXE is in)
            String executableName = System.Reflection.Assembly.GetExecutingAssembly().Location;
            FileInfo executableFileInfo = new FileInfo(executableName);

            string dllPath = executableFileInfo.DirectoryName + "//7z.dll";

            //load the 7zip dll
            try
            {
                SevenZipExtractor.SetLibraryPath(dllPath);
            }
            catch (Exception e)
            {
                throw new Exception(String.Format("Unable to load 7z.dll from: {0}", dllPath), e.InnerException);
            }
            sevenziploaded = true;
        }
    }
}
