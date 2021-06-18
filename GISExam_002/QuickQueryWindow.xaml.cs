using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Mapping;
using GISExam_002.Enum;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

namespace GISExam_002 {
    /// <summary>
    /// QuickQueryWindow.xaml 的交互逻辑
    /// </summary>
    public partial class QuickQueryWindow : Window
    {
        private LayerService layerService;
        private ObservableCollection<FeatureEntry> FeatureEntryList { get; } = new ObservableCollection<FeatureEntry>();

        public delegate void UpdateSelectedDataInMap(Geometry geometry);
        public event UpdateSelectedDataInMap UpdateSelectedDataInMapEvent;

        public QuickQueryWindow()
        {
            this.layerService = IOC.Get<LayerService>();
            InitializeComponent();

            POILayerListView.ItemsSource = POITypeEnum.GetKeys();
            POIListView.ItemsSource = FeatureEntryList;
        }

        private async void POILayerListViewSelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e) {
            if (POILayerListView.SelectedItem is POIType poiType) {
                string fieldName = poiType.FieldName;
                FeatureLayer featureLayer = this.layerService.GetFeatureLayerByName(poiType);
                if (featureLayer != null) {
                    FeatureQueryResult featureQueryResult = 
                        await featureLayer.FeatureTable.QueryFeaturesAsync(new QueryParameters());
                    
                    IDictionary<string, FeatureEntry> valueDict = new Dictionary<string, FeatureEntry>();
                    foreach (Feature feature in featureQueryResult) {
                        if (feature.GetAttributeValue(fieldName) is string value) {
                            valueDict.TryGetValue(value, out FeatureEntry featureEntry);
                            if (featureEntry == null) {
                                valueDict.Add(value, new FeatureEntry(value, feature));
                            }
                        }
                    }

                    FeatureEntryList.Clear();
                    foreach (FeatureEntry featureEntry in valueDict.Values) {
                        FeatureEntryList.Add(featureEntry);
                    }
                }
            }
        }

        private void POIListViewSelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e) {
            if (POIListView.SelectedItem is FeatureEntry featureEntry) {
                if (featureEntry.Feature.FeatureTable.Layer is FeatureLayer featureLayer) {
                    featureLayer.ClearSelection();
                    featureLayer.SelectFeature(featureEntry.Feature);
                    this.UpdateSelectedDataInMapEvent?.Invoke(featureEntry.Feature.Geometry);
                }
            }
        }
    }
}
