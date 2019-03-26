using System;
using Android.Support.V4.App;

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