using System;
using Xmf2.NavigationGraph.Core.Interfaces;

namespace Xmf2.NavigationGraph.Droid.InnerStacks
{
	internal static class InnerStackExtensions
	{
		internal static ActivityInnerStack<TViewModel> GetActivityInnerStack<TViewModel>(this InnerStack<TViewModel> item) where TViewModel : IViewModel
		{
			switch (item)
			{
				case null:
					return null;
				case ActivityInnerStack<TViewModel> res:
					return res;
				case DialogFragmentInnerStack<TViewModel> dialogFragmentInnerStack:
					return dialogFragmentInnerStack.FragmentHost;
				case FragmentInnerStack<TViewModel> fragmentInnerStack:
					return fragmentInnerStack.FragmentHost;
				default:
					throw new InvalidOperationException($"Unsupported inner stack type : {item.GetType().Name}");
			}
		}
	}
}