using CSharpComicLoader.OldFileStructure;
using SevenZip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CSharpComicLoader.NewFileStructure
{
    public class Session
    {
        public Session(SessionToken token, StructureData structure)
        {
            Token = token;
            Structure = structure;
        }

        public SessionToken Token { get; private set; }
        public StructureData Structure { get; private set; }


        public LoadResult Reload()
        {
            string error = "";

            // Load files
            string[] allFiles = Directory.GetFiles(Token.RootDirectory, "*.*", SearchOption.AllDirectories);


            var filterFiles = allFiles.Where(x =>
            {
                // Check pass filter
                if (Token.PassFilter.Length != 0)
                {
                    bool pass = false;
                    foreach (string filter in Token.PassFilter)
                        if (IsSubDirectory(x, filter))
                            pass = true;

                    if (!pass) return false;
                }

                // Check deny filter
                foreach (string filter in Token.DenyFilter)
                    if (IsSubDirectory(x, filter))
                        return false;

                return true;
            }).ToArray();


            // Used for later getting the collection path
            int subpathFrom = Token.RootDirectory.Split(new char[] { '\\', '/' }, StringSplitOptions.RemoveEmptyEntries).Length;

            int totalfileloaded = 0;
            foreach (string filepath in filterFiles)
            {
                // Get the collection

                string[] fileCutPaths = filepath.Split(new char[] { '\\', '/' }, StringSplitOptions.RemoveEmptyEntries);
                string collectionName = fileCutPaths[subpathFrom];

                int id = Structure.Collections.FindIndex(x => x.Name == collectionName);
                if (id == -1)
                {
                    id = Structure.Collections.Count;
                    Structure.Collections.Add(new CollectionData(collectionName));
                }

                var collection = Structure.Collections[id];

                // Add file/files into collection

                FileType type = Utils.GetFileType(filepath);

                if (type == FileType.Archive)
                {
                    // Load all files in zip
                    try
                    {
                        SevenZip.LoadSevenZip();
                        using (SevenZipExtractor extractor = new SevenZipExtractor(filepath))
                        {
                            string[] fileNames = extractor.ArchiveFileNames.ToArray();

                            foreach (string zipfile in fileNames)
                            {
                                FileType zipfiletype = Utils.GetFileType(zipfile);
                                if (zipfiletype == FileType.Image || zipfiletype == FileType.Text)
                                {
                                    collection.Pages.Add(new ArchivePageData(filepath, zipfile));
                                    totalfileloaded++;
                                }
                            }
                        }
                    }
                    catch (Exception exception)
                    {
                        if (error != "") error += "\n";
                        error += exception;
                    }
                }
                else if (type == FileType.Image || type == FileType.Text)
                {
                    collection.Pages.Add(new PageData(filepath));
                    totalfileloaded++;
                }
            }

            Structure.Collections.RemoveAll(x => x.Pages.Count == 0);

            return new LoadResult(error, totalfileloaded > 0);
        }

        public Session Clone()
        {
            return this; // To be completed
        }

        private bool IsSubDirectory(string main, string sub)
        {
            string[] mainpaths = main.Split(new char[] { '\\', '/' }, StringSplitOptions.RemoveEmptyEntries);
            string[] subpaths = sub.Split(new char[] { '\\', '/' }, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < subpaths.Length; i++)
                if (subpaths[i] != mainpaths[i])
                    return false;

            return true;
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
