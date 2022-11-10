using AndroidX.Fragment.App;

namespace Xmf2.NavigationGraph.Droid.Interfaces
{
	internal interface IFragmentInnerStack
	{
		Fragment Fragment { get; }

		string FragmentTag { get; }
	}
}