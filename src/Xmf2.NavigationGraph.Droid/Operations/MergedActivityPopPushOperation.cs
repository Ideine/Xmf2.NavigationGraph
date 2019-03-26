using Android.App;
using Xmf2.NavigationGraph.Core.Interfaces;

namespace Xmf2.NavigationGraph.Droid.Operations
{
	internal class MergedActivityPopPushOperation<TViewModel> : MergedPopPushOperation where TViewModel : IViewModel
	{
		public ActivityPopOperation<TViewModel> PopActivity { get; }

		public ActivityPushOperation<TViewModel> PushActivity { get; }

		public MergedActivityPopPushOperation(ActivityPopOperation<TViewModel> popActivity, ActivityPushOperation<TViewModel> pushActivity)
		{
			PopActivity = popActivity;
			PushActivity = pushActivity;
		}

		public override void Execute(Activity activity)
		{
			PushActivity.Execute(activity);
			PopActivity.Execute(activity);
		}
	}
}