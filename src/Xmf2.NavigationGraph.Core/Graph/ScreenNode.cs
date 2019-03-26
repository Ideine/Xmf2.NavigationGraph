namespace Xmf2.NavigationGraph.Core.Graph
{
	internal class ScreenNode
	{
		public Node Node { get; }

		public ScreenInstance ScreenInstance { get; }

		public ScreenNode(Node node, ScreenInstance screenInstance)
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