using System.Collections.Generic;
using Android.App;
using AndroidX.AppCompat.App;
using Xmf2.NavigationGraph.Core.Interfaces;
using Xmf2.NavigationGraph.Droid.InnerStacks;
using Xmf2.NavigationGraph.Droid.Interfaces;

namespace Xmf2.NavigationGraph.Droid.Operations
{
	internal class MergedFragmentPopPushOperation<TViewModel> : MergedPopPushOperation where TViewModel : IViewModel
	{
		public ActivityInnerStack<TViewModel> HostStack { get; }

		public List<IFragmentInnerStack> FragmentStacksToPop { get; }
		public List<IFragmentInnerStack> FragmentStacksToPush { get; }

		public MergedFragmentPopPushOperation(ActivityInnerStack<TViewModel> hostStack, List<IFragmentInnerStack> fragmentStacksToPop, List<IFragmentInnerStack> fragmentStacksToPush)
		{
			HostStack = hostStack;
			FragmentStacksToPop = fragmentStacksToPop;
			FragmentStacksToPush = fragmentStacksToPush;
		}

		public override void Execute(Activity activity)
		{
			if (activity is AppCompatActivity appCompatActivity)
			{
				NavigationStack<TViewModel>.UpdateFragments(HostStack.NavigationStack, appCompatActivity, FragmentStacksToPop, FragmentStacksToPush, activity as IFragmentActivity);
			}
		}
	}
}