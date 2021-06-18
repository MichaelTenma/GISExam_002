using Esri.ArcGISRuntime.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GISExam_002
{
    class FeatureEntry {
        public Feature Feature { get; }
        public string Name { get; }

        public FeatureEntry(string name, Feature feature) {
            this.Feature = feature;
            this.Name = name;
        }

        public override string ToString() {
            return this.Name;
        }
    }
}
