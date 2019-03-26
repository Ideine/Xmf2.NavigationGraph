using System;
using Android.Support.V4.App;
using Xmf2.NavigationGraph.Droid.Interfaces;
using Xmf2.NavigationGraph.Droid.Operations;

namespace Xmf2.NavigationGraph.Droid.InnerStacks
{
	internal class fragmentInnerStack : InnerStack, IFragmentInnerStack
	{
		public ActivityInnerStack FragmentHost { get; }

		public Fragment Fragment { get; }

		public string FragmentTag { get; }

		public fragmentInnerStack(NavigationStack navigationStack, ActivityInnerStack fragmentHost, Fragment fragment) : base(navigationStack)
		{
			FragmentHost = fragmentHost;
			Fragment = fragment;
			FragmentTag = $"{Fragment.GetType().Name}+{Guid.NewGuid():N}";
		}

		public override int Count => 1;

		public override PopOperation AsPopOperation() => FragmentHost.AsSpecificPopOperation(this);

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