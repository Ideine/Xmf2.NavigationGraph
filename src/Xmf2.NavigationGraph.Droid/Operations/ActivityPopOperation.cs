using Android.App;
using Plugin.CurrentActivity;
using Xmf2.NavigationGraph.Core.Interfaces;
using Xmf2.NavigationGraph.Droid.InnerStacks;

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
			CrossCurrentActivity.Current.Activity.Finish();
		}
	}
}