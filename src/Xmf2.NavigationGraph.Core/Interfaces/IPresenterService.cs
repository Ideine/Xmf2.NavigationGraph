using System.Threading.Tasks;

namespace Xmf2.NavigationGraph.Core.Interfaces
{
	public interface IPresenterService<TViewModel> where TViewModel : IViewModel
	{
		Task UpdateNavigation(NavigationOperation<TViewModel> navigationOperation, INavigationInProgress navigationInProgress);

		void CloseApp();
	}
}