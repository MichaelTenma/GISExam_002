using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GISExam_002
{
    class DataLoaderUtil
    {
        private static readonly Dictionary<string, Func<string[], Task<List<Layer>>>> openFuncList
           = new Dictionary<string, Func<string[], Task<List<Layer>>>>() {
                { ".shp" ,
                    async delegate(string[] filenameList) {
                        List<Layer> layers = new List<Layer>();
                        foreach (string filename in filenameList) {
                            FeatureTable featureTable = await ShapefileFeatureTable.OpenAsync(filename);
                            Layer layer = new FeatureLayer(featureTable);
                            layers.Add(layer);
                        }
                        return layers;
                    }
                },
                {".geodatabase",
                    async delegate(string[] filenameList) {
                        List<Layer> layers = new List<Layer>();
                        foreach(string filename in filenameList){
                            Geodatabase geodatabase = await Geodatabase.OpenAsync(filename);
                            IReadOnlyList<GeodatabaseFeatureTable> geodatabaseFeatureTables
                                = geodatabase.GeodatabaseFeatureTables;
                            for(int i = geodatabaseFeatureTables.Count-1; i >= 0 ;i--){
                                Layer layer = new FeatureLayer(geodatabaseFeatureTables[i]);
                                layers.Add(layer);
                            }
                        }
                        return layers;
                    }
                },
                {".img|.tif|.tiff",
                    async delegate(string[] filenameList) {
                        List<Layer> layers = new List<Layer>();
                        foreach (string filename in filenameList) {
                            Layer layer = new RasterLayer(filename);
                            layers.Add(layer);
                        }
                        return layers;
                    }
                }
           };

        public static Func<string[], Task<List<Layer>>> GetFunc(string suffix) {
            Func<string[], Task<List<Layer>>> resFunc = null;
            foreach (string key in openFuncList.Keys) {
                // 根据后缀选择合适的方法打开数据
                if (key.Contains(suffix)) {
                    resFunc = openFuncList[key];
                    break;
                }
            }
            return resFunc;
        }

        public static Func<string[], Task<List<Layer>>> GetFuncByFilename(string filename) {
            return GetFunc(GetFileSuffix(filename));
        }

        public static string GetFileSuffix(string filename) {
            return filename.Substring(filename.LastIndexOf("."));
        }


    }
}
