using System;
using System.Collections.Generic;
using Android.App;
using Android.Support.V7.App;
using Xmf2.NavigationGraph.Core.Interfaces;
using Xmf2.NavigationGraph.Droid.Factories;
using Xmf2.NavigationGraph.Droid.InnerStacks;
using Xmf2.NavigationGraph.Droid.Interfaces;
using Xmf2.NavigationGraph.Droid.Operations;
using FragmentTransaction = Android.Support.V4.App.FragmentTransaction;

namespace Xmf2.NavigationGraph.Droid
{
	internal class NavigationStack
	{
		private readonly IViewModelLocatorService _viewModelLocatorService;
		private readonly List<InnerStack> _innerStacks = new List<InnerStack>();

		public NavigationStack(IViewModelLocatorService viewModelLocatorService)
		{
			_viewModelLocatorService = viewModelLocatorService;
		}

		private InnerStack Top => _innerStacks[_innerStacks.Count - 1];

		public void ApplyActions(int popsCount, List<PushInformation> pushesInformations)
		{
			List<PopOperation> pops = Pop(popsCount);
			List<PushOperation> pushes = Push(pushesInformations);
			MergedPopPushOperation mergedOp = null;

			if (pops.Count > 0 && pushes.Count > 0 && TryMerge(pops[pops.Count - 1], pushes[0], out mergedOp))
			{
				pops.RemoveAt(pops.Count - 1);
				pushes.RemoveAt(0);
			}

			Activity activity = NavigationPresenter.CurrentActivity;
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
				InnerStack item = _innerStacks[i];
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

		private List<PushOperation> Push(List<PushInformation> pushInformations)
		{
			List<PushOperation> pushOperations = new List<PushOperation>();
			InnerStack top = null;
			if (_innerStacks.Count > 0)
			{
				top = _innerStacks[_innerStacks.Count - 1];
			}

			foreach (PushInformation pushInformation in pushInformations)
			{
				if (pushInformation.Factory is ActivityViewFactory activityViewFactory)
				{
					var activityInnerStack = new ActivityInnerStack(this, activityViewFactory.ActivityType, activityIsOnlyAFragmentContainer: false, shouldClearHistory: activityViewFactory.ShouldClearHistory);
					pushOperations.Add(new ActivityPushOperation(activityInnerStack, pushInformation.Instance.ViewModelInstance));
					top = activityInnerStack;
					_innerStacks.Add(top);
				}
				else if (pushInformation.Factory is DialogFragmentViewFactory dialogFragmentViewFactory)
				{
					ActivityInnerStack host = top.GetActivityInnerStack();
					if (host == null)
					{
						//need to push the activity first
						var activityInnerStack = new ActivityInnerStack(this, dialogFragmentViewFactory.HostActivityType, activityIsOnlyAFragmentContainer: true, shouldClearHistory: dialogFragmentViewFactory.ShouldClearHistory);
						pushOperations.Add(new ActivityPushOperation(activityInnerStack, viewModel: null));
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

					var dialogFragmentInnerStack = new DialogFragmentInnerStack(this, host, fragment);
					pushOperations.Add(new FragmentPushOperation(host)
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
						var activityInnerStack = new ActivityInnerStack(this, fragmentViewFactory.HostActivityType, activityIsOnlyAFragmentContainer: true, shouldClearHistory: fragmentViewFactory.ShouldClearHistory);
						pushOperations.Add(new ActivityPushOperation(activityInnerStack, viewModel: null));
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

					var fragmentInnerStack = new fragmentInnerStack(this, activityHost, fragment);
					pushOperations.Add(new FragmentPushOperation(activityHost)
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
			if (op1 is FragmentPopOperation fragmentPopOperation)
			{
				if (op2 is FragmentPopOperation fragmentPopOperation2)
				{
					if (fragmentPopOperation.HostStack == fragmentPopOperation2.HostStack)
					{
						fragmentPopOperation.FragmentStacksToPop.AddRange(fragmentPopOperation2.FragmentStacksToPop);
						res = fragmentPopOperation;
						return true;
					}
				}
				else if (op2 is ActivityPopOperation finishActivityPopOperation)
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
			if (op1 is ActivityPushOperation startActivityPushOperation)
			{
				if (op2 is FragmentPushOperation showFragmentPushOperation)
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
			else if (op1 is FragmentPushOperation showFragmentPushOperation1 && op2 is FragmentPushOperation showFragmentPushOperation2)
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
			if (popOp is FragmentPopOperation fragmentPopOperation && pushOp is FragmentPushOperation fragmentPushOperation)
			{
				if (fragmentPopOperation.HostStack == fragmentPushOperation.HostStack)
				{
					res = new MergedFragmentPopPushOperation(fragmentPopOperation.HostStack, fragmentPopOperation.FragmentStacksToPop, fragmentPushOperation.FragmentStacksToPush);
					return true;
				}
			}

			if (popOp is ActivityPopOperation activityPopOperation && pushOp is ActivityPushOperation activityPushOperation)
			{
				res = new MergedActivityPopPushOperation(activityPopOperation, activityPushOperation);
				return true;
			}

			res = null;
			return false;
		}

		internal static void UpdateFragments(NavigationStack navigationStack, AppCompatActivity appCompatActivity, List<IFragmentInnerStack> fragmentsToPop, List<IFragmentInnerStack> fragmentsToPush, IFragmentActivity fragmentActivity)
		{
			FragmentTransaction transaction = null;

			if (fragmentsToPop != null)
			{
				List<fragmentInnerStack> fragmentListToPop = null;
				foreach (var fragmentStack in fragmentsToPop)
				{
					if (fragmentStack is DialogFragmentInnerStack dialogFragmentInnerStack)
					{
						dialogFragmentInnerStack.Fragment.DismissAllowingStateLoss();
					}
					else if (fragmentStack is fragmentInnerStack fragmentInnerStack)
					{
						if (fragmentListToPop is null)
						{
							fragmentListToPop = new List<fragmentInnerStack>(fragmentsToPop.Count);
						}

						fragmentListToPop.Add(fragmentInnerStack);
					}
				}

				if (fragmentListToPop != null)
				{
					transaction = appCompatActivity.SupportFragmentManager.BeginTransaction();
					foreach (fragmentInnerStack fragmentStack in fragmentListToPop)
					{
						transaction = transaction.Remove(fragmentStack.Fragment);
					}
				}
			}

			if (fragmentsToPush != null)
			{
				List<DialogFragmentInnerStack> dialogFragmentsToPush = null;
				foreach (var fragmentStack in fragmentsToPush)
				{
					switch (fragmentStack)
					{
						case DialogFragmentInnerStack dialogFragmentInnerStack:
						{
							if (dialogFragmentsToPush is null)
							{
								dialogFragmentsToPush = new List<DialogFragmentInnerStack>(fragmentsToPush.Count);
							}

							dialogFragmentsToPush.Add(dialogFragmentInnerStack);
							break;
						}
						case fragmentInnerStack fragmentInnerStack when fragmentActivity != null:
						{
							if (transaction is null)
							{
								transaction = appCompatActivity.SupportFragmentManager.BeginTransaction();
							}

							transaction.AddToBackStack(fragmentStack.FragmentTag);
							transaction = transaction.Replace(fragmentActivity.FragmentContainerId, fragmentStack.Fragment, fragmentStack.FragmentTag);
							break;
						}
					}
				}

				transaction?.CommitAllowingStateLoss();

				if (dialogFragmentsToPush != null)
				{
					foreach (var dialogFragment in dialogFragmentsToPush)
					{
						dialogFragment.Fragment.Show(appCompatActivity.SupportFragmentManager, dialogFragment.FragmentTag);
					}
				}
			}

			else
			{
				if (transaction != null && navigationStack.Top is ActivityInnerStack activityTop && activityTop.FragmentStack.Count > 0)
				{
					if (activityTop.FragmentStack[activityTop.FragmentStack.Count - 1] is fragmentInnerStack fragmentInnerStack)
					{
						transaction = transaction.Replace(fragmentActivity.FragmentContainerId, fragmentInnerStack.Fragment, fragmentInnerStack.FragmentTag);
					}
				}

				transaction?.CommitAllowingStateLoss();
			}
		}
	}
}