using System.Collections.Generic;
using Xmf2.NavigationGraph.Core.Interfaces;

namespace Xmf2.NavigationGraph.Core.Graph
{
	internal class Node<TViewModel> where TViewModel : IViewModel
	{
		public bool IsEntryPoint { get; }

		public ScreenDefinition<TViewModel> Screen { get; }

		public List<Node<TViewModel>> NextNodes { get; } = new List<Node<TViewModel>>();

		public Node(ScreenDefinition<TViewModel> screen, bool isEntryPoint)
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