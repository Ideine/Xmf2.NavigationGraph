using System;
using Xmf2.NavigationGraph.Core.Interfaces;
using System.Runtime.ConstrainedExecution;
using System.Threading.Tasks;

namespace Xmf2.NavigationGraph.Core
{
	public class ScreenDefinition<TViewModel> where TViewModel : IViewModel
	{
		public Guid Id { get; } = Guid.NewGuid();

		public string RelativeRoute { get; }

		public bool IsParameterRoute { get; }

		public string ParameterName { get; }

		public ViewModelCreator<TViewModel> DefaultViewModelCreator { get; }
		public bool IsInvisible { get; set; }


		public ScreenDefinition(string relativeRoute, SyncViewModelCreator<TViewModel> defaultViewModelCreator)
			: this(relativeRoute, route => Task.FromResult(defaultViewModelCreator(route)))
		{

		}

		public ScreenDefinition(string relativeRoute, ViewModelCreator<TViewModel> defaultViewModelCreator)
		{
			RelativeRoute = relativeRoute.Trim();
			IsParameterRoute = RelativeRoute.StartsWith("{") && RelativeRoute.EndsWith("}");
			ParameterName = RelativeRoute.Substring(1, RelativeRoute.Length - 2);
			DefaultViewModelCreator = defaultViewModelCreator ?? throw new ArgumentNullException(nameof(defaultViewModelCreator));
		}

		public override bool Equals(object obj)
		{
			if (obj is ScreenDefinition<TViewModel> screenDefinition)
			{
				if (ReferenceEquals(this, obj))
				{
					return true;
				}

				if (obj.GetType() != GetType())
				{
					return false;
				}

				return Equals(screenDefinition);
			}

			return false;
		}

		protected bool Equals(ScreenDefinition<TViewModel> other)
		{
			return Id.Equals(other.Id);
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode();
		}

		public static bool operator ==(ScreenDefinition<TViewModel> left, ScreenDefinition<TViewModel> right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(ScreenDefinition<TViewModel> left, ScreenDefinition<TViewModel> right)
		{
			return !Equals(left, right);
		}

		public override string ToString()
		{
			return RelativeRoute;
		}
	}
}