using System;
using System.Collections.Generic;
using System.Linq;
using Xmf2.NavigationGraph.Droid.Operations;

namespace Xmf2.NavigationGraph.Droid.InnerStacks
{
	internal class ActivityInnerStack : InnerStack
	{
		public bool ActivityIsOnlyAFragmentContainer { get; }

		public Type ActivityType { get; }

		public bool ShouldClearHistory { get; }

		public List<InnerStack> FragmentStack { get; } = new List<InnerStack>();

		public ActivityInnerStack(NavigationStack navigationStack, Type activityType, bool activityIsOnlyAFragmentContainer, bool shouldClearHistory) : base(navigationStack)
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
			return new ActivityPopOperation(this);
		}

		public override PopOperation AsSpecificPopOperation(InnerStack child)
		{
			if (child is fragmentInnerStack fragmentInnerStack)
			{
				FragmentStack.RemoveAt(FragmentStack.Count - 1);
				return new FragmentPopOperation(this)
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
			FragmentPopOperation result = new FragmentPopOperation(this);
			for (int i = 0, index = FragmentStack.Count - 1; i < count; ++i, index--)
			{
				if (FragmentStack[index] is DialogFragmentInnerStack dialogFragmentInnerStack)
				{
					result.FragmentStacksToPop.Add(dialogFragmentInnerStack);
				}
				else if (FragmentStack[index] is fragmentInnerStack fragmentInnerStack)
				{
					result.FragmentStacksToPop.Add(fragmentInnerStack);
				}
				else
				{
					throw new InvalidOperationException("Specific pop operation on unsupported child type");
				}
			}

			FragmentStack.RemoveRange(FragmentStack.Count - count, count);

			return result;
		}
	}
}