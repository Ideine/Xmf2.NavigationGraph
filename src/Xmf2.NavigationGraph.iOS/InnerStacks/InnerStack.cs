using UIKit;
using Xmf2.NavigationGraph.iOS.Operations;

namespace Xmf2.NavigationGraph.iOS.InnerStacks
{
	public abstract class InnerStack
	{
		protected InnerStack(UIViewController host)
		{
			Host = host;
		}

		public UIViewController Host { get; }

		public abstract int Count { get; }

		public abstract PopOperation AsPopOperation();

		public abstract PopOperation AsSpecificPopOperation(InnerStack child);
		public abstract PopOperation AsSpecificPopOperation(int count);

		public abstract UIViewController AsViewController();
	}
}