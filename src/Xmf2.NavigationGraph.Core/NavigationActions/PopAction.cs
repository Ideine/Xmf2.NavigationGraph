using System;

namespace Xmf2.NavigationGraph.Core.NavigationActions
{
	public class PopAction : NavigationAction
	{
		internal PopAction(ScreenInstance screen) : base(screen)
		{
#if DEBUG
			Console.WriteLine($"\t\tPop: {screen.Definition.RelativeRoute} (parameter: {screen.Parameter})");
#endif
		}
	}
}