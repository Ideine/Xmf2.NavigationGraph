namespace Xmf2.NavigationGraph.Core.Interfaces
{
	public interface IViewModelLocatorService
	{
		void AddViewModel(string route, IViewModel viewModel);

		IViewModel GetViewModel(string route);
	}
}