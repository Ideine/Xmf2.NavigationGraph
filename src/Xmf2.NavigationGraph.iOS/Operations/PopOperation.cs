using Xmf2.NavigationGraph.Core;
using Xmf2.NavigationGraph.iOS.InnerStacks;

namespace Xmf2.NavigationGraph.iOS.Operations
{
	public abstract class PopOperation
	{
		public abstract void Execute(CallbackActionWaiter callbackActionWaiter, bool animated);
	}

	public abstract class PopOperation<TInnerStack> : PopOperation
		where TInnerStack : InnerStack
	{
		protected PopOperation(TInnerStack hostStack)
		{
			HostStack = hostStack;
		}

		public TInnerStack HostStack { get; }
	}
}