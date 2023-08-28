using Android.App;
using Xmf2.NavigationGraph.Core.Interfaces;
using Xmf2.NavigationGraph.Droid.InnerStacks;
#if NET7_0_OR_GREATER
using Microsoft.Maui.ApplicationModel;

#else
using Plugin.CurrentActivity;
#endif

namespace Xmf2.NavigationGraph.Droid.Operations
{
	internal class ActivityPopOperation<TViewModel> : PopOperation where TViewModel : IViewModel
	{
		public ActivityInnerStack<TViewModel> ActivityStack { get; }

		public ActivityPopOperation(ActivityInnerStack<TViewModel> activityStack)
		{
			ActivityStack = activityStack;
		}

		public override void Execute(Activity activity)
		{
			//TODO: we could have some issue here if we need to close multiple activities at once
#if NET7_0_OR_GREATER
			Platform.CurrentActivity!.Finish();
#else
			CrossCurrentActivity.Current.Activity!.Finish();
#endif
		}
	}
}