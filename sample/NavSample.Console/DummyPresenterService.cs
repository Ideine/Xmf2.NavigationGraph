using System;
using System.Threading.Tasks;
using NavSample.Core;
using Xmf2.NavigationGraph.Core;
using Xmf2.NavigationGraph.Core.Interfaces;

namespace NavSample.ConsoleApp
{
	public class DummyPresenterService : IPresenterService<SampleViewModel>
	{
		public void CloseApp()
		{
			throw new NotImplementedException();
		}

		public Task UpdateNavigation(NavigationOperation<SampleViewModel> navigationOperation, INavigationInProgress navigationInProgress)
		{
			Console.WriteLine($"\tOperations to apply: Pops={navigationOperation.Pops.Count}, Pushes={navigationOperation.Pushes.Count}");
			Console.WriteLine("");

			navigationInProgress.Commit();
			return Task.CompletedTask;
		}
	}
}