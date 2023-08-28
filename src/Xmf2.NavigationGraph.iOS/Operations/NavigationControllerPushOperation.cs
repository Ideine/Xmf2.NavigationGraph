using System.Collections.Generic;
using UIKit;
using Xmf2.NavigationGraph.Core;
using Xmf2.NavigationGraph.iOS.InnerStacks;

namespace Xmf2.NavigationGraph.iOS.Operations
{
	public class NavigationControllerPushOperation : PushOperation<NavigationControllerInnerStack>
	{
		public List<InnerStack> Controllers { get; } = new();

		public NavigationControllerPushOperation(NavigationControllerInnerStack hostStack) : base(hostStack) { }

		public override void Execute(CallbackActionWaiter callbackActionWaiter, bool animated)
		{
			UINavigationController navigationController = (UINavigationController)HostStack.Host;
			if (Controllers.Count == 1)
			{
				navigationController.PushViewController(Controllers[0].AsViewController(), animated);
			}
			else
			{
				UIViewController[] vcs = navigationController.ViewControllers;
				UIViewController[] newVcs = new UIViewController[vcs.Length + Controllers.Count];

				for (int i = 0 ; i < vcs.Length ; i++)
				{
					newVcs[i] = vcs[i];
				}

				for (int i = 0, j = vcs.Length ; j < newVcs.Length ; ++i, ++j)
				{
					newVcs[j] = Controllers[i].AsViewController();
				}

				navigationController.SetViewControllers(newVcs, animated);
			}
		}
	}
}