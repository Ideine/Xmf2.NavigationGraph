using System;
using Android.OS;
using Android.Runtime;
using AndroidX.AppCompat.App;
using Xmf2.NavigationGraph.Core.Interfaces;
using Xmf2.NavigationGraph.Droid.Interfaces;

namespace Xmf2.NavigationGraph.Droid.Bases
{
	public abstract class BaseFragmentActivity<TViewModel> : AppCompatActivity, IFragmentActivity where TViewModel : IViewModel
	{
		private const string SAVE = nameof(SAVE);

		public abstract int FragmentContainerId { get; }

		public BaseFragmentActivity() { }

		protected BaseFragmentActivity(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer) { }

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			if (!savedInstanceState?.ContainsKey(SAVE) ?? true)
			{
				if (Intent?.Extras?.ContainsKey(NavigationConstants.FRAGMENT_START_PARAMETER_CODE) ?? false)
				{
					string navigationKey = Intent.Extras.GetString(NavigationConstants.FRAGMENT_START_PARAMETER_CODE);
					IDeferredNavigationAction deferredNavigationAction = NavigationParameterContainer<TViewModel>.GetDeferredNavigationAction(navigationKey);
					deferredNavigationAction.Execute(this);
				}
			}
		}

		protected override void OnSaveInstanceState(Bundle outState)
		{
			outState.PutBoolean(SAVE, true);
			base.OnSaveInstanceState(outState);
		}
	}
}