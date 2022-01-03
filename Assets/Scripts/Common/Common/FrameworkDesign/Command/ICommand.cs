namespace LGUVirtualOffice.Framework 
{
    public interface ICommand:ICanSetArchitecture, ICanGetModel,ICanGetService,ICanGetUtility,ICanTriggerEvent,ICanSendCommand
    {
        public void Excute();
    }
}
