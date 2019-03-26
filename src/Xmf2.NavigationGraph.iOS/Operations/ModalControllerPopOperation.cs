using System;
using Xmf2.DisposableExtensions;
using Xmf2.NavigationGraph.Core;
using Xmf2.NavigationGraph.iOS.InnerStacks;

namespace Xmf2.NavigationGraph.iOS.Operations
{
	public class ModalControllerPopOperation : PopOperation<ModalControllerInnerStack>
	{
		public ModalControllerPopOperation(ModalControllerInnerStack hostStack) : base(hostStack) { }

		public override void Execute(CallbackActionWaiter callbackActionWaiter, bool animated)
		{
			if (HostStack.Modal is NavigationControllerInnerStack navigationControllerInnerStack)
			{
				callbackActionWaiter.WaitOne();
				navigationControllerInnerStack.Host.DismissViewController(animated, callbackActionWaiter.ReleaseOne);
				callbackActionWaiter.Add(() => navigationControllerInnerStack.Host.SafeDispose());
			}
			else if (HostStack.Modal is SimpleControllerInnerStack simpleControllerInnerStack)
			{
				callbackActionWaiter.WaitOne();
				simpleControllerInnerStack.Controller.DismissViewController(animated, callbackActionWaiter.ReleaseOne);
				callbackActionWaiter.Add(() => simpleControllerInnerStack.Controller.SafeDispose());
			}
			else
			{
				throw new NotSupportedException($"Unsupported type of {HostStack.Modal.GetType().Name}");
			}
		}
	}
}