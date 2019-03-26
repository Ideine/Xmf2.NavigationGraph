using System;
using System.Threading.Tasks;
using Xmf2.NavigationGraph.Core.Interfaces;

namespace Xmf2.NavigationGraph.Core
{
	public class ScreenInstance<TViewModel> : IDisposable where TViewModel : IViewModel
	{
		public ScreenDefinition<TViewModel> Definition { get; }

		public string Parameter { get; }

		public ViewModelCreator<TViewModel> ViewModelCreator { get; }

		public TViewModel ViewModelInstance { get; private set; }

		public ScreenInstance(ScreenDefinition<TViewModel> definition, string parameter, ViewModelCreator<TViewModel> viewModelCreator)
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
			if (obj is ScreenInstance<TViewModel> screenInstance)
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

		protected bool Equals(ScreenInstance<TViewModel> other)
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

		public static bool operator ==(ScreenInstance<TViewModel> left, ScreenInstance<TViewModel> right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(ScreenInstance<TViewModel> left, ScreenInstance<TViewModel> right)
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