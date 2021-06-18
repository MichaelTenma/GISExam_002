using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.UI;
using Esri.ArcGISRuntime.UI.Controls;
using GISExam_002.Utils;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GISExam_002 {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private MapViewModel mapViewModel;
        private LayerService layerService;
        public MainWindow() {
            InitializeComponent();

            /* 初始化依赖注入模块 */
            IOC.Add(MainMapView);
            //this.mapViewModel = new MapViewModel();
            this.mapViewModel = IOC.Get<MapViewModel>();
            this.layerService = IOC.Get<LayerService>();
            this.DataContext = mapViewModel;

            LayerListView.ItemsSource = this.layerService.LayerEntryList;
            MeasureToolbar.MapView = MainMapView;

            InitLayerListViewDrag();

            //BufferService bufferService = DependencyInjectionUtil.Get<BufferService>();
            //bufferService.BindGraphicsOverlay(MainMapView);

            //LayerService layerService = DependencyInjectionUtil.Get<LayerService>();
            //layerService.BindMapView(MainMapView);
        }


        private void LoadNetworkDataset(object sender, RoutedEventArgs e) {

        }

        private void LayerEntryCheckboxChecked(object sender, RoutedEventArgs e) {
            if (sender is CheckBox checkBox) {
                if (checkBox.Content is LayerEntry layerEntry) {
                    layerEntry.ChangeIsChecked();
                    this.layerService.HideOrDisplayLayer(layerEntry.Layer);
                }
            }
            
        }

        private LayerEntry GetPointReferenceLayerEntry(System.Windows.Point point) {
            LayerEntry layerEntry = null;
            HitTestResult hitTestResult = VisualTreeHelper.HitTest(LayerListView, point);
            if (hitTestResult != null && hitTestResult.VisualHit is TextBlock textBlock) {
                layerEntry = textBlock.DataContext as LayerEntry;
            }
            return layerEntry;
        }


        private void InitLayerListViewDrag() {

            LayerListView.AllowDrop = true;
            LayerEntry startLayerEntry = null;
            LayerListView.PreviewMouseLeftButtonDown += (object sender, MouseButtonEventArgs e) => {
                startLayerEntry = null;
                // 获得起始点
                System.Windows.Point dragStartPoint = e.GetPosition(LayerListView);
                startLayerEntry = GetPointReferenceLayerEntry(dragStartPoint);
            };
            LayerListView.PreviewMouseLeftButtonUp += (object sender, MouseButtonEventArgs e) => {
                LayerEntry endLayerEntry = null;
                if (startLayerEntry == null) return;

                // 获取结束点
                System.Windows.Point dragEndPoint = e.GetPosition(LayerListView);
                if (dragEndPoint.X < 20) return;
                endLayerEntry = GetPointReferenceLayerEntry(dragEndPoint);

                if (endLayerEntry == null) return;

                /* 移除当前图层 */
                if (!startLayerEntry.Equals(endLayerEntry)) {
                    this.layerService.MoveLayer(startLayerEntry, endLayerEntry);
                }

                /* 还原值 */
                startLayerEntry = endLayerEntry;
            };
        }

        private void QuickQuery(object sender, RoutedEventArgs e) {

            QuickQueryWindow quickQueryWindow = new QuickQueryWindow();

            quickQueryWindow.UpdateSelectedDataInMapEvent += 
                async (Esri.ArcGISRuntime.Geometry.Geometry geometry) => {
                    await MainMapView.SetViewpointGeometryAsync(geometry.Extent);
                };

            //queryByAttributesWindow.updateSelectedDataInMap += async (maxExtent) => {
            //    await MainMapView.SetViewpointGeometryAsync(maxExtent, 50);
            //};

            //queryByAttributesWindow.showTable += (FeatureTable featureTable, IEnumerable<Feature> featureList) => {
            //    //AttributesTableWindow attributesTableWindow = new AttributesTableWindow(featureTable);
            //    attributesTableWindow.UpdateAndShowFeatureList(featureTable, featureList);

            //    attributesTableWindow.Activate();
            //    attributesTableWindow.Show();
            //};
            quickQueryWindow.Activate();
            quickQueryWindow.Show();
        }

        private void ClickSelect(object sender, RoutedEventArgs e) {
            MouseButtonEventHandler handler = async delegate (object innerSender, MouseButtonEventArgs inner) {
                // 获取当前图层管理器中所选择的图层
                MapView mapView = (MapView)innerSender;
                System.Windows.Point point = inner.GetPosition(mapView);
                MapPoint mapPoint = EnvelopeUtil.SrceenPointToMapPoint(mapView, point);

                QueryParameters queryParameters = new QueryParameters {
                    Geometry = GeometryEngine.Buffer(mapPoint, 3 * mapView.UnitsPerPixel),
                    SpatialRelationship = SpatialRelationship.Intersects
                };

                List<FeatureQueryResult> featureQueryResultList = new List<FeatureQueryResult>();
                foreach (Layer layer in this.mapViewModel.Map.OperationalLayers) {
                    if (layer is FeatureLayer featureLayer) {
                        FeatureQueryResult featureQueryResult
                            = await featureLayer.FeatureTable.QueryFeaturesAsync(queryParameters);

                        if (featureQueryResult.Count() > 0) {
                            featureQueryResultList.Add(featureQueryResult);
                        }
                    }
                }
                QueryResult queryResult = new QueryResult(featureQueryResultList);
                queryResult.Activate();
                queryResult.Show();
            };

            // 绑定MapView左键抬起事件
            // 当鼠标左键抬起时，获取对应要素的属性值
            ManageMenuItemMouseLeftButtonUpEventHanler((MenuItem)sender, handler);
        }

        private void ManageMenuItemMouseLeftButtonUpEventHanler(
           MenuItem menuItem,
           MouseButtonEventHandler eventHandler
       ) {
            menuItem.IsChecked = !menuItem.IsChecked;
            if (menuItem.IsChecked == true) {
                MainMapView.MouseLeftButtonUp += eventHandler;
            } else {
                MainMapView.MouseLeftButtonUp -= eventHandler;
            }
        }

        private void PopulationDistribute(object sender, RoutedEventArgs e) {

        }

        private void GenderDistribute(object sender, RoutedEventArgs e) {

        }

        private void BufferAnalysis(object sender, RoutedEventArgs e) {
            BufferAnalysis bufferAnalysisWindow = new BufferAnalysis();
            bufferAnalysisWindow.UpdateSelectedDataInMapEvent += 
                    async (Esri.ArcGISRuntime.Geometry.Geometry geometry) => {
                        await MainMapView.SetViewpointGeometryAsync(geometry.Extent);
                    };
            bufferAnalysisWindow.Activate();
            bufferAnalysisWindow.Show();
        }

        // Map initialization logic is contained in MapViewModel.cs
    }
}
