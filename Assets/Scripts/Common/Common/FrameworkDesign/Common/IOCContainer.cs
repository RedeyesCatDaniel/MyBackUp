using System;
using System.Collections.Generic;
namespace LGUVirtualOffice.Framework 
{
    public class IOCContainer 
    {
        private Dictionary<Type, object> container = new Dictionary<Type, object>();

        public void Register<T>(T instance) 
        {
            Type key = typeof(T);
            if (container.ContainsKey(key))
            {
                container[key] = instance;
            }
            else 
            {
                container.Add(key, instance);
            }
        }
        public T Get<T>() where T : class 
        {
            Type key = typeof(T);
            if (container.TryGetValue(key, out object value)) 
            {
                return value as T;
            }
            return null;
        }
    }
}
