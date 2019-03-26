using Xmf2.NavigationGraph.Core;

namespace Xmf2.NavigationGraph.iOS.Operations
{
	public abstract class MergedPopPushOperation
	{
		public abstract void Execute(CallbackActionWaiter callbackActionWaiter, bool animated);
	}
}