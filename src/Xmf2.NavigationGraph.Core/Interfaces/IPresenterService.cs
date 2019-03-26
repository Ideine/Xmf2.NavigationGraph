using System.Threading.Tasks;

namespace Xmf2.NavigationGraph.Core.Interfaces
{
	public interface IPresenterService
	{
		Task UpdateNavigation(NavigationOperation navigationOperation, INavigationInProgress navigationInProgress);

		void CloseApp();
	}
}