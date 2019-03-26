using System.Collections.Generic;
using Android.App;
using Android.Content;
using Xmf2.NavigationGraph.Core.Interfaces;
using Xmf2.NavigationGraph.Droid.InnerStacks;
using Xmf2.NavigationGraph.Droid.Interfaces;

namespace Xmf2.NavigationGraph.Droid.Operations
{
	internal class ActivityPushOperation : PushOperation
	{
		public ActivityInnerStack ActivityStack { get; }

		public IViewModel ViewModel { get; }

		public List<IFragmentInnerStack> FragmentStacksToPush { get; } = new List<IFragmentInnerStack>();

		public ActivityPushOperation(ActivityInnerStack activityStack, IViewModel viewModel)
		{
			ActivityStack = activityStack;
			ViewModel = viewModel;
		}

		public override void Execute(Activity activity)
		{
			Intent intent = new Intent(activity, ActivityStack.ActivityType);
			if (ActivityStack.ShouldClearHistory)
			{
				intent.SetFlags(ActivityFlags.ClearTask | ActivityFlags.NewTask);
			}

			if (ViewModel != null)
			{
				intent.PutExtra(NavigationPresenter.VIEWMODEL_LINK_PARAMETER_CODE, NavigationParameterContainer.CreateNavigationParameter(ViewModel));
			}

			if (FragmentStacksToPush.Count > 0)
			{
				FragmentPushOperation operation = new FragmentPushOperation(ActivityStack);
				operation.FragmentStacksToPush.AddRange(FragmentStacksToPush);
				intent.PutExtra(NavigationPresenter.FRAGMENT_START_PARAMETER_CODE, NavigationParameterContainer.CreateNavigationParameter(new DeferredNavigationAction(operation)));
			}

			activity.StartActivity(intent);
		}
	}
}