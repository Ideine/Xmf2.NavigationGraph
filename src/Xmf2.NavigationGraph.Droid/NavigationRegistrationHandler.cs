using Xmf2.NavigationGraph.Droid.Interfaces;

namespace Xmf2.NavigationGraph.Droid
{
	public abstract class NavigationRegistrationHandler<TCoreHandler> : BaseServiceContainer
		where TCoreHandler : CoreNavigationRegistrationHandler
	{
		public TCoreHandler CoreHandler { get; }

		protected NavigationRegistrationHandler(IServiceLocator services, TCoreHandler coreHandler) : base(services)
		{
			CoreHandler = coreHandler;
		}

		public void Register()
		{
			IRegistrationPresenterService registrationPresenterService = Services.Resolve<IRegistrationPresenterService>();

			CoreHandler.Register();
			Register(registrationPresenterService);
		}

		protected virtual void Register(IRegistrationPresenterService presenterService) { }
	}
}