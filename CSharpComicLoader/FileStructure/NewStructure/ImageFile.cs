using SevenZip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpComicLoader.File
{
    public enum ImageState
    {
        Unloaded,
        Loaded,
        Corrupted,
        FileNotExist
    }

    public interface IReadOnlyImageFile
    {
        MemoryStream Stream { get; }
        ImageState State { get; }
        string GetName();
    }

    public interface IImageFile : IDisposable, IReadOnlyImageFile
    {
        void LoadImage();
        void UnloadImage();
    }
    /// <summary>
	/// A class to store memorystream of a corresponding image file.
	/// </summary>
    public class ImageFile : IImageFile
    {
        public ImageFile(string filePath)
        {
            FilePath = filePath;
            State = ImageState.Unloaded;
            _fileName = Path.GetFileName(FilePath);
        }

        private string _fileName;

        public string FilePath { get; }
        public MemoryStream Stream { get; private set; }
        public ImageState State { get; private set; }


        public void LoadImage()
        {
            // Dispose current image holded
            UnloadImage();

            if (!System.IO.File.Exists(FilePath))
            {
                State = ImageState.FileNotExist;
                return;
            }

            // Load image
            Stream = new MemoryStream();
            try
            {
                using (FileStream fs = new FileStream(FilePath, FileMode.Open, FileAccess.Read))
                    fs.CopyTo(Stream);
                State = ImageState.Loaded;
            }
            catch
            {
                Stream?.Dispose();
                State = ImageState.Corrupted;
            }
        }
        public void UnloadImage()
        {
            Stream?.Dispose();
            State = ImageState.Unloaded;
        }

        public string GetName() => _fileName;
        
        public void Dispose() => Stream?.Dispose();
    }

    public class ArchiveImageFile : IImageFile
    {
        public ArchiveImageFile(string archivePath, string innerPath)
        {
            ArchivePath = archivePath;
            InnerPath = InnerPath;
            _fileName = Path.GetFileName(innerPath);
        }

        private string _fileName;

        public string ArchivePath { get; }
        public string InnerPath { get; private set; }
    
        public MemoryStream Stream { get; private set; }
        public ImageState State { get; private set; }

        public void LoadImage()
        {
            // Dispose current image holded
            UnloadImage();

            // Load image
            if (!System.IO.File.Exists(ArchivePath))
            {
                State = ImageState.FileNotExist;
                return;
            }

            try
            {
                using (SevenZipExtractor extractor = new SevenZipExtractor(ArchivePath))
                {
                    Stream = new MemoryStream();
                    extractor.ExtractFile(InnerPath, Stream);
                }
            }
            catch
            {
                Stream?.Dispose();
                State = ImageState.Corrupted;
            }
            
        }
        public void UnloadImage()
        {
            Stream?.Dispose();
            State = ImageState.Unloaded;
        }

        public string GetName() => _fileName;
        public void Dispose() => Stream?.Dispose();
    }
}
