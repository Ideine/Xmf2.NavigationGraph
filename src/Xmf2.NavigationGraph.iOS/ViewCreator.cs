using UIKit;
using Xmf2.NavigationGraph.Core.Interfaces;

namespace Xmf2.NavigationGraph.iOS
{
	public delegate UIViewController ViewCreator<in TViewModel>(TViewModel viewModel) where TViewModel : IViewModel;
}