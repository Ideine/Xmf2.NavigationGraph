using System;
using System.Threading.Tasks;
using Xmf2.NavigationGraph.Core.Interfaces;

namespace Xmf2.NavigationGraph.Core
{
	public class ScreenInstance : IDisposable
	{
		public ScreenDefinition Definition { get; }

		public string Parameter { get; }

		public ViewModelCreator ViewModelCreator { get; }

		public IViewModel ViewModelInstance { get; private set; }

		public ScreenInstance(ScreenDefinition definition, string parameter, ViewModelCreator viewModelCreator)
		{
			Definition = definition;
			Parameter = parameter;
			ViewModelCreator = viewModelCreator ?? definition.DefaultViewModelCreator;
		}

		public async Task<IViewModel> GetViewModel(string route)
		{
			if (ViewModelInstance != null)
			{
				return ViewModelInstance;
			}

			return ViewModelInstance = await ViewModelCreator(route);
		}

		#region Equality members

		public override bool Equals(object obj)
		{
			if (obj is ScreenInstance screenInstance)
			{
				if (ReferenceEquals(this, obj))
				{
					return true;
				}

				if (obj.GetType() != GetType())
				{
					return false;
				}

				return Equals(screenInstance);
			}

			return false;
		}

		protected bool Equals(ScreenInstance other)
		{
			return Equals(Definition.Id, other.Definition.Id) &&
			       string.Equals(Parameter, other.Parameter);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return ((Definition != null ? Definition.Id.GetHashCode() : 0) * 397) ^ (Parameter != null ? Parameter.GetHashCode() : 0);
			}
		}

		public static bool operator ==(ScreenInstance left, ScreenInstance right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(ScreenInstance left, ScreenInstance right)
		{
			return !Equals(left, right);
		}

		#endregion

		protected virtual void Dispose(bool disposing)
		{
			if (disposing) { }
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		~ScreenInstance()
		{
			Dispose(false);
		}

		public override string ToString()
		{
			if (Definition.IsParameterRoute)
			{
				return $"{Definition.RelativeRoute.Substring(0, Definition.RelativeRoute.Length - 1)}:{Parameter}}}";
			}

			return Definition.RelativeRoute;
		}
	}
}