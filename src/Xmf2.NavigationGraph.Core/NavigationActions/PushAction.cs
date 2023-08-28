using System.Diagnostics;
using Xmf2.NavigationGraph.Core.Interfaces;

namespace Xmf2.NavigationGraph.Core.NavigationActions
{
	public class PushAction<TViewModel> : NavigationAction<TViewModel> where TViewModel : IViewModel
	{
		internal PushAction(ScreenInstance<TViewModel> screen) : base(screen)
		{
			Debug.WriteLine($"\t\tPush: {screen.Definition.RelativeRoute} (parameter: {screen.Parameter})");
		}
	}
}