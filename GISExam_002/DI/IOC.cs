using Esri.ArcGISRuntime.UI.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace GISExam_002 {
    public class IOC{
        /**
         * 管理所有的bean对象
         */
        private static readonly IDictionary<Type, object> BEANS = new Dictionary<Type, object>(32);

        private static object Get(Type type) {
            object resObj = null;
            if (BEANS.TryGetValue(type, out object obj)) {
                resObj = obj;
            } else {
                /* 注入这个对象 */
                /* 获取注入这个对象的方法 */
                MethodInfo[] methodInfos = typeof(BeansFactory).GetMethods();
                IEnumerable<MethodInfo> targetMethodInfoList = methodInfos.Where((MethodInfo e) => {
                    return e.ReturnType.Equals(type);
                });
                if (targetMethodInfoList.Count() != 1)
                    throw new Exception($"不存在或是存在多个构建{type.FullName}类型的方法。");

                MethodInfo method = targetMethodInfoList.First();

                ParameterInfo[] parameterInfos = method.GetParameters();
                object[] paramArray = new object[parameterInfos.Length];
                for (int i = 0; i < parameterInfos.Length; i++) {
                    ParameterInfo param = parameterInfos[i];
                    paramArray[i] = Get(param.ParameterType);
                }
                resObj = method.Invoke(null, paramArray);
                BEANS.Add(type, resObj);
            }
            return resObj;
        }

        public static T Get<T>() {
            return (T)Get(typeof(T));
        }

        public static void Add<T>(T t) {
            Type type = typeof(T);
            if (!BEANS.TryGetValue(type, out object obj)) {
                /* 注入这个对象 */
                BEANS.Add(type, t);
            }
        }
    }
}
