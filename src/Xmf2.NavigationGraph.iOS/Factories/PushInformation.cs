using Xmf2.NavigationGraph.Core;

namespace Xmf2.NavigationGraph.iOS.Factories
{
	public class PushInformation
	{
		public ControllerInformation Controller { get; }

		public ScreenInstance Screen { get; }

		public PushInformation(ControllerInformation controller, ScreenInstance screen)
		{
			Controller = controller;
			Screen = screen;
		}
	}
}