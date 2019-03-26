using UIKit;
using Xmf2.NavigationGraph.Core;
using Xmf2.NavigationGraph.iOS.InnerStacks;

namespace Xmf2.NavigationGraph.iOS.Operations
{
	public class ModalControllerPushOperation : PushOperation<InnerStack>
	{
		private InnerStack Controller { get; }

		public ModalControllerPushOperation(InnerStack hostStack, InnerStack controller) : base(hostStack)
		{
			Controller = controller;
		}

		public override void Execute(CallbackActionWaiter callbackActionWaiter, bool animated)
		{
			UIViewController host = Controller.Host;

			var vc = Controller.AsViewController();
			callbackActionWaiter.WaitOne();
			host.PresentViewController(vc, animated, callbackActionWaiter.ReleaseOne);
		}
	}
}