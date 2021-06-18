using Esri.ArcGISRuntime.Mapping;
using GISExam_002.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GISExam_002
{
    public class LayerEntry
    {
        public Layer Layer { get; }
        public LayerType LayerType { get; }
        public bool IsChecked { get; set; }

        public LayerEntry(LayerType layerType, Layer layer) {
            this.Layer = layer;
            this.LayerType = layerType;
            this.IsChecked = true;
        }

        public override string ToString() {
            return this.LayerType.Name;
        }

        public bool ChangeIsChecked() {
            this.IsChecked = !this.IsChecked;
            return this.IsChecked;
        }
    }
}
