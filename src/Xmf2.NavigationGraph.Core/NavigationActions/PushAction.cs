using System;

namespace Xmf2.NavigationGraph.Core.NavigationActions
{
	public class PushAction : NavigationAction
	{
		internal PushAction(ScreenInstance screen) : base(screen)
		{
#if DEBUG
			Console.WriteLine($"\t\tPush: {screen.Definition.RelativeRoute} (parameter: {screen.Parameter})");
#endif
		}
	}
}