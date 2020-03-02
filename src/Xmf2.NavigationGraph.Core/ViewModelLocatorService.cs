using System.Collections.Generic;
using Xmf2.NavigationGraph.Core.Interfaces;

namespace Xmf2.NavigationGraph.Core
{
	public class ViewModelLocatorService<TViewModel> : IViewModelLocatorService<TViewModel> where TViewModel : IViewModel
	{
		#region Singleton

		private static IViewModelLocatorService<TViewModel> _instance;

		public static IViewModelLocatorService<TViewModel> Instance => _instance ??= new ViewModelLocatorService<TViewModel>();

		#endregion

		private ViewModelLocatorService() { }

		private readonly Dictionary<string, TViewModel> _locator = new Dictionary<string, TViewModel>();

		public void AddViewModel(string route, TViewModel viewModel)
		{
			_locator[route] = viewModel;
		}

		public TViewModel GetViewModel(string route)
		{
			return _locator.TryGetValue(route, out TViewModel vm) ? vm : default;
		}
	}
}