using System.Collections.Generic;
using Xmf2.NavigationGraph.Core.Interfaces;
using Xmf2.NavigationGraph.Core.NavigationActions;

namespace Xmf2.NavigationGraph.Core
{
	public class NavigationOperation<TViewModel> where TViewModel : IViewModel
	{
		private readonly List<PopAction<TViewModel>> _pops = new(4);
		private readonly List<PushAction<TViewModel>> _pushes = new(4);

		public IReadOnlyList<PopAction<TViewModel>> Pops => _pops;
		public IReadOnlyList<PushAction<TViewModel>> Pushes => _pushes;

		internal void Add(PopAction<TViewModel> action)
		{
			_pops.Add(action);
		}

		internal void Add(PushAction<TViewModel> action)
		{
			_pushes.Add(action);
		}
	}
}