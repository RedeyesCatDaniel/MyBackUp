namespace LGUVirtualOffice.Framework 
{
    public interface IService:ICanSetArchitecture, ICanGetUtility, ICanGetModel,ICanTriggerEvent,ICanSubscribeEvent,ICanGetService
    {
        void Init();
    }
}
