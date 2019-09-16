using System;
using System.Collections.Generic;
using Xmf2.NavigationGraph.Core.Interfaces;
using Xmf2.NavigationGraph.Droid.Interfaces;

namespace Xmf2.NavigationGraph.Droid
{
	public static class NavigationParameterContainer<TViewModel> where TViewModel : IViewModel
	{
		private static readonly Dictionary<string, object> _parameters = new Dictionary<string, object>();

		private static readonly Guid _id = Guid.NewGuid();

		internal static string CreateNavigationParameter(object parameter)
		{
			string key = $"{parameter.GetType().Name}+{Guid.NewGuid():N}";
			_parameters.Add(key, parameter);
			Android.Util.Log.Info("NAVPARAMETER", $"Added {key} / id : {_id}");
			return key;
		}

		public static TViewModel GetViewModel(string key) => Get<TViewModel>(key);
		public static IDeferredNavigationAction GetDeferredNavigationAction(string key) => Get<IDeferredNavigationAction>(key);

		private static T Get<T>(string key)
		{
			if (_parameters.TryGetValue(key, out object result))
			{
				return (T)result;
			}

			Android.Util.Log.Info("NAVPARAMETER", $"{key} not found / id : {_id}");

			throw new ArgumentOutOfRangeException($"The key {key} does not match any navigation parameters");
		}
	}
}