using Xmf2.NavigationGraph.Core.Interfaces;

namespace Xmf2.NavigationGraph.Core
{
	internal class NavigationInProgress<TViewModel> : INavigationInProgress where TViewModel : IViewModel
	{
		public NavigationInProgress(ScreenInstance<TViewModel>[] stackBeforeNavigation)
		{
			StackBeforeNavigation = stackBeforeNavigation;
		}

		public ScreenInstance<TViewModel>[] StackBeforeNavigation { get; }

		public bool IsCancelled { get; private set; }

		public bool IsFinished { get; private set; }

		public void Cancel()
		{
			IsCancelled = true;
		}

		public void Commit()
		{
			IsFinished = true;
		}
	}
}