using Xmf2.NavigationGraph.Core;
using Xmf2.NavigationGraph.Core.Interfaces;

namespace Xmf2.NavigationGraph.iOS.Factories
{
	public class PushInformation<TViewModel> where TViewModel : IViewModel
	{
		public ControllerInformation<TViewModel> Controller { get; }

		public ScreenInstance<TViewModel> Screen { get; }

		public PushInformation(ControllerInformation<TViewModel> controller, ScreenInstance<TViewModel> screen)
		{
			Controller = controller;
			Screen = screen;
		}
	}
}