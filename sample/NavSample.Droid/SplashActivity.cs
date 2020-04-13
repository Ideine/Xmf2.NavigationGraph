using Android.App;
using Android.OS;
using AndroidX.AppCompat.App;

namespace NavSample.Droid
{
	[Activity(Label = "NavSample.Droid", MainLauncher = true, Icon = "@drawable/icon", Theme = "@style/AppTheme_NoActionBar")]
	public class SplashActivity : AppCompatActivity
	{
		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);

			SetContentView(Resource.Layout.Splash);

			Bootstrap.Initialize();
		}
	}
}