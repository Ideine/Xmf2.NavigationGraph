using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xmf2.NavigationGraph.Core.Interfaces;
using Xmf2.NavigationGraph.Core.NavigationActions;

namespace Xmf2.NavigationGraph.Core
{
	public interface INavigationService
	{
		void RegisterEntryPoint(ScreenDefinition screen);
		void Register(ScreenDefinition from, ScreenDefinition screen);

		Task Show(string route);
		Task Show(ScreenDefinition screen, string parameter = null, ViewModelCreator viewModelCreator = null);

		Task Close();
		Task Push(string route, ViewModelCreator viewModelCreator = null);
	}

	public class NavigationService : INavigationService
	{
		private readonly IPresenterService _presenterService;
		private readonly NavigationGraph _navigationGraph = new NavigationGraph();
		private readonly List<ScreenInstance> _navigationStack = new List<ScreenInstance>(10);

		private readonly object _mutex = new object();
		private NavigationInProgress _navigationInProgress;

		public NavigationService(IPresenterService presenterService)
		{
			_presenterService = presenterService;
		}

		public void RegisterEntryPoint(ScreenDefinition screen)
		{
			_navigationGraph.Add(null, screen);
		}

		public void Register(ScreenDefinition from, ScreenDefinition screen)
		{
			_navigationGraph.Add(from, screen);
		}

		public async Task Show(string route)
		{
			List<ScreenInstance> result = _navigationGraph.FindWithRoute(route).ToList();

#if DEBUG
			Console.WriteLine($"Navigating to route {route}");
			Console.WriteLine($"\tUse stack: {string.Join(", ", result.Select(x => x.ToString()))}");
#endif

			await UpdateNavigationStack(result);
		}

		public async Task Show(ScreenDefinition screen, string parameter, ViewModelCreator viewModelCreator)
		{
			ScreenInstance screenInstance = new ScreenInstance(screen, parameter, viewModelCreator);
			List<ScreenInstance> result = _navigationGraph.FindBestStack(_navigationStack, screenInstance).ToList();

#if DEBUG
			Console.WriteLine($"Navigating to {screen.RelativeRoute}");
			Console.WriteLine($"\tUse stack: {string.Join(", ", result.Select(x => x.ToString()))}");
#endif

			await UpdateNavigationStack(result);
		}

		public async Task Close()
		{
			if (_navigationStack.Count == 1)
			{
				_presenterService.CloseApp();
			}

			List<ScreenInstance> newStack = new List<ScreenInstance>(_navigationStack.Count - 1);
			for (var i = 0; i < _navigationStack.Count - 1; i++)
			{
				newStack.Add(_navigationStack[i]);
			}

#if DEBUG
			Console.WriteLine($"Navigation: Close");
			Console.WriteLine($"\tUse stack: {string.Join(", ", newStack.Select(x => x.ToString()))}");
#endif

			await UpdateNavigationStack(newStack);
		}

		public Task Push(string route, ViewModelCreator viewModelCreator)
		{
			if (route.Contains('/'))
			{
				throw new InvalidOperationException("Push can only be used to push one section of route, not multiple screen at once");
			}

			throw new NotImplementedException();
		}

		private async Task UpdateNavigationStack(List<ScreenInstance> newNavigationStack)
		{
			lock (_mutex)
			{
				if (_navigationInProgress != null && !_navigationInProgress.IsFinished)
				{
					_navigationInProgress.Cancel();

					_navigationStack.Clear();
					_navigationStack.AddRange(_navigationInProgress.StackBeforeNavigation);
				}

				_navigationInProgress = null;
			}

			_navigationInProgress = new NavigationInProgress(_navigationStack.ToArray());

			NavigationOperation navigationOperation = new NavigationOperation();
			int commonIndexLimit = 0;
			for (;
				commonIndexLimit < newNavigationStack.Count &&
				commonIndexLimit < _navigationStack.Count &&
				newNavigationStack[commonIndexLimit] == _navigationStack[commonIndexLimit];
				++commonIndexLimit) { }

			//generate pop instructions
			for (int i = _navigationStack.Count - 1; i >= commonIndexLimit; i--)
			{
				navigationOperation.Add(new PopAction(_navigationStack[i]));
			}

			_navigationStack.RemoveRange(commonIndexLimit, _navigationStack.Count - commonIndexLimit);

			//generate push instructions
			if (_navigationStack.Capacity < newNavigationStack.Count)
			{
				_navigationStack.Capacity = newNavigationStack.Count + 3; //Why 3 ? because we could use some margin and 3 is a nice small number !
			}

			for (int i = commonIndexLimit; i < newNavigationStack.Count; ++i)
			{
				navigationOperation.Add(new PushAction(newNavigationStack[i]));
				_navigationStack.Add(newNavigationStack[i]);
			}

			await _presenterService.UpdateNavigation(navigationOperation, _navigationInProgress);
		}
	}
}