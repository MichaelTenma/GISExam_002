using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.UI.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GISExam_002.Utils
{
    class EnvelopeUtil
    {
        private readonly static List<string> xyzList = new List<string>() { "X", "Y", "Z" };
        private readonly static List<string> minOrMaxList = new List<string>() { "Min", "Max" };

        private static double getMinOrMaxValue(Envelope extentA, Envelope extentB, string XYZ, string MinOrMax) {
            PropertyInfo propertyInfo = extentA.GetType().GetProperty(XYZ + MinOrMax);
            double A = (double)(propertyInfo.GetValue(extentA));
            double B = (double)(propertyInfo.GetValue(extentB));
            return (MinOrMax.Equals("Min"))
                ? (A < B ? A : B)
                : (A < B ? B : A)
            ;
        }

        private static double GetXYZMinMaxValue(Envelope extentA, string XYZ, string MinOrMax) {
            PropertyInfo propertyInfo = extentA.GetType().GetProperty(XYZ + MinOrMax);
            return (double)(propertyInfo.GetValue(extentA));
        }

        public static Dictionary<string, double> InitXYZMinOrMaxMap(Envelope maxExtent) {
            Dictionary<string, double> doubleListMap = new Dictionary<string, double>();
            xyzList.ForEach(xyz => {
                minOrMaxList.ForEach(minOrMax => {
                    doubleListMap.Add(xyz + minOrMax, GetXYZMinMaxValue(maxExtent, xyz, minOrMax));
                });
            });
            return doubleListMap;
        }

        public static Envelope CreateEnvelopeInstance(
            Dictionary<string, double> doubleListMap, SpatialReference spatialReference
        ) {
            Dictionary<string, MapPoint> mapPointMap = new Dictionary<string, MapPoint>();
            EnvelopeUtil.minOrMaxList.ForEach(minOrMax => {
                int len = EnvelopeUtil.xyzList.Count + 1;
                Type[] tps = new Type[len];
                tps[tps.Length - 1] = typeof(SpatialReference);
                for (int i = 0; i < tps.Length - 1; i++) tps[i] = typeof(double);

                object[] paramsObj = new object[len];
                paramsObj[paramsObj.Length - 1] = spatialReference;
                for (int xyzIndex = 0; xyzIndex < EnvelopeUtil.xyzList.Count; xyzIndex++) {
                    paramsObj[xyzIndex] = doubleListMap[
                        EnvelopeUtil.xyzList[xyzIndex] + minOrMax
                    ];
                }

                MapPoint mapPoint = (MapPoint)(typeof(MapPoint).GetConstructor(tps).Invoke(paramsObj));
                mapPointMap.Add(minOrMax, mapPoint);
            });

            return new Envelope(mapPointMap[minOrMaxList[0]], mapPointMap[minOrMaxList[1]]);
        }

        public static Envelope CalMaxExtent(IEnumerator<Envelope> extentList) {
            // wgs84
            SpatialReference spatialReference = SpatialReference.Create(4326);
            EnvelopeBuilder envelopeBuilder = new EnvelopeBuilder(spatialReference);

            while (extentList.MoveNext()) {
                Envelope envelope = extentList.Current;
                envelope = GeometryEngine.Project(envelope, spatialReference).Extent;
                envelopeBuilder.UnionOf(envelope);
            }

            return envelopeBuilder.ToGeometry();
        }

        public static IEnumerator<Envelope> CalEnvelopeBy(IEnumerable<Feature> featureList) {
            foreach (var feature in featureList) {
                yield return feature?.Geometry?.Extent;
            }
        }

        public static Envelope CalMaxExtent(IEnumerable<Feature> featureList) {
            return CalMaxExtent(CalEnvelopeBy(featureList));
        }

        public static MapPoint SrceenPointToMapPoint(MapView mapView, System.Windows.Point clickPoint) {
            //得到基于左上角为原点的点坐标
            double x = clickPoint.X * mapView.UnitsPerPixel;
            double y = clickPoint.Y * mapView.UnitsPerPixel;

            /* 计算点击点在当前坐标系下对应的点坐标
             * 若空间坐标系为地理坐标系是为经纬度
             * 若为投影坐标系则为坐标
             */
            x += mapView.VisibleArea.Extent.XMin;//横轴偏移
            y = mapView.VisibleArea.Extent.YMax - y;//竖轴偏移
            return new MapPoint(x, y, mapView.SpatialReference);
        }
    }
}
