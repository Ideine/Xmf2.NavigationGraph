using System;
using System.Collections.Generic;
using System.Linq;
using UIKit;
using Xmf2.NavigationGraph.iOS.Operations;

namespace Xmf2.NavigationGraph.iOS.InnerStacks
{
	public class NavigationControllerInnerStack : InnerStack
	{
		public NavigationControllerInnerStack(UINavigationController host, InnerStack container) : base(host)
		{
			Container = container;
		}

		public List<InnerStack> Stack { get; } = new List<InnerStack>();

		public InnerStack Container { get; }

		public override int Count => Stack.Sum(x => x.Count);

		public override PopOperation AsPopOperation()
		{
			if (Container != null)
			{
				Stack.Clear();
				return Container.AsSpecificPopOperation(this);
			}

			var result = new NavigationControllerPopOperation(this, Stack.Count);
			Stack.Clear();
			return result;
		}

		public override PopOperation AsSpecificPopOperation(InnerStack child)
		{
			Stack.RemoveAt(Stack.Count - 1);
			return new NavigationControllerPopOperation(this, 1);
		}

		public override PopOperation AsSpecificPopOperation(int count)
		{
			Stack.RemoveRange(Stack.Count - count, count);
			return new NavigationControllerPopOperation(this, count);
		}

		public override UIViewController AsViewController()
		{
			throw new NotSupportedException($"Unsupported type of {GetType().Name}");
		}
	}
}