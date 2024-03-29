using System.Collections.Generic;
using Android.App;
using AndroidX.AppCompat.App;
using Xmf2.NavigationGraph.Core.Interfaces;
using Xmf2.NavigationGraph.Droid.InnerStacks;
using Xmf2.NavigationGraph.Droid.Interfaces;

namespace Xmf2.NavigationGraph.Droid.Operations
{
	internal class FragmentPopOperation<TViewModel> : PopOperation where TViewModel : IViewModel
	{
		public ActivityInnerStack<TViewModel> HostStack { get; }

		public List<IFragmentInnerStack> FragmentStacksToPop { get; } = new();

		public FragmentPopOperation(ActivityInnerStack<TViewModel> hostStack)
		{
			HostStack = hostStack;
		}

		public override void Execute(Activity activity)
		{
			if (activity is AppCompatActivity appCompatActivity)
			{
				NavigationStack<TViewModel>.UpdateFragments(HostStack.NavigationStack, appCompatActivity, FragmentStacksToPop, null, activity as IFragmentActivity);
			}
		}
	}
}