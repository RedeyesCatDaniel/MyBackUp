namespace LGUVirtualOffice.Framework 
{
    public abstract class AbstractService : IService
    {
        private IArchitecture architecture;
        IArchitecture ICanGetArchitecture.GetArchitecture()
        {
            return architecture;
        }

        void IService.Init()
        {
            OnInit();
        }
        protected abstract void OnInit();
        void ICanSetArchitecture.SetArchitecture(IArchitecture architecture)
        {
            this.architecture = architecture;
        }
    }

    
}
