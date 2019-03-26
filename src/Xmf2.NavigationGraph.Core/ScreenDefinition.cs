using System;

namespace Xmf2.NavigationGraph.Core
{
	public class ScreenDefinition
	{
		public Guid Id { get; } = Guid.NewGuid();

		public string RelativeRoute { get; }

		public bool IsParameterRoute { get; }

		public string ParameterName { get; }
		
		public ViewModelCreator DefaultViewModelCreator { get; }

		public ScreenDefinition(string relativeRoute, ViewModelCreator defaultViewModelCreator)
		{
			RelativeRoute = relativeRoute.Trim();
			IsParameterRoute = RelativeRoute.StartsWith("{") && RelativeRoute.EndsWith("}");
			ParameterName = RelativeRoute.Substring(1, RelativeRoute.Length - 2);
			DefaultViewModelCreator = defaultViewModelCreator ?? throw new ArgumentNullException(nameof(defaultViewModelCreator));
		}

		public override bool Equals(object obj)
		{
			if (obj is ScreenDefinition screenDefinition)
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

		protected bool Equals(ScreenDefinition other)
		{
			return Id.Equals(other.Id);
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode();
		}

		public static bool operator ==(ScreenDefinition left, ScreenDefinition right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(ScreenDefinition left, ScreenDefinition right)
		{
			return !Equals(left, right);
		}

		public override string ToString()
		{
			return RelativeRoute;
		}
	}
}