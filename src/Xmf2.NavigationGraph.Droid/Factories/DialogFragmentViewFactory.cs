using System;
#if __ANDROID_29__
using AndroidX.Fragment.App;
#else
using Android.Support.V4.App;
#endif

namespace Xmf2.NavigationGraph.Droid.Factories
{
	internal class DialogFragmentViewFactory : ViewFactory
	{
		public Func<DialogFragment> Creator { get; }

		public Type HostActivityType { get; }

		public bool ShouldClearHistory { get; }

		public DialogFragmentViewFactory(Func<DialogFragment> creator, Type hostActivityType, bool shouldClearHistory)
		{
			Creator = creator;
			HostActivityType = hostActivityType;
			ShouldClearHistory = shouldClearHistory;
		}
	}
}