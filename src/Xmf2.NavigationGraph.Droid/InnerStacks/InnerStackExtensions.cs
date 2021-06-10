using System;
using Xmf2.NavigationGraph.Core.Interfaces;

namespace Xmf2.NavigationGraph.Droid.InnerStacks
{
	internal static class InnerStackExtensions
	{
		internal static ActivityInnerStack<TViewModel> GetActivityInnerStack<TViewModel>(this InnerStack<TViewModel> item) where TViewModel : IViewModel
		{
			return item switch
			{
				null => null,
				ActivityInnerStack<TViewModel> res => res,
				DialogFragmentInnerStack<TViewModel> dialogFragmentInnerStack => dialogFragmentInnerStack.FragmentHost,
				FragmentInnerStack<TViewModel> fragmentInnerStack => fragmentInnerStack.FragmentHost,
				_ => throw new InvalidOperationException($"Unsupported inner stack type : {item.GetType().Name}")
			};
		}
	}
}