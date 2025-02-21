using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpComicLoader.FileStructure
{
    public class LoadResult
    {
        public string Error { get; }
        public bool HasFile { get; }

        public bool HasError { get => !string.IsNullOrEmpty(Error); }
        
        public LoadResult(string error, bool hasFile)
        {
            Error = error;
            HasFile = hasFile;
        }

    }
}
