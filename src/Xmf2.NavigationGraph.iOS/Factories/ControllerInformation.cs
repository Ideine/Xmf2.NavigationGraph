namespace Xmf2.NavigationGraph.iOS.Factories
{
	public class ControllerInformation
	{
		public ViewCreator Factory { get; }

		public bool IsModal { get; }

		public ControllerInformation(ViewCreator factory, bool isModal)
		{
			Factory = factory;
			IsModal = isModal;
		}
	}
}