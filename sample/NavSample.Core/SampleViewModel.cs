using Xmf2.NavigationGraph.Core.Interfaces;

namespace NavSample.Core
{
	public class SampleViewModel : IViewModel
	{
		private readonly string _name;

		public SampleViewModel(string name)
		{
			_name = name;
			System.Diagnostics.Debug.WriteLine($"Creating: {_name}");
		}

		public override string ToString() => _name;

		public void Dispose()
		{
			System.Diagnostics.Debug.WriteLine($"Disposing: {_name}");
		}
	}
}