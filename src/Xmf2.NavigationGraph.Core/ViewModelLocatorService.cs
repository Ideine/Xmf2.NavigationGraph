using System.Collections.Generic;
using Xmf2.NavigationGraph.Core.Interfaces;

namespace Xmf2.NavigationGraph.Core
{
	public class ViewModelLocatorService : IViewModelLocatorService
	{
		private readonly Dictionary<string, IViewModel> _locator = new Dictionary<string, IViewModel>();

		public void AddViewModel(string route, IViewModel viewModel)
		{
			_locator[route] = viewModel;
		}

		public IViewModel GetViewModel(string route)
		{
			return _locator.TryGetValue(route, out var vm) ? vm : null;
		}
	}
}