using Xmf2.NavigationGraph.Core.Interfaces;

namespace Xmf2.NavigationGraph.iOS.Factories
{
	public class ControllerInformation<TViewModel> where TViewModel : IViewModel
	{
		public ViewCreator<TViewModel> Factory { get; }

		public bool IsModal { get; }

		public ControllerInformation(ViewCreator<TViewModel> factory, bool isModal)
		{
			Factory = factory;
			IsModal = isModal;
		}
	}
}