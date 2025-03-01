using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CSharpComicLoader.NewFileStructure
{
    public class StructureData
    {
        public StructureData() 
        {
            Collections = new List<CollectionData>();
            PointingIndex = 0;
        }

        public List<CollectionData> Collections { get; private set; }

        private int pointingIndex;
        public int PointingIndex 
        {
            get { return pointingIndex; }
            set { if (value >= 0) pointingIndex = value; }
        }

        public CollectionData Pointing
        {
            get => (Collections.Count > 0 && pointingIndex < Collections.Count) ? Collections[pointingIndex] : null;
        }
    }
}
