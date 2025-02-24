using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpComicLoader.NewFileStructure
{
    public interface IReadOnlyFileCollection
    {
        string CollectionPath { get; }
        IReadOnlyList<IReadOnlyImageFile> GetElements();

        string GetName();
    }
    
    public class FileCollection : IReadOnlyFileCollection, IDisposable
    {
        public FileCollection (string collectionPath)
        {
            CollectionPath = collectionPath;
            Elements = new List<IImageFile>();
            _name = Path.GetFileName(CollectionPath);
        }

        private string _name;

        public List<IImageFile> Elements { get; }
        public string CollectionPath { get; }
        public IReadOnlyList<IReadOnlyImageFile> GetElements() => Elements.AsReadOnly();
        public string GetName() => _name;

        public void Dispose()
        {
            foreach (var element in Elements)
                element.Dispose();
        }
    }
}
