
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace GISExam_002.Enum {
    public class POIType : LayerType {
        public string FieldName { get; }

        public POIType(string Name, string POILayerName, string FieldName = "Name"): base(Name, POILayerName) {
            this.FieldName = FieldName;
        }

        public POIType(LayerType layerType, string FieldName = "Name") 
            : this(layerType.Name, layerType.LayerName){}

    }

    public class POITypeEnum {
        public readonly static POIType Dormitory_Point = new POIType(LayerEnum.Dormitory_Point);
        public readonly static POIType Playground_Point = new POIType(LayerEnum.Playground_Point);
        public readonly static POIType TeachingBuilding_Point = new POIType(LayerEnum.TeachingBuilding_Point);
        public readonly static POIType College_Point = new POIType(LayerEnum.College_Point);
        public readonly static POIType Bank_Point = new POIType(LayerEnum.Bank_Point);
        public readonly static POIType Office_Point = new POIType(LayerEnum.Office_Point);
        public readonly static POIType Supermarket_Point = new POIType(LayerEnum.Supermarket_Point);
        public readonly static POIType Canteen_Point = new POIType(LayerEnum.Canteen_Point);

        public readonly static POIType Administration = new POIType(LayerEnum.Administration);
        public readonly static POIType StudentCenter = new POIType(LayerEnum.StudentCenter);
        public readonly static POIType Library_1 = new POIType(LayerEnum.Library_1);
        public readonly static POIType Lake_1 = new POIType(LayerEnum.Lake_1);

        public readonly static POIType MainRoad = new POIType(LayerEnum.MainRoad, "注记");
        public readonly static POIType OutsideRoad = new POIType(LayerEnum.OutsideRoad, "注记");

        private static List<POIType> POITypesEnumList;
        public static List<POIType> GetKeys() {
            if (POITypesEnumList == null) {
                POITypesEnumList = CustomEnum.GetKeys<POITypeEnum, POIType>();
            }
            return POITypesEnumList;
        }

    }
}