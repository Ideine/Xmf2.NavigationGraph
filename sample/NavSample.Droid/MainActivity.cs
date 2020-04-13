using Android.App;
using Android.OS;
using Android.Widget;
using AndroidX.AppCompat.App;
using NavSample.Core;
using Xmf2.NavigationGraph.Core.Interfaces;
using Xmf2.NavigationGraph.Droid;
using Xmf2.NavigationGraph.Droid.Interfaces;

namespace NavSample.Droid
{
	public abstract class BaseActivity : AppCompatActivity
	{
		protected IViewModel ViewModel { get; private set; }
		
		protected abstract int LayoutId { get; }

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			SetContentView(LayoutId);
			
			if (Intent?.Extras != null)
			{
				if (Intent.Extras.ContainsKey(NavigationConstants.VIEWMODEL_LINK_PARAMETER_CODE))
				{
					string viewModelKey = Intent.Extras.GetString(NavigationConstants.VIEWMODEL_LINK_PARAMETER_CODE);
					ViewModel = NavigationParameterContainer<SampleViewModel>.GetViewModel(viewModelKey);
				}

				if (Intent.Extras.ContainsKey(NavigationConstants.FRAGMENT_START_PARAMETER_CODE))
				{
					string navigationKey = Intent.Extras.GetString(NavigationConstants.FRAGMENT_START_PARAMETER_CODE);
					IDeferredNavigationAction deferredNavigationAction = NavigationParameterContainer<SampleViewModel>.GetDeferredNavigationAction(navigationKey);
					deferredNavigationAction.Execute(this);
				}
			}
		}
	}
	
	[Activity(Theme = "@style/AppTheme_NoActionBar")]
	public class MainActivity : BaseActivity, IFragmentActivity
	{
		protected override int LayoutId { get; } = Resource.Layout.Main;
		public int FragmentContainerId => Resource.Id.FragmentRootView;

		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);
		}
	}
	
	[Activity(Theme = "@style/AppTheme_NoActionBar")]
	public class LoginActivity : BaseActivity
	{
		protected override int LayoutId { get; } = Resource.Layout.Login;

		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);

			LinearLayout layout = FindViewById<LinearLayout>(Resource.Id.container);
			ViewHelper.AddViews(layout);
		}
	}
}