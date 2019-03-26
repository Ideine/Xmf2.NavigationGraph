using System;

namespace Xmf2.NavigationGraph.Droid.InnerStacks
{
	internal static class InnerStackExtensions
	{
		internal static ActivityInnerStack GetActivityInnerStack(this InnerStack item)
		{
			switch (item)
			{
				case null:
					return null;
				case ActivityInnerStack res:
					return res;
				case DialogFragmentInnerStack dialogFragmentInnerStack:
					return dialogFragmentInnerStack.FragmentHost;
				case fragmentInnerStack fragmentInnerStack:
					return fragmentInnerStack.FragmentHost;
				default:
					throw new InvalidOperationException($"Unsupported inner stack type : {item.GetType().Name}");
			}
		}
	}
}