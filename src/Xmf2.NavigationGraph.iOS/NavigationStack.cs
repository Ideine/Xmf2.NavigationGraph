using System;
using System.Collections.Generic;
using System.Linq;
using UIKit;
using Xmf2.NavigationGraph.Core;
using Xmf2.NavigationGraph.Core.Interfaces;
using Xmf2.NavigationGraph.iOS.Factories;
using Xmf2.NavigationGraph.iOS.InnerStacks;
using Xmf2.NavigationGraph.iOS.Operations;

namespace Xmf2.NavigationGraph.iOS
{
	public class NavigationStack<TViewModel> where TViewModel : IViewModel
	{
		private readonly List<InnerStack> _innerStacks = new List<InnerStack>();

		public void EnsureInitialized(UIWindow window)
		{
			//TODO POURRAIT ÊTRE MIEUX FAIT

			if (!_innerStacks.Any(x => x is NavigationControllerInnerStack))
			{
				_innerStacks.Add(new NavigationControllerInnerStack(window.RootViewController, null));
			}
		}

		public void ApplyActions(int popsCount, IEnumerable<PushInformation<TViewModel>> pushesCount, CallbackActionWaiter callbackActionWaiter)
		{
			var pops = Pop(popsCount);
			var pushes = Push(pushesCount);
			MergedPopPushOperation mergedOp = null;

			if (pops.Count > 0 && pushes.Count > 0 && TryMerge(pops[pops.Count - 1], pushes[0], out mergedOp))
			{
				pops.RemoveAt(pops.Count - 1);
				pushes.RemoveAt(0);
			}

			int popAnimatedIndex = mergedOp is null && pushes.Count == 0 ? pops.Count - 1 : -1;
			bool mergedAnimated = pushes.Count == 0;
			int pushAnimatedIndex = pushes.Count - 1;

			for (var index = 0 ; index < pops.Count ; index++)
			{
				pops[index].Execute(callbackActionWaiter, popAnimatedIndex == index);
			}

			mergedOp?.Execute(callbackActionWaiter, mergedAnimated);

			for (var index = 0 ; index < pushes.Count ; index++)
			{
				pushes[index].Execute(callbackActionWaiter, pushAnimatedIndex == index);
			}

			Console.WriteLine("apply actions");
		}

		private List<PopOperation> Pop(int popCount)
		{
			if (popCount == 0 || _innerStacks.Count == 0)
			{
				return new List<PopOperation>();
			}

			var popOperations = new List<PopOperation>();

			var lastInnerStackPopIndex = _innerStacks.Count;
			for (var i = _innerStacks.Count - 1 ; i >= 0 ; i--)
			{
				var item = _innerStacks[i];
				if (item.Count <= popCount)
				{
					popCount -= item.Count;
					popOperations.Add(item.AsPopOperation());
					lastInnerStackPopIndex = i;
				}
				else
				{
					break;
				}
			}

			if (lastInnerStackPopIndex == 0)
			{
				lastInnerStackPopIndex = 1;
			}

			if (lastInnerStackPopIndex < _innerStacks.Count)
			{
				_innerStacks.RemoveRange(lastInnerStackPopIndex, _innerStacks.Count - lastInnerStackPopIndex);
			}

			if (popCount > 0)
			{
				if (_innerStacks.Count == 0)
				{
					throw new InvalidOperationException("Trying to close more views than existing");
				}

				popOperations.Add(_innerStacks[_innerStacks.Count - 1].AsSpecificPopOperation(popCount));
			}

			//simplify list of pop operations
			var insertIndex = 0;
			for (var i = 1 ; i < popOperations.Count ; i++)
			{
				if (TryMerge(popOperations[insertIndex], popOperations[i], out var op))
				{
					popOperations[insertIndex] = op;
				}
				else
				{
					insertIndex++;
					if (insertIndex != i)
					{
						popOperations[insertIndex] = popOperations[i];
					}
				}
			}

			if (insertIndex != popOperations.Count - 1)
			{
				var firstIndexToRemove = insertIndex + 1;
				popOperations.RemoveRange(firstIndexToRemove, popOperations.Count - firstIndexToRemove);
			}

			return popOperations;
		}

		private List<PushOperation> Push(IEnumerable<PushInformation<TViewModel>> pushInformations)
		{
			var pushOperations = new List<PushOperation>();
			var stackTop = _innerStacks[_innerStacks.Count - 1];
			var top = stackTop;
			if (stackTop is ModalControllerInnerStack topModal)
			{
				top = topModal.Modal;
			}

			foreach (var pushInformation in pushInformations)
			{
				if (pushInformation.Controller.IsModal)
				{
					var host = top.Host;
					if (stackTop is ModalControllerInnerStack)
					{
						host = AsViewController(top);
					}

					var newTopStack = new ModalControllerInnerStack(host)
					{
						Modal = CreateFromType(pushInformation, top)
					};
					pushOperations.Add(new ModalControllerPushOperation(top, newTopStack));
					_innerStacks.Add(newTopStack);
					top = newTopStack.Modal;
				}
				else if (top is NavigationControllerInnerStack topNavigationController)
				{
					var newItem = CreateFromType(pushInformation, top);
					pushOperations.Add(new NavigationControllerPushOperation(topNavigationController)
					{
						Controllers =
						{
							newItem
						}
					});
					topNavigationController.Stack.Add(newItem);
				}
				else
				{
					throw new InvalidOperationException("Something went wrong, come with the debugger to find, too many possibilities...");
				}
			}

			//simplify list of pop operations
			var insertIndex = 0;
			for (var i = 1 ; i < pushOperations.Count ; i++)
			{
				if (TryMerge(pushOperations[insertIndex], pushOperations[i], out PushOperation op))
				{
					pushOperations[insertIndex] = op;
				}
				else
				{
					insertIndex++;
					if (insertIndex != i)
					{
						pushOperations[insertIndex] = pushOperations[i];
					}
				}
			}

			if (insertIndex < pushOperations.Count - 1)
			{
				var firstIndexToRemove = insertIndex + 1;
				pushOperations.RemoveRange(firstIndexToRemove, pushOperations.Count - firstIndexToRemove);
			}

			return pushOperations;

			InnerStack CreateFromType(PushInformation<TViewModel> info, InnerStack container)
			{
				var res = info.Controller.Factory(info.Screen.ViewModelInstance);
				if (res is UINavigationController)
				{
					return new NavigationControllerInnerStack(res, container);
				}

				return new SimpleControllerInnerStack(container.Host, res, container);
			}
		}

		private bool TryMerge(PopOperation op1, PopOperation op2, out PopOperation res)
		{
			if (op1 is NavigationControllerPopOperation nav1 && op2 is NavigationControllerPopOperation nav2)
			{
				if (nav1.HostStack == nav2.HostStack)
				{
					res = new NavigationControllerPopOperation(nav1.HostStack, nav1.CountToPop + nav2.CountToPop);
					return true;
				}
			}

			res = null;
			return false;
		}

		private bool TryMerge(PushOperation op1, PushOperation op2, out PushOperation res)
		{
			if (op1 is NavigationControllerPushOperation nav1 && op2 is NavigationControllerPushOperation nav2)
			{
				if (nav1.HostStack == nav2.HostStack)
				{
					NavigationControllerPushOperation result = new NavigationControllerPushOperation(nav1.HostStack);
					result.Controllers.AddRange(nav1.Controllers);
					result.Controllers.AddRange(nav2.Controllers);
					res = result;
					return true;
				}
			}

			res = null;
			return false;
		}

		private bool TryMerge(PopOperation popOp, PushOperation pushOp, out MergedPopPushOperation res)
		{
			if (popOp is NavigationControllerPopOperation navigationControllerPopOperation &&
			    pushOp is NavigationControllerPushOperation navigationControllerPushOperation)
			{
				if (navigationControllerPopOperation.HostStack == navigationControllerPushOperation.HostStack)
				{
					res = new MergedPopPushNavigationControllerOperation(navigationControllerPopOperation, navigationControllerPushOperation);
					return true;
				}
			}

			res = null;
			return false;
		}

		private static UIViewController AsViewController(InnerStack item)
		{
			switch (item)
			{
				case SimpleControllerInnerStack simpleControllerInnerStack:
					return simpleControllerInnerStack.Controller;
				case ModalControllerInnerStack modalControllerInnerStack:
					return AsViewController(modalControllerInnerStack.Modal);
				default:
					throw new NotSupportedException($"Unsupported type of {item.GetType().Name}");
			}
		}
	}
}