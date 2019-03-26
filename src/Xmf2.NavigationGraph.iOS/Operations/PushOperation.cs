using Xmf2.NavigationGraph.Core;
using Xmf2.NavigationGraph.iOS.InnerStacks;

namespace Xmf2.NavigationGraph.iOS.Operations
{
	public abstract class PushOperation
	{
		public abstract void Execute(CallbackActionWaiter callbackActionWaiter, bool animated);
	}

	public abstract class PushOperation<THostStack> : PushOperation
		where THostStack : InnerStack
	{
		public THostStack HostStack { get; }

		protected PushOperation(THostStack hostStack)
		{
			HostStack = hostStack;
		}
	}
}