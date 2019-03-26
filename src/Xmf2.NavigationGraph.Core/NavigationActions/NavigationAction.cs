namespace Xmf2.NavigationGraph.Core.NavigationActions
{
	public abstract class NavigationAction
	{
		public ScreenDefinition Screen { get; }

		public ScreenInstance Instance { get; }

		internal NavigationAction(ScreenInstance screen)
		{
			Screen = screen.Definition;
			Instance = screen;
		}
	}
}