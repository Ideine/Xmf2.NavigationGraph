namespace System.Threading.Tasks
{
	public static class TaskExtensions
	{
		public static Task<TResult> AsTask<TResult>(this TResult result) => Task.FromResult(result);
	}
}
