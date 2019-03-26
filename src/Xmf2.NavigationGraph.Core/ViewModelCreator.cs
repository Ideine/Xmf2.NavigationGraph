using System.Threading.Tasks;
using Xmf2.NavigationGraph.Core.Interfaces;

namespace Xmf2.NavigationGraph.Core
{
	public delegate Task<IViewModel> ViewModelCreator(string route);
}