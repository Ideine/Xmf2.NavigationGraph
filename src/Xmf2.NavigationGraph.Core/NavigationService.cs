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
			List<ScreenInstance<TViewModel>> result = _navigationGraph.FindWithRoute(route).ToList();

			Debug.WriteLine($"Navigating to route {route}");
			Debug.WriteLine($"\tUse stack: {string.Join(", ", result.Select(x => x.ToString()))}");

			await UpdateNavigationStack(result);
		}

		public async Task Show(ScreenDefinition<TViewModel> screen, string parameter = null, ViewModelCreator<TViewModel> viewModelCreator = null)
		{
			ScreenInstance<TViewModel> screenInstance = new(screen, parameter, viewModelCreator);
			List<ScreenInstance<TViewModel>> result = _navigationGraph.FindBestStack(_navigationStack, screenInstance).ToList();

			Debug.WriteLine($"Navigating to {screen.RelativeRoute}");
			Debug.WriteLine($"\tUse stack: {string.Join(", ", result.Select(x => x.ToString()))}");

			await UpdateNavigationStack(result);
		}

		public async Task Close()
		{
			if (_navigationStack.Count == 1)
			{
				_presenterService.CloseApp();
			}

			List<ScreenInstance<TViewModel>> newStack = new(_navigationStack.Count - 1);
			for (int i = 0 ; i < _navigationStack.Count - 1 ; i++)
			{
				newStack.Add(_navigationStack[i]);
			}

			Debug.WriteLine($"Navigation: Close");
			Debug.WriteLine($"\tUse stack: {string.Join(", ", newStack.Select(x => x.ToString()))}");

			await UpdateNavigationStack(newStack);
		}

		public Task Push(string route, ViewModelCreator<TViewModel> viewModelCreator)
		{
			if (route.Contains('/'))
			{
				throw new InvalidOperationException("Push can only be used to push one section of route, not multiple screen at once");
			}

			throw new NotImplementedException();
		}

		private async Task UpdateNavigationStack(List<ScreenInstance<TViewModel>> newNavigationStack)
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

			_navigationInProgress = new(_navigationStack.ToArray());

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

			await _presenterService.UpdateNavigation(navigationOperation, _navigationInProgress);
		}
	}
}