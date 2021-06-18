using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GISExam_002.Enum {
    public class LayerType {
        public string Name { get; }
        public string LayerName { get; }

        public LayerType(string Name, string LayerName) {
            this.Name = Name;
            this.LayerName = LayerName;
        }

        public override string ToString() {
            return this.Name;
        }

        public override bool Equals(object obj) {
            bool isEquals = false;
            if (obj is LayerType layerType) {
                isEquals = this.Name.Equals(layerType.Name) && this.LayerName.Equals(layerType.LayerName);
            }
            return isEquals;
        }

        public override int GetHashCode() {
            return LayerName.GetHashCode() ^ Name.GetHashCode();
        }
    }

    public class LayerEnum {
        public readonly static LayerType Dormitory_Point = new LayerType("宿舍", "Dormitory_Point");
        public readonly static LayerType Playground_Point = new LayerType("运动场", "Playground_Point");
        public readonly static LayerType TeachingBuilding_Point = new LayerType("教学楼", "TeachingBuilding_Point");
        public readonly static LayerType College_Point = new LayerType("学院楼", "College_Point");
        public readonly static LayerType Bank_Point = new LayerType("银行", "Bank_Point");
        public readonly static LayerType Office_Point = new LayerType("办公楼", "Office_Point");
        public readonly static LayerType Supermarket_Point = new LayerType("超市", "Supermarket_Point");
        public readonly static LayerType Canteen_Point = new LayerType("饭堂", "Canteen_Point");

        public readonly static LayerType Border_Line = new LayerType("边界线", "Border_Line");
        public readonly static LayerType MainRoad = new LayerType("主要道路", "MainRoad");
        public readonly static LayerType OutsideRoad = new LayerType("外部道路", "OutsideRoad");
        public readonly static LayerType StudentDormitory = new LayerType("学生宿舍", "StudentDormitory");

        public readonly static LayerType Administration = new LayerType("行政单位", "Administration");
        public readonly static LayerType StudentCenter = new LayerType("学生中心", "StudentCenter");
        public readonly static LayerType Playground_1 = new LayerType("运动场1", "Playground_1");
        public readonly static LayerType TeachersHome = new LayerType("教工住所", "TeachersHome");
        public readonly static LayerType Library_1 = new LayerType("图书馆", "Library_1");
        public readonly static LayerType Green = new LayerType("绿地", "Green");
        public readonly static LayerType College_1 = new LayerType("院系", "College_1");
        public readonly static LayerType Lake_1 = new LayerType("湖", "Lake_1");

        public readonly static LayerType Canteen = new LayerType("饭堂1", "Canteen");
        public readonly static LayerType TeachingBuilding = new LayerType("教学楼面", "TeachingBuilding");
        public readonly static LayerType Border_Polygon = new LayerType("边界面", "Border_Polygon");

        private static List<LayerType> EnumList;
        public static List<LayerType> GetKeys() {
            if (EnumList == null) {
                EnumList = CustomEnum.GetKeys<LayerEnum, LayerType>();
            }
            return EnumList;
        }

        public static LayerType GetLayerTypeByLayerName(string layerName) {
            LayerType res = null;
            foreach (LayerType layerType in GetKeys()) {
                if (layerType.LayerName.Equals(layerName)) {
                    res = layerType;
                    break;
                }
            }
            return res;
        }
    }
}
