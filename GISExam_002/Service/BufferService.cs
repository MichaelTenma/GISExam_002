using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Symbology;
using Esri.ArcGISRuntime.UI;
using Esri.ArcGISRuntime.UI.Controls;
using GISExam_002.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GISExam_002
{
    class BufferService
    {
        private readonly GraphicsOverlay BufferGraphicsOverlay;
        private readonly SimpleFillSymbol BufferFillSymbol;

        public BufferService(MapView mapView) {
            this.BufferGraphicsOverlay = new GraphicsOverlay();
            this.BindGraphicsOverlay(mapView);

            this.BufferFillSymbol = new SimpleFillSymbol(
                SimpleFillSymbolStyle.Solid, Color.FromArgb(125, 255, 250, 0),
                new SimpleLineSymbol(SimpleLineSymbolStyle.Solid, Color.FromArgb(0, 0, 0), 4.0)
            );
        }

        private void BindGraphicsOverlay(Esri.ArcGISRuntime.UI.Controls.MapView mapView) {
            int index = mapView.GraphicsOverlays.IndexOf(this.BufferGraphicsOverlay);
            if (index < 0) {
                mapView.GraphicsOverlays.Add(this.BufferGraphicsOverlay);
            }
        }

        public void AddGeometry(Geometry geometry) {
            //利用缓冲结果构建新的图形
            Graphic bufferGraphic = new Graphic(geometry, BufferFillSymbol);
            this.BufferGraphicsOverlay.Graphics.Add(bufferGraphic);
        }

        public void ClearBuffer() {
            this.BufferGraphicsOverlay.Graphics.Clear();
        }
    }
}
