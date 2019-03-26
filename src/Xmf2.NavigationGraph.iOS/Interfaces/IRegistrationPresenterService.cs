using Xmf2.NavigationGraph.Core;
using Xmf2.NavigationGraph.Core.Interfaces;

namespace Xmf2.NavigationGraph.iOS.Interfaces
{
	public interface IRegistrationPresenterService<TViewModel> where TViewModel : IViewModel
	{
		void Associate(ScreenDefinition<TViewModel> screenDefinition, ViewCreator<TViewModel> controllerFactory);
		void AssociateModal(ScreenDefinition<TViewModel> screenDefinition, ViewCreator<TViewModel> controllerFactory);
	}
}