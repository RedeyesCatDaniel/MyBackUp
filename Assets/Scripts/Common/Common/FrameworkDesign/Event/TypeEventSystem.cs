using System;
using System.Collections.Generic;
namespace LGUVirtualOffice.Framework 
{
    public class TypeEventSystem : ITypeEventSystem
    {
        interface ISubscrition { void Trigger(IEvent e); }

        class Subscription<T> : ISubscrition where T : IEvent
        {
            public Action<T> mOnEvent;
            public void Trigger(IEvent e)
            {
                mOnEvent?.Invoke((T)e);
            }
        }

        Dictionary<Type, ISubscrition> eventPool = new Dictionary<Type, ISubscrition>();

        public IUnSubscribe Subscribe<T>(Action<T> mOnEvent) where T:IEvent
        {
            var type = typeof(T);
            ISubscrition subscrition;
            if (!eventPool.TryGetValue(type,out subscrition)) 
            {
                subscrition = new Subscription<T>();
                eventPool.Add(type, subscrition);
            }
            (subscrition as Subscription<T>).mOnEvent += mOnEvent;
            return new TypeEventSystemUnSubscribe<T>()
            {
                typeEventSystem=this,
                onEvent=mOnEvent
            };
        }


        public void UnSubscribe<T>(Action<T> mOnEvent) where T:IEvent
        {
            var type = typeof(T);
            if(eventPool.TryGetValue(type, out ISubscrition subscrition))
            {
                (subscrition as Subscription<T>).mOnEvent -= mOnEvent;
            }
        }
        public void Trigger<T>() where T : IEvent, new()
        {
            var e = new T();
            Trigger<T>(e);
        }

        public void Trigger<T>(T e) where T : IEvent
        {
            var type = e.GetType();
            ISubscrition subscrition;
            if (eventPool.TryGetValue(type, out subscrition))
            {
                subscrition.Trigger(e);
            }
        }
    }
}
