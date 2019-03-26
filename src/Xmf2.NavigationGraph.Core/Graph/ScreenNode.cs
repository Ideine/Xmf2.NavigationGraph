using Xmf2.NavigationGraph.Core.Interfaces;

namespace Xmf2.NavigationGraph.Core.Graph
{
	internal class ScreenNode<TViewModel> where TViewModel : IViewModel
	{
		public Node<TViewModel> Node { get; }

		public ScreenInstance<TViewModel> ScreenInstance { get; }

		public ScreenNode(Node<TViewModel> node, ScreenInstance<TViewModel> screenInstance)
		{
			Node = node;
			ScreenInstance = screenInstance;
		}

		public override string ToString()
		{
			return ScreenInstance.ToString();
		}
	}
}