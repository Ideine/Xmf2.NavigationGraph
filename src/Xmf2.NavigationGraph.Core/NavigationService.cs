using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Xmf2.NavigationGraph.Core.Interfaces;
using Xmf2.NavigationGraph.Core.NavigationActions;

namespace Xmf2.NavigationGraph.Core
{
	public interface INavigationService<TViewModel> where TViewModel : IViewModel
	{
		void RegisterEntryPoint(ScreenDefinition<TViewModel> screen);
		void Register(ScreenDefinition<TViewModel> from, ScreenDefinition<TViewModel> screen);

		Task Show(string route);
		Task Show(ScreenDefinition<TViewModel> screen, string parameter = null, ViewModelCreator<TViewModel> viewModelCreator = null);

		Task Close();
		Task Push(string route, ViewModelCreator<TViewModel> viewModelCreator = null);
	}

	public class NavigationService<TViewModel> : INavigationService<TViewModel> where TViewModel : IViewModel
	{
		private readonly IPresenterService<TViewModel> _presenterService;
		private readonly NavigationGraph<TViewModel> _navigationGraph = new();
		private readonly List<ScreenInstance<TViewModel>> _navigationStack = new(10);

		private readonly object _mutex = new();
		private NavigationInProgress<TViewModel> _navigationInProgress;
		private Task _runningNavigation;

		public NavigationService(IPresenterService<TViewModel> presenterService)
		{
			_presenterService = presenterService;
		}

		public void RegisterEntryPoint(ScreenDefinition<TViewModel> screen)
		{
			_navigationGraph.Add(null, screen);
		}

		public void Register(ScreenDefinition<TViewModel> from, ScreenDefinition<TViewModel> screen)
		{
			_navigationGraph.Add(from, screen);
		}

		public async Task Show(string route)
		{
			Debug.WriteLine($"Navigating to route {route}");

			await UpdateNavigationStack(_ => _navigationGraph.FindWithRoute(route).ToList());
		}

		public async Task Show(ScreenDefinition<TViewModel> screen, string parameter = null, ViewModelCreator<TViewModel> viewModelCreator = null)
		{
			ScreenInstance<TViewModel> screenInstance = new(screen, parameter, viewModelCreator);

			Debug.WriteLine($"Navigating to {screen.RelativeRoute}");

			await UpdateNavigationStack(currentStack => _navigationGraph.FindBestStack(currentStack, screenInstance).ToList());
		}

		public async Task Close()
		{
			Debug.WriteLine($"Navigation: Close");

			await UpdateNavigationStack(currentStack =>
			{
				if (currentStack.Count == 1)
				{
					_presenterService.CloseApp();
				}

				List<ScreenInstance<TViewModel>> newStack = new(currentStack.Count - 1);
				for (int i = 0 ; i < currentStack.Count - 1 ; i++)
				{
					newStack.Add(currentStack[i]);
				}

				return newStack;
			});
		}

		public Task Push(string route, ViewModelCreator<TViewModel> viewModelCreator)
		{
			if (route.Contains('/'))
			{
				throw new InvalidOperationException("Push can only be used to push one section of route, not multiple screen at once");
			}

			throw new NotImplementedException();
		}

		private Task UpdateNavigationStack(Func<List<ScreenInstance<TViewModel>>, List<ScreenInstance<TViewModel>>> buildNewNavigationStack)
		{
			TaskCompletionSource<bool> start = new();
			Task navigation;

			lock (_mutex)
			{
				List<ScreenInstance<TViewModel>> newNavigationStack = buildNewNavigationStack(_navigationStack);
				Debug.WriteLine($"\tUse stack: {string.Join(", ", newNavigationStack.Select(x => x.ToString()))}");

				//the presenter runs outside of the lock: while it creates the view models, _navigationStack already holds the stack it will display.
				//The same request (double tap) would otherwise cancel the running navigation and create its view models a second time
				if (_runningNavigation is { IsCompleted: false } && newNavigationStack.SequenceEqual(_navigationStack))
				{
					return _runningNavigation;
				}

				NavigationInProgress<TViewModel> previousNavigation = _navigationInProgress;
				if (previousNavigation != null && previousNavigation.TryCancel())
				{
					//the presenter disposes the view models of a cancelled navigation, its screens must not be reused
					newNavigationStack = newNavigationStack.ConvertAll(x => previousNavigation.HasPushed(x) ? new ScreenInstance<TViewModel>(x.Definition, x.Parameter, x.ViewModelCreator) : x);

					_navigationStack.Clear();
					_navigationStack.AddRange(previousNavigation.StackBeforeNavigation);
				}

				ScreenInstance<TViewModel>[] stackBeforeNavigation = _navigationStack.ToArray();
				NavigationOperation<TViewModel> navigationOperation = new();

				int commonIndexLimit = 0;
				for (;
				     commonIndexLimit < newNavigationStack.Count &&
				     commonIndexLimit < _navigationStack.Count &&
				     newNavigationStack[commonIndexLimit] == _navigationStack[commonIndexLimit] ;
				     ++commonIndexLimit) { }

				//generate pop instructions
				for (int i = _navigationStack.Count - 1 ; i >= commonIndexLimit ; i--)
				{
					navigationOperation.Add(new PopAction<TViewModel>(_navigationStack[i]));
				}

				_navigationStack.RemoveRange(commonIndexLimit, _navigationStack.Count - commonIndexLimit);

				//generate push instructions
				if (_navigationStack.Capacity < newNavigationStack.Count)
				{
					_navigationStack.Capacity = newNavigationStack.Count + 3; //Why 3 ? because we could use some margin and 3 is a nice small number !
				}

				for (int i = commonIndexLimit ; i < newNavigationStack.Count ; ++i)
				{
					navigationOperation.Add(new PushAction<TViewModel>(newNavigationStack[i]));
					_navigationStack.Add(newNavigationStack[i]);
				}

				NavigationInProgress<TViewModel> navigationInProgress = _navigationInProgress = new(stackBeforeNavigation, navigationOperation);
				navigation = _runningNavigation = ApplyNavigation(navigationOperation, navigationInProgress, start.Task);
			}

			start.SetResult(true);
			return navigation;
		}

		private async Task ApplyNavigation(NavigationOperation<TViewModel> navigationOperation, NavigationInProgress<TViewModel> navigationInProgress, Task start)
		{
			await start;
			await _presenterService.UpdateNavigation(navigationOperation, navigationInProgress);
		}
	}
}