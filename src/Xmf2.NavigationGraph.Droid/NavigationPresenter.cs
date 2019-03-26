using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.App;
using Android.Support.V7.App;
using Plugin.CurrentActivity;
using Xmf2.DisposableExtensions;
using Xmf2.NavigationGraph.Core;
using Xmf2.NavigationGraph.Core.Extensions;
using Xmf2.NavigationGraph.Core.Interfaces;
using Xmf2.NavigationGraph.Droid.Factories;
using Xmf2.NavigationGraph.Droid.Interfaces;
using DialogFragment = Android.Support.V4.App.DialogFragment;
using Fragment = Android.Support.V4.App.Fragment;

namespace Xmf2.NavigationGraph.Droid
{
	/*
	 * Dans le cas où l'on voudrait show plusieurs activity :
	 * Intent intent = new Intent (this, MainActivity.class);
     *  TaskStackBuilder stackBuilder = TaskStackBuilder.create(this);
     *  stackBuilder.addParentStack(MainActivity.class);
     *  stackBuilder.addNextIntent(intent);
     *  Intent intentEmailView = new Intent (this, EmailViewActivity.class);
     *  intentEmailView.putExtra("EmailId","you can Pass emailId here");
     *  stackBuilder.addNextIntent(intentEmailView);
	 */

	public class NavigationPresenter : IPresenterService, IRegistrationPresenterService
	{
		public const string FRAGMENT_START_PARAMETER_CODE = nameof(FRAGMENT_START_PARAMETER_CODE);
		public const string VIEWMODEL_LINK_PARAMETER_CODE = nameof(VIEWMODEL_LINK_PARAMETER_CODE);

		private readonly Dictionary<ScreenDefinition, ViewFactory> _factoryAssociation = new Dictionary<ScreenDefinition, ViewFactory>();
		private readonly NavigationStack _navigationStack;
		private ActivityViewFactory _defaultFragmentHost;

		public NavigationPresenter(IViewModelLocatorService viewModelLocatorService)
		{
			_navigationStack = new NavigationStack(viewModelLocatorService);
		}

		internal static Activity CurrentActivity
		{
			get
			{
				var result = CrossCurrentActivity.Current.Activity;
				if (result is null)
				{
					throw new InvalidOperationException("Can not resolve current activity");
				}

				return result;
			}
		}

		public void RegisterDefaultFragmentHost<TActivity>(bool shouldClearHistory = false) where TActivity : AppCompatActivity
		{
			_defaultFragmentHost = new ActivityViewFactory(typeof(TActivity), shouldClearHistory);
		}

		public void AssociateActivity<TActivity>(ScreenDefinition screenDefinition, bool shouldClearHistory = false) where TActivity : AppCompatActivity
		{
			_factoryAssociation[screenDefinition] = new ActivityViewFactory(typeof(TActivity), shouldClearHistory);
		}

		public void AssociateFragment<TFragment>(ScreenDefinition screenDefinition, Func<TFragment> fragmentCreator, Type hostActivityType = null, bool? shouldClearHistory = null)
			where TFragment : Fragment
		{
			FragmentViewFactory res;
			if (hostActivityType is null)
			{
				if (_defaultFragmentHost is null)
				{
					throw new InvalidOperationException("No fragment host has been specified and none has been registered as default");
				}

				res = new FragmentViewFactory(fragmentCreator, _defaultFragmentHost.ActivityType, shouldClearHistory ?? _defaultFragmentHost.ShouldClearHistory);
			}
			else
			{
				res = new FragmentViewFactory(fragmentCreator, hostActivityType, shouldClearHistory ?? false);
			}

			_factoryAssociation[screenDefinition] = res;
		}

		public void AssociateFragment<TFragmentHost, TFragment>(ScreenDefinition screenDefinition, Func<TFragment> fragmentCreator)
			where TFragmentHost : AppCompatActivity
			where TFragment : Fragment
		{
			AssociateFragment(screenDefinition, fragmentCreator, typeof(TFragmentHost));
		}

		public void AssociateDialogFragment<TDialogFragment>(ScreenDefinition screenDefinition, Func<TDialogFragment> fragmentCreator, Type hostActivityType = null, bool? shouldClearHistory = null)
			where TDialogFragment : DialogFragment
		{
			DialogFragmentViewFactory res;
			if (hostActivityType is null)
			{
				if (_defaultFragmentHost is null)
				{
					throw new InvalidOperationException("No fragment host has been specified and none has been registered as default");
				}

				res = new DialogFragmentViewFactory(fragmentCreator, _defaultFragmentHost.ActivityType, shouldClearHistory ?? _defaultFragmentHost.ShouldClearHistory);
			}
			else
			{
				res = new DialogFragmentViewFactory(fragmentCreator, hostActivityType, shouldClearHistory ?? false);
			}

			_factoryAssociation[screenDefinition] = res;
		}

		public void AssociateDialogFragment<TFragmentHost, TDialogFragment>(ScreenDefinition screenDefinition, Func<TDialogFragment> fragmentCreator)
			where TFragmentHost : AppCompatActivity
			where TDialogFragment : DialogFragment
		{
			AssociateDialogFragment(screenDefinition, fragmentCreator, typeof(TFragmentHost));
		}

		public async Task UpdateNavigation(NavigationOperation navigationOperation, INavigationInProgress navigationInProgress)
		{
			if (navigationOperation.Pushes.Count == 0 && navigationOperation.Pops.Count == 0)
			{
				return;
			}

			var controllersToPush = navigationOperation.Pushes.ConvertAll(x => new PushInformation(_factoryAssociation[x.Screen], x.Instance));

			foreach (var push in navigationOperation.Pushes)
			{
				await push.Instance.GetViewModel(""); //TODO: add route here
			}

			if (navigationInProgress.IsCancelled)
			{
				Task.Run(async () =>
				{
					// we wait 10s just in case, shouldn't put too much memory pressure on GC
					await Task.Delay(10_000);

					foreach (var push in navigationOperation.Pushes)
					{
						push.Instance.ViewModelInstance?.SafeDispose();
					}
				}).ConfigureAwait(false);
				return;
			}

			navigationInProgress.Commit();

			CurrentActivity.RunOnUiThread(() => _navigationStack.ApplyActions(navigationOperation.Pops.Count, controllersToPush));
		}

		public void CloseApp()
		{
			CurrentActivity.Finish();
		}
	}
}