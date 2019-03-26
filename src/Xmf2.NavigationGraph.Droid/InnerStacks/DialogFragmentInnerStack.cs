using System;
using Android.Support.V4.App;
using Xmf2.NavigationGraph.Droid.Interfaces;
using Xmf2.NavigationGraph.Droid.Operations;

namespace Xmf2.NavigationGraph.Droid.InnerStacks
{
	internal class DialogFragmentInnerStack : InnerStack, IFragmentInnerStack
	{
		public ActivityInnerStack FragmentHost { get; }

		public DialogFragment Fragment { get; }
		Fragment IFragmentInnerStack.Fragment => Fragment;

		public string FragmentTag { get; }

		public DialogFragmentInnerStack(NavigationStack navigationStack, ActivityInnerStack fragmentHost, DialogFragment fragment) : base(navigationStack)
		{
			FragmentHost = fragmentHost;
			Fragment = fragment;
			FragmentTag = $"{Fragment.GetType().Name}+{Guid.NewGuid():N}";
		}

		public override int Count => 1;

		public override PopOperation AsPopOperation()
		{
			return new FragmentPopOperation(FragmentHost)
			{
				FragmentStacksToPop =
				{
					this
				}
			};
		}

		public override PopOperation AsSpecificPopOperation(InnerStack child)
		{
			throw new InvalidOperationException("Operation not supported, fragment can not have children");
		}

		public override PopOperation AsSpecificPopOperation(int count)
		{
			throw new InvalidOperationException("Operation not supported, fragment can not have children");
		}
	}
}