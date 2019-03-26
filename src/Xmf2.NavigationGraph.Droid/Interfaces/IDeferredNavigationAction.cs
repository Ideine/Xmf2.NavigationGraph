using Android.App;
using Xmf2.NavigationGraph.Droid.Operations;

namespace Xmf2.NavigationGraph.Droid.Interfaces
{
	public interface IDeferredNavigationAction
	{
		void Execute(Activity activity);
	}

	internal class DeferredNavigationAction : IDeferredNavigationAction
	{
		private readonly Operation _deferredOperation;

		public DeferredNavigationAction(Operation deferredOperation)
		{
			_deferredOperation = deferredOperation;
		}

		public void Execute(Activity activity)
		{
			_deferredOperation.Execute(activity);
		}
	}
}