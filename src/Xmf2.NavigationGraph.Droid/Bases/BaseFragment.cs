using System;
using Android.OS;
using Android.Runtime;
#if __ANDROID_29__
using AndroidX.Fragment.App;
#else
using Android.Support.V4.App;
#endif
using Xmf2.NavigationGraph.Core.Interfaces;
using Xmf2.NavigationGraph.Droid.Interfaces;

namespace Xmf2.NavigationGraph.Droid.Bases
{
	public abstract class BaseFragment<TViewModel> : Fragment, IScreenView where TViewModel : class, IViewModel
	{
		protected const string VIEWMODEL_ROUTE = nameof(VIEWMODEL_ROUTE);

		protected abstract IRegistrationPresenterService<TViewModel> PresenterService { get; }

		protected virtual TViewModel ViewModel { get; set; }

		public string ScreenRoute { get; set; }

		protected abstract IViewModelLocatorService<TViewModel> ViewModelLocatorService { get; }

		protected BaseFragment() { }

		protected BaseFragment(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer) { }

		public override void OnSaveInstanceState(Bundle outState)
		{
			outState.PutString(VIEWMODEL_ROUTE, ScreenRoute);
			base.OnSaveInstanceState(outState);
		}

		public override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			if (string.IsNullOrEmpty(ScreenRoute) && savedInstanceState != null)
			{
				ScreenRoute = savedInstanceState.GetString(VIEWMODEL_ROUTE);
			}

			ViewModel = ViewModelLocatorService.GetViewModel(ScreenRoute);

			if (savedInstanceState != null)
			{
				PresenterService?.ReplaceDisposedFragment(this);
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				ViewModel = null;
			}

			base.Dispose(disposing);
		}
	}
}