using System;
#if __ANDROID_29__
using AndroidX.Fragment.App;
#else
using Android.Support.V4.App;
#endif

namespace Xmf2.NavigationGraph.Droid.Factories
{
	internal class FragmentViewFactory : ViewFactory
	{
		public Func<Fragment> Creator { get; }

		public Type HostActivityType { get; }

		public bool ShouldClearHistory { get; }

		public FragmentViewFactory(Func<Fragment> creator, Type hostActivityType, bool shouldClearHistory)
		{
			Creator = creator;
			HostActivityType = hostActivityType;
			ShouldClearHistory = shouldClearHistory;
		}
	}
}