using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpComicLoader.FileStructure
{
    public class InfoData
    {
        public string Title { get; }
        public string Content { get; }
        public InfoData(string title, string content)
        {
            Title = title;
            Content = content;
        }
    }
}
