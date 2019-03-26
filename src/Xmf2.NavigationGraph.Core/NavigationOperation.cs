using System.Collections.Generic;
using Xmf2.NavigationGraph.Core.NavigationActions;

namespace Xmf2.NavigationGraph.Core
{
	public class NavigationOperation
	{
		private readonly List<PopAction> _pops = new List<PopAction>(4);
		private readonly List<PushAction> _pushes = new List<PushAction>(4);

		public IReadOnlyList<PopAction> Pops => _pops;
		public IReadOnlyList<PushAction> Pushes => _pushes;

		internal void Add(PopAction action)
		{
			_pops.Add(action);
		}

		internal void Add(PushAction action)
		{
			_pushes.Add(action);
		}
	}
}