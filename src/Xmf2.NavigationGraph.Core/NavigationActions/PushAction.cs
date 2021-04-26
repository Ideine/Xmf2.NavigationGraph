using System;
using Xmf2.NavigationGraph.Core.Interfaces;

namespace Xmf2.NavigationGraph.Core.NavigationActions
{
	public class PushAction<TViewModel> : NavigationAction<TViewModel> where TViewModel : IViewModel
	{
		public string BaseRoute { get; }

		internal PushAction(ScreenInstance<TViewModel> screen, string baseRoute) : base(screen)
		{
			BaseRoute = baseRoute;
			System.Diagnostics.Debug.WriteLine($"\t\tPush: {screen.Definition.RelativeRoute} (parameter: {screen.Parameter})");
		}
	}
}