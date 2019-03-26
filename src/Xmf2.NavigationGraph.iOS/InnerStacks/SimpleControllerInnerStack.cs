using System;
using UIKit;
using Xmf2.NavigationGraph.iOS.Operations;

namespace Xmf2.NavigationGraph.iOS.InnerStacks
{
	public class SimpleControllerInnerStack : InnerStack
	{
		public UIViewController Controller { get; }

		public InnerStack Container { get; }

		public SimpleControllerInnerStack(UIViewController host, UIViewController controller, InnerStack container) : base(host)
		{
			Controller = controller;
			Container = container;
		}

		public override int Count => 1;

		public override PopOperation AsPopOperation() => Container.AsSpecificPopOperation(this);

		public override PopOperation AsSpecificPopOperation(InnerStack child) => throw new NotSupportedException("This operation is not supported for simple controllers");
		public override PopOperation AsSpecificPopOperation(int count) => throw new NotSupportedException("This operation is not supported for simple controllers");

		public override UIViewController AsViewController() => Controller;
	}
}