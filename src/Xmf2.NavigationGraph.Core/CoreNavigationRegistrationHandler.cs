using System;
using System.Threading.Tasks;

namespace Xmf2.NavigationGraph.Core
{
	public abstract class CoreNavigationRegistrationHandler : BaseServiceContainer
	{
		private readonly string _defaultPath;
		private readonly Lazy<IViewModelFactory> _factory;
		private readonly Lazy<INavigationService> _navigationService;
		private IViewModelFactory Factory => _factory.Value;
		protected INavigationService NavigationService => _navigationService.Value;

		protected CoreNavigationRegistrationHandler(IServiceLocator services, string defaultPath) : base(services)
		{
			_factory = LazyResolver<IViewModelFactory>();
			_navigationService = LazyResolver<INavigationService>();
			_defaultPath = defaultPath;
		}

		protected IComponentViewModel CreateViewModel<TViewModelType>(string id)
		{
			IServiceLocator locator = Services.Scope(id);
			locator.RegisterSingleton<IEventBus, EventBus>();
			return Factory.Create(locator, new Location<TViewModelType>(_defaultPath, id));
		}

		public void Register()
		{
			Register(NavigationService);
		}

		protected virtual void Register(INavigationService navigationService) { }

		protected ScreenDefinition CreateScreen<TViewModel>(string name) where TViewModel : IComponentViewModel
			=> new ScreenDefinition(name, _ => CreateViewModel<TViewModel>(name).WaitInitialize());

		protected ScreenDefinition CreateScreenWithoutInitialization<TViewModel>(string name) where TViewModel : IComponentViewModel
			=> new ScreenDefinition(name, _ => Task.FromResult(CreateViewModel<TViewModel>(name)));

		protected ScreenDefinition CreateScreen<TViewModel>(string name, string parameterName) where TViewModel : IComponentViewModel
			=> new ScreenDefinition(name, _ => CreateViewModel<TViewModel>($"{{{parameterName}}}").WaitInitialize());

		protected ScreenDefinition CreateScreenWithoutInitialization<TViewModel>(string name, string parameterName) where TViewModel : IComponentViewModel
			=> new ScreenDefinition(name, _ => Task.FromResult(CreateViewModel<TViewModel>($"{{{parameterName}}}")));
	}
}