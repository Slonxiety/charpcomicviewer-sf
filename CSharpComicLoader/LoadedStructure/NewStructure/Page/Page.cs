using SevenZip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace CSharpComicLoader.NewFileStructure
{
    public enum LoadingState
    {
        Unloaded,
        Loaded,
        Corrupted,
        Missing,
    }


    public interface IPageContent
    {
        FileType FileType { get; }
        LoadingState State { get; }
        object Content { get; }
        string ErrorMessage { get; }
    }


    public class Page : IPageContent, IDisposable
    {
        public Page (IPageData pageData)
        {
            PageData = pageData;
            Reload();
        }

        public IPageData PageData { get; private set; }
        public LoadingState State { get; private set; }
        public FileType FileType { get; private set; }
        public object Content { get; private set; }
        public string ErrorMessage { get; private set; }

        public void Reload()
        {
            if (!PageData.CheckFileExist())
            {
                State = LoadingState.Missing;
                return;
            }

            ErrorMessage = "";
            State = LoadingState.Unloaded;

            try
            {
                using (var ms = PageData.GetPageStream())
                {
                    ms.Position = 0;

                    FileType = PageData.GetFileType();

                    if (FileType == FileType.Image)
                    {
                        BitmapImage Image = new BitmapImage();
                        Image.BeginInit();
                        Image.CacheOption = BitmapCacheOption.OnLoad;
                        Image.StreamSource = ms;
                        Image.EndInit();
                        Image.Freeze();

                        Content = Image;
                        State = LoadingState.Loaded;
                    }
                    else if (FileType == FileType.Text)
                    {
                        string content = "";
                        string line = "";
                        using (var reader = new StreamReader(ms, Encoding.UTF8))
                            while ((line = reader.ReadLine()) != null)
                                content += line;

                        Content = content;
                        State = LoadingState.Loaded;
                    }
                }
            }
            catch (Exception e)
            {
                ErrorMessage = e.Message;
                State = LoadingState.Corrupted;
            }
        }

        public void Dispose() => Content = null;
    }

}
