using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Mapping;
using GISExam_002.Enum;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace GISExam_002 {
    /// <summary>
    /// BufferAnalysis.xaml 的交互逻辑
    /// </summary>
    public partial class BufferAnalysis : Window
    {
        private BufferService bufferService;
        private LayerService layerService;
        private ObservableCollection<FeatureEntry> FeatureEntryList { get; } = new ObservableCollection<FeatureEntry>();

        public delegate void UpdateSelectedDataInMap(Geometry geometry);
        public event UpdateSelectedDataInMap UpdateSelectedDataInMapEvent;

        public BufferAnalysis()
        {
            this.bufferService = IOC.Get<BufferService>();
            this.layerService = IOC.Get<LayerService>();
            InitializeComponent();

            POILayerComboBox.ItemsSource = POITypeEnum.GetKeys();
            POIListView.ItemsSource = FeatureEntryList;
        }

        private async void POILayerComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (POILayerComboBox.SelectedItem is POIType poiType) {
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

        private void POIListViewSelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (POIListView.SelectedItem is FeatureEntry featureEntry) {
                if (featureEntry.Feature.FeatureTable.Layer is FeatureLayer featureLayer) {
                    featureLayer.ClearSelection();
                    featureLayer.SelectFeature(featureEntry.Feature);
                    this.UpdateSelectedDataInMapEvent?.Invoke(featureEntry.Feature.Geometry);
                }
            }
        }

        private async void Button_Click(object sender, RoutedEventArgs e) {
            if (
                BufferDistanceTextBox.Text is string value && !value.Equals("") &&
                double.TryParse(value, out double distance) &&
                POIListView.SelectedValue is FeatureEntry featureEntry
            ) {
                distance = Math.Abs(distance);

                Geometry selectGeometry = featureEntry.Feature.Geometry;
                
                //执行缓冲操作
                Geometry resultBuffer = GeometryEngine.Buffer(selectGeometry, distance);
                //this.bufferService.ClearBuffer();
                this.bufferService.AddGeometry(resultBuffer);

                FeatureLayer StudentDormitory = this.layerService.GetFeatureLayerByName(LayerEnum.StudentDormitory);

                FeatureQueryResult featureQueryResult 
                    = await StudentDormitory.FeatureTable.QueryFeaturesAsync(new QueryParameters());

                IEnumerable<Feature> featureList = featureQueryResult.Where((feature) => {
                    return GeometryEngine.Intersects(resultBuffer, feature.Geometry);
                });

                //this.layerService.ClearSelection();
                StudentDormitory.SelectFeatures(featureList);

                int population = featureList.Sum((feature) => {
                    return Convert.ToInt32(feature.GetAttributeValue("已住人数").ToString());
                });

                MessageBox.Show($"缓冲区内有{population}人居住！");
            }
        }

        private void Clear_Click(object sender, RoutedEventArgs e) {
            this.layerService.ClearSelection();
            this.bufferService.ClearBuffer();
        }
    }
}
