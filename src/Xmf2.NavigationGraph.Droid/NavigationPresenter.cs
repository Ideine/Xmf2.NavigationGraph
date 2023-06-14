using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AndroidX.AppCompat.App;
using AndroidX.Fragment.App;
using Xmf2.DisposableExtensions;
using Xmf2.NavigationGraph.Core;
using Xmf2.NavigationGraph.Core.Interfaces;
using Xmf2.NavigationGraph.Droid.Factories;
using Xmf2.NavigationGraph.Droid.Interfaces;

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

	public class NavigationPresenter<TViewModel> : IPresenterService<TViewModel>, IRegistrationPresenterService<TViewModel> where TViewModel : IViewModel
	{
		private readonly Dictionary<ScreenDefinition<TViewModel>, ViewFactory> _factoryAssociation = new();
		private readonly NavigationStack<TViewModel> _navigationStack;
		private ActivityViewFactory _defaultFragmentHost;

		public NavigationPresenter(IViewModelLocatorService<TViewModel> viewModelLocatorService)
		{
			_navigationStack = new NavigationStack<TViewModel>(viewModelLocatorService);
		}

		public void RegisterDefaultFragmentHost<TActivity>(bool shouldClearHistory = false) where TActivity : AppCompatActivity
		{
			_defaultFragmentHost = new ActivityViewFactory(typeof(TActivity), shouldClearHistory);
		}

		public void AssociateActivity<TActivity>(ScreenDefinition<TViewModel> screenDefinition, bool shouldClearHistory = false) where TActivity : AppCompatActivity
		{
			_factoryAssociation[screenDefinition] = new ActivityViewFactory(typeof(TActivity), shouldClearHistory);
		}

		public void AssociateFragment<TFragment>(ScreenDefinition<TViewModel> screenDefinition, Func<TFragment> fragmentCreator, Type hostActivityType = null, bool? shouldClearHistory = null)
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

		public void AssociateFragment<TFragmentHost, TFragment>(ScreenDefinition<TViewModel> screenDefinition, Func<TFragment> fragmentCreator)
			where TFragmentHost : AppCompatActivity
			where TFragment : Fragment
		{
			AssociateFragment(screenDefinition, fragmentCreator, typeof(TFragmentHost));
		}

		public void AssociateDialogFragment<TDialogFragment>(ScreenDefinition<TViewModel> screenDefinition, Func<TDialogFragment> fragmentCreator, Type hostActivityType = null, bool? shouldClearHistory = null)
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

		public void AssociateDialogFragment<TFragmentHost, TDialogFragment>(ScreenDefinition<TViewModel> screenDefinition, Func<TDialogFragment> fragmentCreator)
			where TFragmentHost : AppCompatActivity
			where TDialogFragment : DialogFragment
		{
			AssociateDialogFragment(screenDefinition, fragmentCreator, typeof(TFragmentHost));
		}

		public void ReplaceDisposedFragment(Fragment fragment)
		{
			_navigationStack.ReplaceDisposedFragment(fragment);
		}

		public async Task UpdateNavigation(NavigationOperation<TViewModel> navigationOperation, INavigationInProgress navigationInProgress)
		{
			if (navigationOperation.Pushes.Count == 0 && navigationOperation.Pops.Count == 0)
			{
				return;
			}

			var controllersToPush = navigationOperation.Pushes.Select(x => new PushInformation<TViewModel>(_factoryAssociation[x.Screen], x.Instance)).ToList();

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

			Microsoft.Maui.ApplicationModel.Platform.CurrentActivity.RunOnUiThread(() => _navigationStack.ApplyActions(navigationOperation.Pops.Count, controllersToPush));
		}

		public void CloseApp()
		{
			Microsoft.Maui.ApplicationModel.Platform.CurrentActivity.Finish();
		}
	}
}