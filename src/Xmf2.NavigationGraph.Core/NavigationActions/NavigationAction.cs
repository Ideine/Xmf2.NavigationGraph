using Xmf2.NavigationGraph.Core.Interfaces;

namespace Xmf2.NavigationGraph.Core.NavigationActions
{
	public abstract class NavigationAction<TViewModel> where TViewModel : IViewModel
	{
		public ScreenDefinition<TViewModel> Screen { get; }

		public ScreenInstance<TViewModel> Instance { get; }

		internal NavigationAction(ScreenInstance<TViewModel> screen)
		{
			Screen = screen.Definition;
			Instance = screen;
		}
	}
}