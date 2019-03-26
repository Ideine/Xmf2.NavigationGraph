using System;

namespace Xmf2.NavigationGraph.Droid.Factories
{
	internal class ActivityViewFactory : ViewFactory
	{
		public Type ActivityType { get; }

		public bool ShouldClearHistory { get; }

		public ActivityViewFactory(Type activityType, bool shouldClearHistory)
		{
			ActivityType = activityType;
			ShouldClearHistory = shouldClearHistory;
		}
	}
}