using System.Threading.Tasks;
using Xmf2.NavigationGraph.Core.Interfaces;

namespace Xmf2.NavigationGraph.Core
{
	public delegate TViewModel SyncViewModelCreator<TViewModel>(string route) where TViewModel : IViewModel;
	public delegate Task<TViewModel> ViewModelCreator<TViewModel>(string route) where TViewModel : IViewModel;
}