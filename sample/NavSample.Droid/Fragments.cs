using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.Fragment.App;

namespace NavSample.Droid
{
	public class BaseFragment : Fragment
	{
		private readonly string _text;

		public BaseFragment()
		{
			_text = GetType().Name;
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			var layout = inflater.Inflate(Resource.Layout.GenericFragment, container, false);
			layout.FindViewById<TextView>(Resource.Id.fragmentText).Text = _text;

			ViewHelper.AddViews((LinearLayout)layout);
			
			return layout;
		}
	}
	
	public class BaseDialogFragment : DialogFragment
	{
		private readonly string _text;

		public BaseDialogFragment()
		{
			_text = GetType().Name;
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			var layout = inflater.Inflate(Resource.Layout.GenericFragment, container, false);
			layout.FindViewById<TextView>(Resource.Id.fragmentText).Text = _text;

			ViewHelper.AddViews((LinearLayout)layout);

			return layout;
		}

		public override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetStyle(DialogFragment.StyleNoTitle, Resource.Style.Theme_DialogFragment);
		}
	}
	
	public class HomeFragment : BaseFragment{ }
	public class MenuFragment : BaseDialogFragment{ }
	public class OfferListFragment : BaseFragment{ }
	public class OfferDetailFragment : BaseFragment{ }
	public class ProductFragment : BaseFragment{ }
	public class ProfileFragment : BaseFragment{ }
	public class UpdatePasswordFragment : BaseFragment{ }
	public class CGUFragment : BaseDialogFragment{ }

	public static class ViewHelper
	{
		public static void AddViews(LinearLayout layout)
		{
			List<(string text, Func<Task> onClick)> items = new List<(string text, Func<Task> onClick)>
			{
				("back", Bootstrap.Navigation.Close),
				("home", Bootstrap.Navigation.ShowHome),
				("menu", Bootstrap.Navigation.ShowMenu),
				("profile", Bootstrap.Navigation.ShowProfile),
				("updatepwd", Bootstrap.Navigation.ShowUpdatePassword),
				("cgu", Bootstrap.Navigation.ShowCGU),
				("login", Bootstrap.Navigation.ShowLogin),
				("offer list", Bootstrap.Navigation.ShowOfferList),
				("offer 1", () => Bootstrap.Navigation.ShowOfferDetail("offer1")),
				("offer 2", () => Bootstrap.Navigation.ShowOfferDetail("offer2")),
				("product 1", () => Bootstrap.Navigation.ShowProduct("product1")),
				("product 2", () => Bootstrap.Navigation.ShowProduct("product2")),
			};

			LinearLayout child = null;
			foreach ((string text, Func<Task> onClick) in items)
			{
				Button button = new Button(layout.Context) {Text = text};
				var x = onClick;
				button.Click += async (sender, args) =>
				{
					await x();
			 	};

				if (child == null)
				{
					child = new LinearLayout(layout.Context);
					
					child.AddView(button, new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent, 1));
					layout.AddView(child, new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent));
				}
				else
				{
					child.AddView(button, new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent, 1));
					child = null;
				}
				
			}
		}
	}
}