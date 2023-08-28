using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UIKit;
using Xmf2.DisposableExtensions;
using Xmf2.NavigationGraph.Core;
using Xmf2.NavigationGraph.Core.Interfaces;
using Xmf2.NavigationGraph.Core.NavigationActions;
using Xmf2.NavigationGraph.iOS.Factories;
using Xmf2.NavigationGraph.iOS.Interfaces;

namespace Xmf2.NavigationGraph.iOS
{
	public class NavigationPresenter<TViewModel> : IPresenterService<TViewModel>, IRegistrationPresenterService<TViewModel> where TViewModel : IViewModel
	{
		private readonly Dictionary<ScreenDefinition<TViewModel>, ControllerInformation<TViewModel>> _factoryAssociation = new();
		private readonly UINavigationController _navigationController;
		private readonly NavigationStack<TViewModel> _navigationStack = new();

		public NavigationPresenter(UINavigationController navigationController)
		{
			_navigationController = navigationController;
		}

		public void Associate(ScreenDefinition<TViewModel> screenDefinition, ViewCreator<TViewModel> controllerFactory)
		{
			_factoryAssociation[screenDefinition] = new ControllerInformation<TViewModel>(controllerFactory, isModal: false);
		}

		public void AssociateModal(ScreenDefinition<TViewModel> screenDefinition, ViewCreator<TViewModel> controllerFactory)
		{
			_factoryAssociation[screenDefinition] = new ControllerInformation<TViewModel>(controllerFactory, isModal: true);
		}

		public async Task UpdateNavigation(NavigationOperation<TViewModel> navigationOperation, INavigationInProgress navigationInProgress)
		{
			if (navigationOperation.Pushes.Count == 0 && navigationOperation.Pops.Count == 0)
			{
				return;
			}

			var controllersToPush = navigationOperation.Pushes.Select(x => new PushInformation<TViewModel>(_factoryAssociation[x.Screen], x.Instance));

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
			UIApplication.SharedApplication.InvokeOnMainThread(() =>
			{
				var callbackActionWaiter = new CallbackActionWaiter();
				callbackActionWaiter.WaitOne();
				_navigationStack.EnsureInitialized(_navigationController);

				_navigationStack.ApplyActions(navigationOperation.Pops.Count, controllersToPush, callbackActionWaiter);

				// lines below could be commented if we encounter issues with viewmodel disposing
				callbackActionWaiter.Add(() =>
				{
					foreach (PopAction<TViewModel> pop in navigationOperation.Pops)
					{
						pop.Instance.ViewModelInstance?.DispatchSafeDispose();
					}
				});

				Task.Run(async () =>
				{
					// we wait 10s to let the time for navigation controller animations before disposing content
					await Task.Delay(10_000);
					UIApplication.SharedApplication.InvokeOnMainThread(callbackActionWaiter.ReleaseOne);
				});
			});
		}

		public void CloseApp()
		{
			throw new InvalidOperationException("Cannot be perform in this platform");
		}
	}
}