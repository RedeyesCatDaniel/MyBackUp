using System;
namespace LGUVirtualOffice.Framework 
{
    public class TypeEventSystemUnSubscribe<T> : IUnSubscribe where T:IEvent
    {
        public ITypeEventSystem typeEventSystem;
        public Action<T> onEvent;
        public void UnSubscribe()
        {
            typeEventSystem.UnSubscribe(onEvent);
            typeEventSystem = null;
            onEvent = null;
        }
    }
}
