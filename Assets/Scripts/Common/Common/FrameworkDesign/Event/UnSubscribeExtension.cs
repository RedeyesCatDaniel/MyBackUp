using UnityEngine;
namespace LGUVirtualOffice.Framework 
{
    public static class UnSubscribeExtension
    {
        public static void UnSubScribeWhenGameObjectDestroyed(this IUnSubscribe self,GameObject gameObject) 
        {
            var trigger = gameObject.GetComponent<UnSubscribeOnDestroyTrigger>();
            if (!trigger) 
            {
               trigger= gameObject.AddComponent<UnSubscribeOnDestroyTrigger>();
            }
            trigger.AddUnSubscribes(self);
        }
    }
}
