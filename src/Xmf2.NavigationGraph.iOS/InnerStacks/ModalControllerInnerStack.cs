using UIKit;
using Xmf2.NavigationGraph.iOS.Operations;

namespace Xmf2.NavigationGraph.iOS.InnerStacks
{
	public class ModalControllerInnerStack : InnerStack
	{
		public ModalControllerInnerStack(UIViewController host) : base(host) { }

		public InnerStack Modal { get; set; }

		public override int Count => Modal.Count;

		public override PopOperation AsPopOperation()
		{
			return new ModalControllerPopOperation(this);
		}

		public override PopOperation AsSpecificPopOperation(InnerStack child) => Modal.AsSpecificPopOperation(child);
		public override PopOperation AsSpecificPopOperation(int count) => Modal.AsSpecificPopOperation(count);
		public override UIViewController AsViewController() => Modal.AsViewController();
	}
}