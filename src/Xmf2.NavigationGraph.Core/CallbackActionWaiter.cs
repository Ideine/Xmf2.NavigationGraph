using System;
using System.Collections.Generic;

namespace Xmf2.NavigationGraph.Core
{
	public class CallbackActionWaiter
	{
		private List<Action> _callbackActions = new();
		private int _waiterCount;

		public void Add(Action callback)
		{
			if (_waiterCount == 0)
			{
				callback();
			}
			else
			{
				_callbackActions.Add(callback);
			}
		}

		public void WaitOne()
		{
			_waiterCount++;
		}

		public void ReleaseOne()
		{
			_waiterCount--;

			if (_waiterCount == 0)
			{
				Action[] actions = _callbackActions.ToArray();
				_callbackActions.Clear();
				_callbackActions = null;
				foreach (Action action in actions)
				{
					action();
				}
			}
		}
	}
}