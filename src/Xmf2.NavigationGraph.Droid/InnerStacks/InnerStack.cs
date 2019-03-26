using Xmf2.NavigationGraph.Core.Interfaces;
using Xmf2.NavigationGraph.Droid.Operations;

namespace Xmf2.NavigationGraph.Droid.InnerStacks
{
	internal abstract class InnerStack<TViewModel> where TViewModel : IViewModel
	{
		public NavigationStack<TViewModel> NavigationStack { get; }

		protected InnerStack(NavigationStack<TViewModel> navigationStack)
		{
			NavigationStack = navigationStack;
		}

		public abstract int Count { get; }

		public abstract PopOperation AsPopOperation();

		public abstract PopOperation AsSpecificPopOperation(InnerStack<TViewModel> child);
		public abstract PopOperation AsSpecificPopOperation(int count);
	}
}