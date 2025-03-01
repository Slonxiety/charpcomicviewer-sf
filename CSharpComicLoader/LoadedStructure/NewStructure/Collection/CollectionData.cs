using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CSharpComicLoader.NewFileStructure
{
    public class CollectionData
    {
        public CollectionData() { }
        public CollectionData(string name)
        {
            Name = name;
            Pages = new List<IPageData>();
            PointingIndex = 0;
        }

        public string Name { get; private set; }
        public List<IPageData> Pages { get; private set; }


        private int pointingIndex;
        public int PointingIndex
        {
            get { return pointingIndex; }
            set { if (value >= 0) pointingIndex = value; }
        }

        public IPageData Pointing
        {
            get => (Pages.Count > 0 && pointingIndex < Pages.Count) ? Pages[pointingIndex] : null;
        }

    }
}
