using System.Collections.Generic;
using UIKit;
using Xmf2.DisposableExtensions;
using Xmf2.NavigationGraph.Core;

namespace Xmf2.NavigationGraph.iOS.Operations
{
	public class MergedPopPushNavigationControllerOperation : MergedPopPushOperation
	{
		public NavigationControllerPopOperation Pop { get; }

		public NavigationControllerPushOperation Push { get; }

		public MergedPopPushNavigationControllerOperation(NavigationControllerPopOperation pop, NavigationControllerPushOperation push)
		{
			Pop = pop;
			Push = push;
		}

		public override void Execute(CallbackActionWaiter callbackActionWaiter, bool animated)
		{
			var navigationController = (UINavigationController)Pop.HostStack.Host;
			var vcs = navigationController.ViewControllers;
			UIViewController[] newVcs;
			var controllersToDispose = new List<UIViewController>(Pop.CountToPop);

			int newCount = vcs.Length - Pop.CountToPop + Push.Controllers.Count;
			if (vcs.Length == newCount)
			{
				newVcs = vcs;

				for (int j = vcs.Length - Pop.CountToPop ; j < vcs.Length ; ++j)
				{
					controllersToDispose.Add(newVcs[j]);
				}
			}
			else
			{
				newVcs = new UIViewController[vcs.Length - Pop.CountToPop + Push.Controllers.Count];

				int copyCount = vcs.Length - Pop.CountToPop;
				for (int i = 0 ; i < copyCount ; ++i)
				{
					newVcs[i] = vcs[i];
				}

				for (int j = copyCount ; j < vcs.Length ; ++j)
				{
					controllersToDispose.Add(vcs[j]);
				}
			}

			for (int i = 0, j = vcs.Length - Pop.CountToPop ; j < newVcs.Length ; ++i, ++j)
			{
				newVcs[j] = Push.Controllers[i].AsViewController();
			}

			navigationController.SetViewControllers(newVcs, animated);

			callbackActionWaiter.Add(() =>
			{
				foreach (UIViewController controller in controllersToDispose)
				{
					controller.SafeDispose();
				}
			});
		}
	}
}