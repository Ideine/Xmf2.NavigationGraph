using Xmf2.NavigationGraph.Droid.Operations;

namespace Xmf2.NavigationGraph.Droid.InnerStacks
{
	internal abstract class InnerStack
	{
		public NavigationStack NavigationStack { get; }

		protected InnerStack(NavigationStack navigationStack)
		{
			NavigationStack = navigationStack;
		}

		public abstract int Count { get; }

		public abstract PopOperation AsPopOperation();

		public abstract PopOperation AsSpecificPopOperation(InnerStack child);
		public abstract PopOperation AsSpecificPopOperation(int count);
	}
}