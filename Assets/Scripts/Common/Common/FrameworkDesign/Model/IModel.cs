namespace LGUVirtualOffice.Framework 
{
    public interface IModel : ICanSetArchitecture, ICanGetUtility,ICanTriggerEvent
    {
        void Init();
    }
}
