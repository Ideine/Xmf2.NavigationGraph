using NavSample.Core;
using UIKit;
using Xmf2.NavigationGraph.iOS;

namespace NavSample.iOS
{
	public class SampleNavigationPresenter : NavigationPresenter<SampleViewModel>
	{
		public SampleNavigationPresenter(UIWindow window) : base(window)
		{
		}
	}
}