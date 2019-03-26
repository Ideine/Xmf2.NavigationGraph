using System.Collections.Generic;
using Android.App;
using Android.Support.V7.App;
using Xmf2.NavigationGraph.Droid.InnerStacks;
using Xmf2.NavigationGraph.Droid.Interfaces;

namespace Xmf2.NavigationGraph.Droid.Operations
{
	internal class MergedFragmentPopPushOperation : MergedPopPushOperation
	{
		public ActivityInnerStack HostStack { get; }

		public List<IFragmentInnerStack> FragmentStacksToPop { get; }
		public List<IFragmentInnerStack> FragmentStacksToPush { get; }

		public MergedFragmentPopPushOperation(ActivityInnerStack hostStack, List<IFragmentInnerStack> fragmentStacksToPop, List<IFragmentInnerStack> fragmentStacksToPush)
		{
			HostStack = hostStack;
			FragmentStacksToPop = fragmentStacksToPop;
			FragmentStacksToPush = fragmentStacksToPush;
		}

		public override void Execute(Activity activity)
		{
			if (activity is AppCompatActivity appCompatActivity)
			{
				NavigationStack.UpdateFragments(HostStack.NavigationStack, appCompatActivity, FragmentStacksToPop, FragmentStacksToPush, activity as IFragmentActivity);
			}
		}
	}
}