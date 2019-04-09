using System.Threading.Tasks;
using Xmf2.NavigationGraph.Core;
using Xmf2.NavigationGraph.Core.Interfaces;

namespace NavSample.Core
{
	public class SampleNavigationService : NavigationService<SampleViewModel>
	{
		private IPresenterService<SampleViewModel> _presenter;

		public readonly ScreenDefinition<SampleViewModel> HomeSync;
		public readonly ScreenDefinition<SampleViewModel> Menu;
		public readonly ScreenDefinition<SampleViewModel> Profile;
		public readonly ScreenDefinition<SampleViewModel> UpdatePwd;
		public readonly ScreenDefinition<SampleViewModel> Cgu;
		public readonly ScreenDefinition<SampleViewModel> Login;
		public readonly ScreenDefinition<SampleViewModel> ListOffer;
		public readonly ScreenDefinition<SampleViewModel> DetailOffer;
		public readonly ScreenDefinition<SampleViewModel> DetailProduct;

		public SampleNavigationService(IPresenterService<SampleViewModel> presenterService) : base(presenterService)
		{
			_presenter = presenterService;

			HomeSync = new ScreenDefinition<SampleViewModel>("home", _ => new SampleViewModel("home").AsTask());
			Menu = new ScreenDefinition<SampleViewModel>("menu", _ => new SampleViewModel("menu").AsTask());
			Profile = new ScreenDefinition<SampleViewModel>("profile", _ => new SampleViewModel("profile").AsTask());
			UpdatePwd = new ScreenDefinition<SampleViewModel>("updatePwd", _ => new SampleViewModel("updatePwd").AsTask());
			Cgu = new ScreenDefinition<SampleViewModel>("cgu", _ => new SampleViewModel("cgu").AsTask());
			Login = new ScreenDefinition<SampleViewModel>("login", _ => new SampleViewModel("login").AsTask());

			ListOffer = new ScreenDefinition<SampleViewModel>("offers", _ => new SampleViewModel("offers").AsTask());
			DetailOffer = new ScreenDefinition<SampleViewModel>("{offerId}", _ => new SampleViewModel("{offerId}").AsTask());
			DetailProduct = new ScreenDefinition<SampleViewModel>("{productId}", _ => new SampleViewModel("{productId}").AsTask());


			//Registre scree associations.
			this.RegisterEntryPoint(HomeSync);
			this.RegisterEntryPoint(Login);

			this.Register(HomeSync, Profile);
			this.Register(Profile, UpdatePwd);
			this.Register(Profile, Cgu);

			this.Register(Login, Cgu);

			this.Register(HomeSync, ListOffer);
			this.Register(ListOffer, DetailOffer);
			this.Register(DetailOffer, DetailProduct);
			this.Register(DetailOffer, DetailOffer);

			this.Register(HomeSync, Menu);
			this.Register(Profile, Menu);
			this.Register(UpdatePwd, Menu);
			this.Register(Cgu, Menu);
			this.Register(ListOffer, Menu);
			this.Register(DetailOffer, Menu);
			this.Register(DetailProduct, Menu);
		}

		public Task ShowHome() => this.Show(HomeSync);
		public Task ShowMenu() => this.Show(Menu);
		public Task ShowProfile() => this.Show(Profile);
		public Task ShowUpdatePassword() => this.Show(UpdatePwd);
		public Task ShowCGU() => this.Show(Cgu);
		public Task ShowLogin() => this.Show(Login);
		public Task ShowOfferList() => this.Show(ListOffer);
		public Task ShowOfferDetail(string id) => this.Show(DetailOffer, id);
		public Task ShowProduct(string id) => this.Show(DetailProduct, id);
	}
}