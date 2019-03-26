using System.Collections.Generic;
using Android.App;
using Android.Support.V7.App;
using Xmf2.NavigationGraph.Droid.InnerStacks;
using Xmf2.NavigationGraph.Droid.Interfaces;

namespace Xmf2.NavigationGraph.Droid.Operations
{
	internal class FragmentPopOperation : PopOperation
	{
		public ActivityInnerStack HostStack { get; }

		public List<IFragmentInnerStack> FragmentStacksToPop { get; } = new List<IFragmentInnerStack>();

		public FragmentPopOperation(ActivityInnerStack hostStack)
		{
			HostStack = hostStack;
		}

		public override void Execute(Activity activity)
		{
			if (activity is AppCompatActivity appCompatActivity)
			{
				NavigationStack.UpdateFragments(HostStack.NavigationStack, appCompatActivity, FragmentStacksToPop, null, activity as IFragmentActivity);
			}
		}
	}
}