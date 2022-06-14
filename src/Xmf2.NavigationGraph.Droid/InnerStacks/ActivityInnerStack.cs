using System;
using System.Collections.Generic;
using System.Linq;
using Xmf2.NavigationGraph.Core.Interfaces;
using Xmf2.NavigationGraph.Droid.Operations;

namespace Xmf2.NavigationGraph.Droid.InnerStacks
{
	internal class ActivityInnerStack<TViewModel> : InnerStack<TViewModel> where TViewModel : IViewModel
	{
		public bool ActivityIsOnlyAFragmentContainer { get; }

		public Type ActivityType { get; }

		public bool ShouldClearHistory { get; }

		public List<InnerStack<TViewModel>> FragmentStack { get; } = new();

		public ActivityInnerStack(NavigationStack<TViewModel> navigationStack, Type activityType, bool activityIsOnlyAFragmentContainer, bool shouldClearHistory) : base(navigationStack)
		{
			ActivityType = activityType;
			ActivityIsOnlyAFragmentContainer = activityIsOnlyAFragmentContainer;
			ShouldClearHistory = shouldClearHistory;
		}

		public override int Count
		{
			get
			{
				if (ActivityIsOnlyAFragmentContainer)
				{
					return FragmentStack.Sum(x => x.Count);
				}

				return FragmentStack.Sum(x => x.Count) + 1;
			}
		}

		public override PopOperation AsPopOperation()
		{
			FragmentStack.Clear();
			//TODO: do we really want to finish the activity if it's the last one of the app ?
			return new ActivityPopOperation<TViewModel>(this);
		}

		public override PopOperation AsSpecificPopOperation(InnerStack<TViewModel> child)
		{
			if (child is FragmentInnerStack<TViewModel> fragmentInnerStack)
			{
				FragmentStack.RemoveAt(FragmentStack.Count - 1);
				return new FragmentPopOperation<TViewModel>(this)
				{
					FragmentStacksToPop =
					{
						fragmentInnerStack
					}
				};
			}

			throw new InvalidOperationException("Specific pop operation on unsupported child type");
		}

		public override PopOperation AsSpecificPopOperation(int count)
		{
			var result = new FragmentPopOperation<TViewModel>(this);
			for (int i = 0, index = FragmentStack.Count - 1 ; i < count ; ++i, index--)
			{
				switch (FragmentStack[index])
				{
					case DialogFragmentInnerStack<TViewModel> dialogFragmentInnerStack:
						result.FragmentStacksToPop.Add(dialogFragmentInnerStack);
						break;
					case FragmentInnerStack<TViewModel> fragmentInnerStack:
						result.FragmentStacksToPop.Add(fragmentInnerStack);
						break;
					default:
						throw new InvalidOperationException("Specific pop operation on unsupported child type");
				}
			}

			FragmentStack.RemoveRange(FragmentStack.Count - count, count);

			return result;
		}
	}
}