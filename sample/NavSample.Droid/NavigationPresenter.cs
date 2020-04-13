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