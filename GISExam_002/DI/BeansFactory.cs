using Esri.ArcGISRuntime.UI.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GISExam_002
{
    class BeansFactory
    {
        /**
         * 如何解决循环依赖问题
         */
        public static LayerService LayerService() {
            return new LayerService();
        }

        public static MapViewModel MapViewModel(LayerService layerService) {
            return new MapViewModel(layerService);
        }

        public static BufferService BufferService(MapView mapView) {
            return new BufferService(mapView);
        }
    }
}
