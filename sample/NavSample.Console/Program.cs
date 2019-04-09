using NavSample.Core;

namespace NavSample.ConsoleApp
{
	class Program
	{
		static void Main(string[] args)
		{
			DummyPresenterService dummyPresenter = new DummyPresenterService();
			SampleNavigationService navigation = new SampleNavigationService(dummyPresenter);

			/*
			navigation.Show(home);
			
			navigation.Show(cgu);
			navigation.Show(menu);
			navigation.Show(updatePwd);
			navigation.Show(profile);
			navigation.Show(login);
			navigation.Show(cgu);
			// */
			//*
			navigation.ShowOfferDetail("offer1");
			navigation.ShowProduct("product1");
			navigation.ShowOfferDetail("offer1");
			navigation.ShowProduct("product1");
			navigation.ShowOfferDetail("offer2");
			navigation.ShowOfferDetail("offer3");
			// */
			/*
			navigation.Show("/home");
			navigation.Show("/home/menu");
			navigation.Show("/home/offers/azerty/productId:qsdfgh");
			navigation.Show("/home/offers/azerty/offerId:qsdfgh");
			// */
		}
	}
}