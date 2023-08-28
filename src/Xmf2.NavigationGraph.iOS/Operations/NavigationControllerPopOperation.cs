using System.Collections.Generic;
using System.Linq;
using UIKit;
using Xmf2.DisposableExtensions;
using Xmf2.NavigationGraph.Core;
using Xmf2.NavigationGraph.iOS.InnerStacks;

namespace Xmf2.NavigationGraph.iOS.Operations
{
	public class NavigationControllerPopOperation : PopOperation<NavigationControllerInnerStack>
	{
		public int CountToPop { get; }

		public NavigationControllerPopOperation(NavigationControllerInnerStack hostStack, int countToPop) : base(hostStack)
		{
			CountToPop = countToPop;
		}

		public override void Execute(CallbackActionWaiter callbackActionWaiter, bool animated)
		{
			UINavigationController navigationController = (UINavigationController)HostStack.Host;
			List<UIViewController> vcs = navigationController.ViewControllers.ToList();
			if (CountToPop == 1)
			{
				UIViewController poppedViewController = vcs[vcs.Count - 1];
				callbackActionWaiter.Add(() => poppedViewController.SafeDispose());
				navigationController.PopViewController(animated);
			}
			else
			{
				int popToIndex = vcs.Count - 1 - CountToPop;
				navigationController.PopToViewController(vcs[popToIndex], animated);
				callbackActionWaiter.Add(() =>
				{
					for (int i = popToIndex + 1 ; i < vcs.Count ; ++i)
					{
						vcs[i].SafeDispose();
					}
				});
			}
		}
	}
}