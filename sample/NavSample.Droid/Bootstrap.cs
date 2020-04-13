using NavSample.Core;
using Xmf2.NavigationGraph.Core;
using Xmf2.NavigationGraph.Core.Interfaces;
using Xmf2.NavigationGraph.Droid.Interfaces;

namespace NavSample.Droid
{
	public static class Bootstrap
	{
		public static SampleNavigationService Navigation { get; private set; }
		private static IViewModelLocatorService<SampleViewModel> _viewModelLocatorService;
		private static SampleNavigationPresenter _presenter;

		public static void Initialize()
		{
			_viewModelLocatorService =  ViewModelLocatorService<SampleViewModel>.Instance;
			_presenter = new SampleNavigationPresenter(_viewModelLocatorService);
			Navigation = new SampleNavigationService(_presenter);

			SetupAndroidNav(_presenter);

			Navigation.ShowHome();
		}

		private static void SetupAndroidNav(IRegistrationPresenterService<SampleViewModel> presenterService)
		{
			presenterService.RegisterDefaultFragmentHost<MainActivity>(shouldClearHistory: false);
			presenterService.AssociateActivity<LoginActivity>(Navigation.Login, shouldClearHistory: true);
			presenterService.AssociateFragment(Navigation.HomeSync, () => new HomeFragment());
			presenterService.AssociateDialogFragment(Navigation.Menu, () => new MenuFragment());
			presenterService.AssociateFragment(Navigation.Profile, () => new ProfileFragment());
			presenterService.AssociateFragment(Navigation.UpdatePwd, () => new UpdatePasswordFragment());
			presenterService.AssociateDialogFragment(Navigation.Cgu, () => new CGUFragment());
			presenterService.AssociateFragment(Navigation.ListOffer, () => new OfferListFragment());
			presenterService.AssociateFragment(Navigation.DetailOffer, () => new OfferDetailFragment());
			presenterService.AssociateFragment(Navigation.DetailProduct, () => new ProductFragment());
		}
	}
}