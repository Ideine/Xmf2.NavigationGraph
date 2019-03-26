namespace Xmf2.NavigationGraph.Core.Interfaces
{
	public interface INavigationInProgress
	{
		bool IsCancelled { get; }
		void Commit();
	}
}