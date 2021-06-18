using Esri.ArcGISRuntime.Data;
using GISExam_002.Enum;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace GISExam_002 {
    /// <summary>
    /// QueryResult.xaml 的交互逻辑
    /// </summary>
    public partial class QueryResult : Window
    {
        private IDictionary<LayerType, Feature> results;

        public QueryResult(List<FeatureQueryResult> featureQueryResultList) {
            InitializeComponent();

            this.results = new Dictionary<LayerType, Feature>();
            foreach (FeatureQueryResult featureQueryResult in featureQueryResultList) {
                Feature feature = featureQueryResult.FirstOrDefault();
                if (feature != default(Feature) && feature != null) {
                    string layerName = feature.FeatureTable.Layer.Name;
                    LayerType layerType = LayerEnum.GetLayerTypeByLayerName(layerName);
                    this.results.Add(layerType, feature);
                }
            }

            comboBoxSelectionCount.ItemsSource = this.results.Keys;
            comboBoxSelectionCount.SelectedIndex = 0;
        }

        private void ComboBoxSelectionCount_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (comboBoxSelectionCount.SelectedItem is LayerType value) {
                results.TryGetValue(value, out Feature feature);
                dataGridResult.ItemsSource = feature.Attributes;
            }
        }
    }
}
