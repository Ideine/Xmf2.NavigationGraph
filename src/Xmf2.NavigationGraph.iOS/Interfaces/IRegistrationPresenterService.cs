using Xmf2.NavigationGraph.Core;

namespace Xmf2.NavigationGraph.iOS.Interfaces
{
	public interface IRegistrationPresenterService
	{
		void Associate(ScreenDefinition screenDefinition, ViewCreator controllerFactory);
		void AssociateModal(ScreenDefinition screenDefinition, ViewCreator controllerFactory);
	}
}