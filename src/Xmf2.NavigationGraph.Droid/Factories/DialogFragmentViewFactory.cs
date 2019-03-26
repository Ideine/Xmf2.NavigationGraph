using System;
using Android.Support.V4.App;

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