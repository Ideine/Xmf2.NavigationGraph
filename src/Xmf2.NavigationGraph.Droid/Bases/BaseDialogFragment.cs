using System;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Xmf2.NavigationGraph.Core.Interfaces;
using Xmf2.NavigationGraph.Droid.Interfaces;

namespace Xmf2.NavigationGraph.Droid.Bases
{
	public abstract class BaseDialogFragment<TViewModel> : DialogFragment, IScreenView where TViewModel : class, IViewModel
	{
		protected abstract IViewModelLocatorService<TViewModel> ViewModelLocatorService { get; }

		public string ScreenRoute { get; set; }

		protected virtual TViewModel ViewModel { get; set; }

		public BaseDialogFragment() { }

		protected BaseDialogFragment(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer) { }

		public override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			ViewModel = ViewModelLocatorService.GetViewModel(ScreenRoute);
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