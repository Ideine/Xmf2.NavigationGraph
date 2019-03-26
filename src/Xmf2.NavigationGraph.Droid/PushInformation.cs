using Xmf2.NavigationGraph.Core;
using Xmf2.NavigationGraph.Droid.Factories;

namespace Xmf2.NavigationGraph.Droid
{
	internal class PushInformation
	{
		public ViewFactory Factory { get; }

		public ScreenInstance Instance { get; }

		public PushInformation(ViewFactory factory, ScreenInstance instance)
		{
			Factory = factory;
			Instance = instance;
		}
	}
}