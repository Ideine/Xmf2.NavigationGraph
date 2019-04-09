using System;
using CoreGraphics;
using UIKit;

namespace NavSample.iOS
{
	public abstract class BaseController : UIViewController
	{
		public BaseController()
		{
			
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			if (NavigationController != null)
			{
				NavigationController.NavigationBarHidden = true;
			}

			string name = GetType().Name;
			name = name.Substring(0, name.Length - "Controller".Length);

			UILabel label = new UILabel(new CGRect(20, 80, 500, 20))
			{
				Text = name, 
				TextColor = UIColor.Black
			};
			
			UIButton button = new UIButton(new CGRect(20, 40, 100, 40));
			button.SetTitle("< back", UIControlState.Normal);
			button.SetTitleColor(UIColor.White, UIControlState.Normal);
			button.BackgroundColor = UIColor.Blue;
			button.TouchUpInside += (sender, args) =>
			{
				Console.WriteLine("Back clicked");
				Bootstrap.Navigation.Close();
			};

			View.Add(label);
			
			
			View.AddSubviews(
				Create("home", () => Bootstrap.Navigation.ShowHome()),
				Create("menu", () => Bootstrap.Navigation.ShowMenu()),
				Create("profile", () => Bootstrap.Navigation.ShowProfile()),
				Create("passwd", () => Bootstrap.Navigation.ShowUpdatePassword()),
				Create("cgu", () => Bootstrap.Navigation.ShowCGU()),
				Create("login", () => Bootstrap.Navigation.ShowLogin()),
				Create("offers", () => Bootstrap.Navigation.ShowOfferList()),
				Create("offer1", () => Bootstrap.Navigation.ShowOfferDetail("offer1")),
				Create("offer2", () => Bootstrap.Navigation.ShowOfferDetail("offer2")),
				Create("offer3", () => Bootstrap.Navigation.ShowOfferDetail("offer3")),
				Create("product1", () => Bootstrap.Navigation.ShowProduct("product1")),
				Create("product2", () => Bootstrap.Navigation.ShowProduct("product2"))
			);
			
			View.BackgroundColor = UIColor.White;
			View.Add(button);
		}

		private int _buttonOffsetX = 20;
		private int _buttonOffsetY = 180;

		private UIButton Create(string label, Action action)
		{
			UIButton button = new UIButton(new CGRect(_buttonOffsetX, _buttonOffsetY, 100, 40));
			button.SetTitle(label, UIControlState.Normal);
			button.SetTitleColor(UIColor.Black, UIControlState.Normal);
			button.TouchUpInside += (sender, args) => action();

			_buttonOffsetX += 100;

			if (_buttonOffsetX + 100 > UIScreen.MainScreen.Bounds.Width)
			{
				_buttonOffsetY += 60;
				_buttonOffsetX = 20;
			}

			return button;
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			System.Diagnostics.Debug.WriteLine($"DisposeView({disposing}): {GetType().Name}");
		}
	}
	
	public class HomeController : BaseController { }
	public class MenuController : BaseController { }
	public class OfferListController : BaseController { }
	public class OfferDetailController : BaseController { }
	public class ProductController : BaseController { }
	public class ProfileController : BaseController { }
	public class UpdatePasswordController : BaseController { }
	public class CGUController : BaseController { }
	public class LoginController : BaseController { }
}