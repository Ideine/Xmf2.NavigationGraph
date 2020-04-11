using System;
using System.Collections.Generic;
#if __ANDROID_29__
using AndroidX.AppCompat.App;
using AndroidX.Fragment.App;
#else
using Android.App;
using Android.Support.V7.App;
using FragmentTransaction = Android.Support.V4.App.FragmentTransaction;
#endif
using Plugin.CurrentActivity;
using Xmf2.NavigationGraph.Core.Interfaces;
using Xmf2.NavigationGraph.Droid.Factories;
using Xmf2.NavigationGraph.Droid.InnerStacks;
using Xmf2.NavigationGraph.Droid.Interfaces;
using Xmf2.NavigationGraph.Droid.Operations;
using System.Linq;

namespace Xmf2.NavigationGraph.Droid
{
	internal class NavigationStack<TViewModel> where TViewModel : IViewModel
	{
		private readonly IViewModelLocatorService<TViewModel> _viewModelLocatorService;
		private readonly List<InnerStack<TViewModel>> _innerStacks = new List<InnerStack<TViewModel>>();

		public NavigationStack(IViewModelLocatorService<TViewModel> viewModelLocatorService)
		{
			_viewModelLocatorService = viewModelLocatorService;
		}

		private InnerStack<TViewModel> Top => _innerStacks[_innerStacks.Count - 1];

		private T FindFirstOfType<T>() where T : InnerStack<TViewModel>
		{
			for (int index = _innerStacks.Count - 1; index >= 0; index--)
			{
				InnerStack<TViewModel> stack = _innerStacks[index];
				if (stack is T result)
				{
					return result;
				}
			}

			return null;
		}

		public void ApplyActions(int popsCount, List<PushInformation<TViewModel>> pushesInformations)
		{
			List<PopOperation> pops = Pop(popsCount);
			List<PushOperation> pushes = Push(pushesInformations);
			MergedPopPushOperation mergedOp = null;

			if (pops.Count > 0 && pushes.Count > 0 && TryMerge(pops[pops.Count - 1], pushes[0], out mergedOp))
			{
				pops.RemoveAt(pops.Count - 1);
				pushes.RemoveAt(0);
			}

			var activity = CrossCurrentActivity.Current.Activity;
			foreach (PopOperation pop in pops)
			{
				pop.Execute(activity);
			}

			mergedOp?.Execute(activity);

			foreach (PushOperation push in pushes)
			{
				push.Execute(activity);
			}

			Console.WriteLine("apply actions");
		}

		private List<PopOperation> Pop(int popCount)
		{
			if (popCount == 0 || _innerStacks.Count == 0)
			{
				return new List<PopOperation>();
			}

			List<PopOperation> popOperations = new List<PopOperation>();

			int lastInnerStackPopIndex = _innerStacks.Count;
			for (int i = _innerStacks.Count - 1; i >= 0; i--)
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
			int insertIndex = 0;
			for (int i = 1; i < popOperations.Count; i++)
			{
				if (TryMerge(popOperations[insertIndex], popOperations[i], out PopOperation op))
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
				int firstIndexToRemove = insertIndex + 1;
				popOperations.RemoveRange(firstIndexToRemove, popOperations.Count - firstIndexToRemove);
			}

			return popOperations;
		}

		private List<PushOperation> Push(List<PushInformation<TViewModel>> pushInformations)
		{
			List<PushOperation> pushOperations = new List<PushOperation>();
			InnerStack<TViewModel> top = null;
			if (_innerStacks.Count > 0)
			{
				top = _innerStacks[_innerStacks.Count - 1];
			}

			foreach (var pushInformation in pushInformations)
			{
				if (pushInformation.Factory is ActivityViewFactory activityViewFactory)
				{
					var activityInnerStack = new ActivityInnerStack<TViewModel>(this, activityViewFactory.ActivityType, activityIsOnlyAFragmentContainer: false, shouldClearHistory: activityViewFactory.ShouldClearHistory);
					pushOperations.Add(new ActivityPushOperation<TViewModel>(activityInnerStack, pushInformation.Instance.ViewModelInstance));
					top = activityInnerStack;
					_innerStacks.Add(top);
				}
				else if (pushInformation.Factory is DialogFragmentViewFactory dialogFragmentViewFactory)
				{
					var host = top.GetActivityInnerStack();
					if (host == null)
					{
						//need to push the activity first
						var activityInnerStack = new ActivityInnerStack<TViewModel>(this, dialogFragmentViewFactory.HostActivityType, activityIsOnlyAFragmentContainer: true, shouldClearHistory: dialogFragmentViewFactory.ShouldClearHistory);
						pushOperations.Add(new ActivityPushOperation<TViewModel>(activityInnerStack, viewModel: null));
						top = host = activityInnerStack;
						_innerStacks.Add(top);
					}

					var fragment = dialogFragmentViewFactory.Creator();

					//we set screeninformation to the fragment can retrieve its viewmodel
					if (fragment is IScreenView screenView)
					{
						screenView.ScreenRoute = pushInformation.Instance.ToString();
						_viewModelLocatorService.AddViewModel(pushInformation.Instance.ToString(), pushInformation.Instance.ViewModelInstance);
					}

					var dialogFragmentInnerStack = new DialogFragmentInnerStack<TViewModel>(this, host, fragment);
					pushOperations.Add(new FragmentPushOperation<TViewModel>(host)
					{
						FragmentStacksToPush =
						{
							dialogFragmentInnerStack
						}
					});
					top = dialogFragmentInnerStack;
					_innerStacks.Add(top);
				}
				else if (pushInformation.Factory is FragmentViewFactory fragmentViewFactory)
				{
					var activityHost = top.GetActivityInnerStack();
					if (activityHost == null)
					{
						//need to push the activity first
						var activityInnerStack = new ActivityInnerStack<TViewModel>(this, fragmentViewFactory.HostActivityType, activityIsOnlyAFragmentContainer: true, shouldClearHistory: fragmentViewFactory.ShouldClearHistory);
						pushOperations.Add(new ActivityPushOperation<TViewModel>(activityInnerStack, viewModel: null));
						top = activityHost = activityInnerStack;
						_innerStacks.Add(top);
					}

					var fragment = fragmentViewFactory.Creator();

					//we set screeninformation to the fragment can retrieve its viewmodel
					if (fragment is IScreenView screenView)
					{
						screenView.ScreenRoute = pushInformation.Instance.ToString();
						_viewModelLocatorService.AddViewModel(pushInformation.Instance.ToString(), pushInformation.Instance.ViewModelInstance);
					}

					var fragmentInnerStack = new FragmentInnerStack<TViewModel>(this, activityHost, fragment);
					pushOperations.Add(new FragmentPushOperation<TViewModel>(activityHost)
					{
						FragmentStacksToPush =
						{
							fragmentInnerStack
						}
					});
					activityHost.FragmentStack.Add(fragmentInnerStack);
				}
				else
				{
					throw new InvalidOperationException("This kind of factory is not supported...");
				}
			}

			//simplify list of pop operations
			int insertIndex = 0;
			for (int i = 1; i < pushOperations.Count; i++)
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
				int firstIndexToRemove = insertIndex + 1;
				pushOperations.RemoveRange(firstIndexToRemove, pushOperations.Count - firstIndexToRemove);
			}

			return pushOperations;
		}

		private bool TryMerge(PopOperation op1, PopOperation op2, out PopOperation res)
		{
			if (op1 is FragmentPopOperation<TViewModel> fragmentPopOperation)
			{
				if (op2 is FragmentPopOperation<TViewModel> fragmentPopOperation2)
				{
					if (fragmentPopOperation.HostStack == fragmentPopOperation2.HostStack)
					{
						fragmentPopOperation.FragmentStacksToPop.AddRange(fragmentPopOperation2.FragmentStacksToPop);
						res = fragmentPopOperation;
						return true;
					}
				}
				else if (op2 is ActivityPopOperation<TViewModel> finishActivityPopOperation)
				{
					if (fragmentPopOperation.HostStack == finishActivityPopOperation.ActivityStack)
					{
						// avoid popping fragment if activity will be stopped anyway
						res = finishActivityPopOperation;
						return true;
					}
				}
			}

			res = null;
			return false;
		}

		private bool TryMerge(PushOperation op1, PushOperation op2, out PushOperation res)
		{
			if (op1 is ActivityPushOperation<TViewModel> startActivityPushOperation)
			{
				if (op2 is FragmentPushOperation<TViewModel> showFragmentPushOperation)
				{
					if (startActivityPushOperation.ActivityStack == showFragmentPushOperation.HostStack)
					{
						//merge show fragment in start activity
						startActivityPushOperation.FragmentStacksToPush.AddRange(showFragmentPushOperation.FragmentStacksToPush);
						res = startActivityPushOperation;
						return true;
					}
				}
			}
			else if (op1 is FragmentPushOperation<TViewModel> showFragmentPushOperation1 && op2 is FragmentPushOperation<TViewModel> showFragmentPushOperation2)
			{
				if (showFragmentPushOperation1.HostStack == showFragmentPushOperation2.HostStack)
				{
					showFragmentPushOperation1.FragmentStacksToPush.AddRange(showFragmentPushOperation2.FragmentStacksToPush);
					res = showFragmentPushOperation1;
					return true;
				}
			}

			res = null;
			return false;
		}

		private bool TryMerge(PopOperation popOp, PushOperation pushOp, out MergedPopPushOperation res)
		{
			if (popOp is FragmentPopOperation<TViewModel> fragmentPopOperation && pushOp is FragmentPushOperation<TViewModel> fragmentPushOperation)
			{
				if (fragmentPopOperation.HostStack == fragmentPushOperation.HostStack)
				{
					res = new MergedFragmentPopPushOperation<TViewModel>(fragmentPopOperation.HostStack, fragmentPopOperation.FragmentStacksToPop, fragmentPushOperation.FragmentStacksToPush);
					return true;
				}
			}

			if (popOp is ActivityPopOperation<TViewModel> activityPopOperation && pushOp is ActivityPushOperation<TViewModel> activityPushOperation)
			{
				res = new MergedActivityPopPushOperation<TViewModel>(activityPopOperation, activityPushOperation);
				return true;
			}

			res = null;
			return false;
		}

		internal static void UpdateFragments(NavigationStack<TViewModel> navigationStack, AppCompatActivity appCompatActivity, List<IFragmentInnerStack> fragmentsToPop, List<IFragmentInnerStack> fragmentsToPush, IFragmentActivity fragmentActivity)
		{
			FragmentTransaction transaction = null;

			var dialogFragmentToPush = fragmentsToPush?.Where(x => x is DialogFragmentInnerStack<TViewModel>).Cast<DialogFragmentInnerStack<TViewModel>>().ToList();
			var normalFragmentToPush = fragmentsToPush?.Where(x => x is FragmentInnerStack<TViewModel>).ToList();
			List<FragmentInnerStack<TViewModel>> fragmentListToPop = null;

			if (fragmentsToPop != null)
			{
				foreach (IFragmentInnerStack fragmentStack in fragmentsToPop)
				{
					if (fragmentStack is DialogFragmentInnerStack<TViewModel> dialogFragmentInnerStack)
					{
						dialogFragmentInnerStack.Fragment.DismissAllowingStateLoss();
					}
					else if (fragmentStack is FragmentInnerStack<TViewModel> fragmentInnerStack)
					{
						if (fragmentListToPop is null)
						{
							fragmentListToPop = new List<FragmentInnerStack<TViewModel>>(fragmentsToPop.Count);
						}

						fragmentListToPop.Add(fragmentInnerStack);
					}
				}

				if (fragmentListToPop != null)
				{
					transaction = appCompatActivity.SupportFragmentManager.BeginTransaction();
					foreach (FragmentInnerStack<TViewModel> fragmentStack in fragmentListToPop)
					{
						transaction = transaction.Remove(fragmentStack.Fragment);
					}
				}
			}

			if (normalFragmentToPush != null && normalFragmentToPush.Count > 0 && fragmentActivity != null)
			{
				foreach (IFragmentInnerStack fragmentStack in normalFragmentToPush)
				{
					if (transaction is null)
					{
						transaction = appCompatActivity.SupportFragmentManager.BeginTransaction();
					}

					transaction.AddToBackStack(fragmentStack.FragmentTag);
					transaction = transaction.Replace(fragmentActivity.FragmentContainerId, fragmentStack.Fragment, fragmentStack.FragmentTag);
				}
			}
			else if (transaction != null)
			{
				ActivityInnerStack<TViewModel> activityTop;
				if (dialogFragmentToPush != null && dialogFragmentToPush.Count > 0)
				{
					activityTop = navigationStack.FindFirstOfType<ActivityInnerStack<TViewModel>>();
				}
				else
				{
					activityTop = navigationStack.Top as ActivityInnerStack<TViewModel>;
				}

				if (activityTop != null && activityTop.FragmentStack.Count > 0 && activityTop.FragmentStack[activityTop.FragmentStack.Count - 1] is FragmentInnerStack<TViewModel> fragmentInnerStack)
				{
					transaction = transaction.Replace(fragmentActivity.FragmentContainerId, fragmentInnerStack.Fragment, fragmentInnerStack.FragmentTag);
				}
			}

			transaction?.CommitAllowingStateLoss();

			if (dialogFragmentToPush != null && dialogFragmentToPush.Count > 0)
			{
				foreach (DialogFragmentInnerStack<TViewModel> dialogFragment in dialogFragmentToPush)
				{

					dialogFragment.Fragment.Show(appCompatActivity.SupportFragmentManager, dialogFragment.FragmentTag);
				}
			}
		}
	}
}