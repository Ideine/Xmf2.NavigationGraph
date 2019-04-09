using System;
using System.Diagnostics;
using NavSample.Core;
using UIKit;
using Xmf2.NavigationGraph.iOS.Interfaces;

namespace NavSample.iOS
{
	public static class Bootstrap
	{
		public static SampleNavigationService Navigation { get; private set; }
		private static SampleNavigationPresenter _presenter;

		public static async void Initialize(UIWindow window)
		{
			var navigationController = new UINavigationController();
			window.RootViewController = navigationController;

			_presenter = new SampleNavigationPresenter(navigationController);
			Navigation = new SampleNavigationService(_presenter);
			AssociateScreenToViewController(_presenter);
			await Navigation.ShowHome();
		}

		private static void Time(Action action, string label)
		{
			Stopwatch watcher = Stopwatch.StartNew();
			action();
			watcher.Stop();
			Console.WriteLine($"TIME: {label} took {watcher.ElapsedMilliseconds}ms");
		}

		private static void AssociateScreenToViewController(IRegistrationPresenterService<SampleViewModel> registrationService)
		{
			registrationService.Associate(Navigation.HomeSync, vm => new HomeController());
			registrationService.AssociateModal(Navigation.Menu, vm => new MenuController());
			registrationService.Associate(Navigation.Profile, vm => new ProfileController());
			registrationService.Associate(Navigation.UpdatePwd, vm => new UpdatePasswordController());
			registrationService.AssociateModal(Navigation.Cgu, vm => new CGUController());
			registrationService.Associate(Navigation.Login, vm => new LoginController());
			registrationService.Associate(Navigation.ListOffer, vm => new OfferListController());
			registrationService.Associate(Navigation.DetailOffer, vm => new OfferDetailController());
			registrationService.Associate(Navigation.DetailOffer, vm => new ProductController());
		}
	}
}