using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GISExam_002.Enum {
    class CustomEnum {
        public static List<T> GetKeys<EnumType, T>() {
            Type type = typeof(T);
            return typeof(EnumType).GetFields(BindingFlags.Public | BindingFlags.Static)
                        .Where(f => f.FieldType == type)
                        .ToList().ConvertAll(f => (T)f.GetValue(null));
        }
    }
}
