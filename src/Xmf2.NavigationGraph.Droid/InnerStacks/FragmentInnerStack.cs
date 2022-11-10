using System;
using AndroidX.Fragment.App;
using Xmf2.NavigationGraph.Core.Interfaces;
using Xmf2.NavigationGraph.Droid.Interfaces;
using Xmf2.NavigationGraph.Droid.Operations;

namespace Xmf2.NavigationGraph.Droid.InnerStacks
{
	internal class FragmentInnerStack<TViewModel> : InnerStack<TViewModel>, IFragmentInnerStack where TViewModel : IViewModel
	{
		public ActivityInnerStack<TViewModel> FragmentHost { get; }

		public Fragment Fragment { get; internal set; }

		public string FragmentTag { get; }

		public FragmentInnerStack(NavigationStack<TViewModel> navigationStack, ActivityInnerStack<TViewModel> fragmentHost, Fragment fragment) : base(navigationStack)
		{
			FragmentHost = fragmentHost;
			Fragment = fragment;
			FragmentTag = $"{Fragment.GetType().Name}+{Guid.NewGuid():N}";
		}

		public override int Count => 1;

		public override PopOperation AsPopOperation() => FragmentHost.AsSpecificPopOperation(this);

		public override PopOperation AsSpecificPopOperation(InnerStack<TViewModel> child)
		{
			throw new InvalidOperationException("Operation not supported, fragment can not have children");
		}

		public override PopOperation AsSpecificPopOperation(int count)
		{
			throw new InvalidOperationException("Operation not supported, fragment can not have children");
		}
	}
}