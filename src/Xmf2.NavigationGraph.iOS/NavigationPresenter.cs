using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UIKit;
using Xmf2.DisposableExtensions;
using Xmf2.NavigationGraph.Core;
using Xmf2.NavigationGraph.Core.Extensions;
using Xmf2.NavigationGraph.Core.Interfaces;
using Xmf2.NavigationGraph.Core.NavigationActions;
using Xmf2.NavigationGraph.iOS.Factories;
using Xmf2.NavigationGraph.iOS.InnerStacks;
using Xmf2.NavigationGraph.iOS.Interfaces;
using Xmf2.NavigationGraph.iOS.Operations;

namespace Xmf2.NavigationGraph.iOS
{
	public class NavigationPresenter : IPresenterService, IRegistrationPresenterService
	{
		private readonly Dictionary<ScreenDefinition, ControllerInformation> _factoryAssociation = new Dictionary<ScreenDefinition, ControllerInformation>();
		private readonly UIWindow _window;
		private readonly NavigationStack _navigationStack = new NavigationStack();

		public NavigationPresenter(UIWindow window)
		{
			_window = window;
		}

		public void Associate(ScreenDefinition screenDefinition, ViewCreator controllerFactory)
		{
			_factoryAssociation[screenDefinition] = new ControllerInformation(controllerFactory, isModal: false);
		}

		public void AssociateModal(ScreenDefinition screenDefinition, ViewCreator controllerFactory)
		{
			_factoryAssociation[screenDefinition] = new ControllerInformation(controllerFactory, isModal: true);
		}

		public async Task UpdateNavigation(NavigationOperation navigationOperation, INavigationInProgress navigationInProgress)
		{
			if (navigationOperation.Pushes.Count == 0 && navigationOperation.Pops.Count == 0)
			{
				return;
			}

			List<PushInformation> controllersToPush = navigationOperation.Pushes.ConvertAll(x => new PushInformation(_factoryAssociation[x.Screen], x.Instance));

			foreach (PushAction push in navigationOperation.Pushes)
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
				_navigationStack.EnsureInitialized(_window);

				_navigationStack.ApplyActions(navigationOperation.Pops.Count, controllersToPush, callbackActionWaiter);

				// lines below could be commented if we encounter issues with viewmodel disposing
				callbackActionWaiter.Add(() =>
				{
					foreach (PopAction pop in navigationOperation.Pops)
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