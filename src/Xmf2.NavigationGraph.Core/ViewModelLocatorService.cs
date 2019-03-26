using System.Collections.Generic;
using Xmf2.NavigationGraph.Core.Interfaces;

namespace Xmf2.NavigationGraph.Core
{
	public class ViewModelLocatorService<TViewModel> : IViewModelLocatorService<TViewModel> where TViewModel : IViewModel
	{
		private readonly Dictionary<string, TViewModel> _locator = new Dictionary<string, TViewModel>();

		public void AddViewModel(string route, TViewModel viewModel)
		{
			_locator[route] = viewModel;
		}

		public TViewModel GetViewModel(string route)
		{
			return _locator.TryGetValue(route, out var vm) ? vm : default(TViewModel);
		}
	}
}