using System;
using System.Collections.Generic;
using Android.App;
using NavSample.Core;
using Xmf2.NavigationGraph.Core.Interfaces;
using Xmf2.NavigationGraph.Droid;

namespace NavSample.Droid
{
	/* TODO 
	 * TODO - Les activity devront probablement hériter d'une class interne qui permettra de gérer l'affichage des fragments pour une nouvelle activity
	 * TODO - Il sera nécessaire d'avoir une classe static permettant de transmettre les view models au activity du fait de ne pas pouvoir récupérer l'instance
	 */

	/*
	 * Dans le cas où l'on voudrait show plusieurs activity :
	 * Intent intent = new Intent (this, MainActivity.class);
     *  TaskStackBuilder stackBuilder = TaskStackBuilder.create(this);
     *  stackBuilder.addParentStack(MainActivity.class);
     *  stackBuilder.addNextIntent(intent);
     *  Intent intentEmailView = new Intent (this, EmailViewActivity.class);
     *  intentEmailView.putExtra("EmailId","you can Pass emailId here");
     *  stackBuilder.addNextIntent(intentEmailView);
	 */

	public interface IDeferredNavigationAction
	{
		void Execute(Activity activity);
	}

	public static class NavigationParameterContainer
	{
		private static readonly Dictionary<string, object> _parameters = new Dictionary<string, object>();

		internal static string CreateNavigationParameter(object parameter)
		{
			string key = $"{parameter.GetType().Name}+{Guid.NewGuid():N}";
			_parameters.Add(key, parameter);
			return key;
		}

		public static IViewModel GetViewModel(string key) => Get<IViewModel>(key);
		public static IDeferredNavigationAction GetDeferredNavigationAction(string key) => Get<IDeferredNavigationAction>(key);

		private static T Get<T>(string key)
		{
			if (_parameters.TryGetValue(key, out object result))
			{
				return (T)result;
			}

			throw new ArgumentOutOfRangeException($"The key {key} does not match any navigation parameters");
		}
	}

	public class SampleNavigationPresenter : NavigationPresenter<SampleViewModel>
	{
		public SampleNavigationPresenter(IViewModelLocatorService<SampleViewModel> viewModelLocatorService) : base(viewModelLocatorService)
		{
		}
	}

	public interface IFragmentActivity
	{
		int FragmentContainerId { get; }
	}
}