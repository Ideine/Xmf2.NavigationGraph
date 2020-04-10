using System;
using Xmf2.NavigationGraph.Core.Interfaces;

namespace Xmf2.NavigationGraph.Core.NavigationActions
{
	public class PopAction<TViewModel> : NavigationAction<TViewModel> where TViewModel : IViewModel
	{
		internal PopAction(ScreenInstance<TViewModel> screen) : base(screen)
		{
			System.Diagnostics.Debug.WriteLine($"\t\tPop: {screen.Definition.RelativeRoute} (parameter: {screen.Parameter})");
		}
	}
}