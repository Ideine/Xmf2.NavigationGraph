using System.Linq;
using System.Threading;
using Xmf2.NavigationGraph.Core.Interfaces;

namespace Xmf2.NavigationGraph.Core
{
	internal class NavigationInProgress<TViewModel> : INavigationInProgress where TViewModel : IViewModel
	{
		private const int RUNNING = 0;
		private const int COMMITTED = 1;
		private const int CANCELLED = 2;

		private int _state = RUNNING;

		public NavigationInProgress(ScreenInstance<TViewModel>[] stackBeforeNavigation, NavigationOperation<TViewModel> operation)
		{
			StackBeforeNavigation = stackBeforeNavigation;
			Operation = operation;
		}

		public ScreenInstance<TViewModel>[] StackBeforeNavigation { get; }

		public NavigationOperation<TViewModel> Operation { get; }

		public bool IsCancelled => Volatile.Read(ref _state) == CANCELLED;

		public bool TryCancel()
		{
			return Interlocked.CompareExchange(ref _state, CANCELLED, RUNNING) == RUNNING;
		}

		public bool TryCommit()
		{
			return Interlocked.CompareExchange(ref _state, COMMITTED, RUNNING) == RUNNING;
		}

		public void Commit()
		{
			TryCommit();
		}

		public bool HasPushed(ScreenInstance<TViewModel> screen)
		{
			return Operation.Pushes.Any(x => ReferenceEquals(x.Instance, screen));
		}
	}
}
