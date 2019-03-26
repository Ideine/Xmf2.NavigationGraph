using System.Collections.Generic;

namespace Xmf2.NavigationGraph.Core.Graph
{
	internal class Node
	{
		public bool IsEntryPoint { get; }

		public ScreenDefinition Screen { get; }

		public List<Node> NextNodes { get; } = new List<Node>();

		public Node(ScreenDefinition screen, bool isEntryPoint)
		{
			Screen = screen;
			IsEntryPoint = isEntryPoint;
		}

		public override string ToString()
		{
			return Screen.ToString();
		}
	}
}