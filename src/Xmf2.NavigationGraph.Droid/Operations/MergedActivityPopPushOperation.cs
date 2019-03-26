using Android.App;

namespace Xmf2.NavigationGraph.Droid.Operations
{
	internal class MergedActivityPopPushOperation : MergedPopPushOperation
	{
		public ActivityPopOperation PopActivity { get; }

		public ActivityPushOperation PushActivity { get; }

		public MergedActivityPopPushOperation(ActivityPopOperation popActivity, ActivityPushOperation pushActivity)
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