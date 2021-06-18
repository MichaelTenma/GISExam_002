using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.UI.Controls;
using GISExam_002.Enum;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GISExam_002
{
    public class LayerService
    {
        private readonly Map _map;
        public ObservableCollection<LayerEntry> LayerEntryList { get; }
        private readonly IDictionary<LayerType, Layer> LayerDict
            = new Dictionary<LayerType, Layer>();

        public LayerService() {
            this._map = new Map();
            this.LayerEntryList = new ObservableCollection<LayerEntry>();
            Init();
        }

        private async void Init() {
            Uri uri = new Uri("pack://application:,,,/data/scau.geodatabase");
            string filename = System.Environment.CurrentDirectory + uri.AbsolutePath;

            Func<string[], Task<List<Layer>>> func = DataLoaderUtil.GetFunc(".geodatabase");
            List<Layer> layerList = await func?.Invoke(new string[] { filename });

            this.AddLayer(layerList);
        }

        public void ClearSelection() {
            foreach (var layer in LayerDict.Values) {
                if (layer is FeatureLayer featureLayer) {
                    featureLayer.ClearSelection();
                }
            }
        }

        public Layer GetLayerByName(LayerType layerEnum) {
            Layer layer = null;
            if (!this.LayerDict.TryGetValue(layerEnum, out layer)) {
                layer = null;
            }
            return layer;
        }

        public FeatureLayer GetFeatureLayerByName(LayerType layerEnum) {
            return this.GetLayerByName(layerEnum) as FeatureLayer;
        }


        private void AddLayer(Layer layer) {
            LayerType layerType = LayerEnum.GetLayerTypeByLayerName(layer.Name);
            if (this.LayerDict.TryGetValue(layerType, out Layer value)) {
                /* 图层名称重复，不再加入 */
            } else {
                this.LayerDict.Add(layerType, layer);
                this.LayerEntryList.Insert(0, new LayerEntry(layerType, layer));
                this._map.OperationalLayers.Add(layer);
            }
        }

        private void AddLayer(IEnumerable<Layer> items) {
            foreach (Layer layer in items) {
                this.AddLayer(layer);
            }
        }

        public void HideOrDisplayLayer(Layer layer) {
            layer.IsVisible = !layer.IsVisible;
        }

        public void MoveLayer(LayerEntry startLayerEntry, LayerEntry endLayerEntry) {
            {
                int startIndex = this._map.OperationalLayers.IndexOf(startLayerEntry.Layer);
                int endIndex = this._map.OperationalLayers.IndexOf(endLayerEntry.Layer);
                this._map.OperationalLayers.Move(startIndex, endIndex);
            }
            {
                int startIndex = this.LayerEntryList.IndexOf(startLayerEntry);
                int endIndex = this.LayerEntryList.IndexOf(endLayerEntry);
                this.LayerEntryList.Move(startIndex, endIndex);
            }
        }

        public Map GetMap() {
            return this._map;
        }
    }
}
