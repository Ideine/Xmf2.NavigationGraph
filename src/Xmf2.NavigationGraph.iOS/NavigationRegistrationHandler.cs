using Xmf2.NavigationGraph.Core;
using Xmf2.NavigationGraph.iOS.Interfaces;

namespace Xmf2.NavigationGraph.iOS
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