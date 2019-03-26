using Xmf2.NavigationGraph.Core;
using Xmf2.NavigationGraph.Core.Interfaces;
using Xmf2.NavigationGraph.Droid.Factories;

namespace Xmf2.NavigationGraph.Droid
{
	internal class PushInformation<TViewModel> where TViewModel : IViewModel
	{
		public ViewFactory Factory { get; }

		public ScreenInstance<TViewModel> Instance { get; }

		public PushInformation(ViewFactory factory, ScreenInstance<TViewModel> instance)
		{
			Factory = factory;
			Instance = instance;
		}
	}
}