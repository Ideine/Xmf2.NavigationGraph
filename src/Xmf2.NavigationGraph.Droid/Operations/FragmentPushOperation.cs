using System.Collections.Generic;
using Android.App;
using Android.Support.V7.App;
using Xmf2.NavigationGraph.Core.Interfaces;
using Xmf2.NavigationGraph.Droid.InnerStacks;
using Xmf2.NavigationGraph.Droid.Interfaces;

namespace Xmf2.NavigationGraph.Droid.Operations
{
	internal class FragmentPushOperation<TViewModel> : PushOperation where TViewModel : IViewModel
	{
		public ActivityInnerStack<TViewModel> HostStack { get; }

		public List<IFragmentInnerStack> FragmentStacksToPush { get; } = new List<IFragmentInnerStack>();

		public FragmentPushOperation(ActivityInnerStack<TViewModel> hostStack)
		{
			HostStack = hostStack;
		}

		public override void Execute(Activity activity)
		{
			if (activity is AppCompatActivity appCompatActivity)
			{
				NavigationStack<TViewModel>.UpdateFragments(HostStack.NavigationStack, appCompatActivity, null, FragmentStacksToPush, activity as IFragmentActivity);
			}
		}
	}
}