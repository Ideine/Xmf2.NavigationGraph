using Android.App;
using Xmf2.NavigationGraph.Droid.InnerStacks;

namespace Xmf2.NavigationGraph.Droid.Operations
{
	internal class ActivityPopOperation : PopOperation
	{
		public ActivityInnerStack ActivityStack { get; }

		public ActivityPopOperation(ActivityInnerStack activityStack)
		{
			ActivityStack = activityStack;
		}

		public override void Execute(Activity activity)
		{
			//TODO: we could have some issue here if we need to close multiple activities at once
			NavigationPresenter.CurrentActivity.Finish();
		}
	}
}