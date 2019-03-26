namespace Xmf2.NavigationGraph.Core.Interfaces
{
	public interface IViewModelLocatorService<TViewModel> where TViewModel : IViewModel
	{
		void AddViewModel(string route, TViewModel viewModel);

		TViewModel GetViewModel(string route);
	}
}