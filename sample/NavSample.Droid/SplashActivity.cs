﻿using Android.App;
using Android.OS;
using Android.Support.V7.App;
using NavSample;

namespace NavSample.Droid
{
	[Activity(Label = "NavRefacto.Droid", MainLauncher = true, Icon = "@drawable/icon", Theme = "@style/AppTheme_NoActionBar")]
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