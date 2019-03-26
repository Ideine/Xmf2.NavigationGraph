using Xmf2.NavigationGraph.Core.Interfaces;

namespace Xmf2.NavigationGraph.Core
{
	internal class NavigationInProgress : INavigationInProgress
	{
		public NavigationInProgress(ScreenInstance[] stackBeforeNavigation)
		{
			StackBeforeNavigation = stackBeforeNavigation;
		}

		public ScreenInstance[] StackBeforeNavigation { get; }

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