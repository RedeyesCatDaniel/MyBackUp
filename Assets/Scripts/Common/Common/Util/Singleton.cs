using System;
using System.Reflection;
namespace LGUVirtualOffice { 
    public class Singleton<T> where T:Singleton<T>
    {
        private static T instance;

        public static T Instance
        {
            get {
                if (instance == null) {
                    Type classType = typeof(T);
                    ConstructorInfo[] consArr=classType.GetConstructors(BindingFlags.Instance|BindingFlags.NonPublic);
                    ConstructorInfo nonPublicConst=Array.Find<ConstructorInfo>(consArr,c=>c.GetParameters().Length==0);
                    if (nonPublicConst == null) {
                        throw new Exception("Non public Constructor Not Found In Type "+classType);
                    }
                    instance=nonPublicConst.Invoke(null) as T;
                }
                return instance;
            }
        }
    }

}
