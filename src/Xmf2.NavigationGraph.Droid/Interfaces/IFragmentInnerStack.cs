#if __ANDROID_29__
using AndroidX.Fragment.App;
#else
using Android.Support.V4.App;
#endif

namespace Xmf2.NavigationGraph.Droid.Interfaces
{
	internal interface IFragmentInnerStack
	{
		Fragment Fragment { get; }

		string FragmentTag { get; }
	}
}