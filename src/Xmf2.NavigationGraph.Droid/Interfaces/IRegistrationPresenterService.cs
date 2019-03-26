using System;
using Android.Support.V4.App;
using Android.Support.V7.App;
using Xmf2.NavigationGraph.Core;
using Xmf2.NavigationGraph.Core.Interfaces;

namespace Xmf2.NavigationGraph.Droid.Interfaces
{
	public interface IRegistrationPresenterService<TViewModel> where TViewModel : IViewModel
	{
		void RegisterDefaultFragmentHost<TActivity>(bool shouldClearHistory = false)
			where TActivity : AppCompatActivity;

		void AssociateActivity<TActivity>(ScreenDefinition<TViewModel> screenDefinition, bool shouldClearHistory = false)
			where TActivity : AppCompatActivity;

		void AssociateFragment<TFragment>(ScreenDefinition<TViewModel> screenDefinition, Func<TFragment> fragmentCreator, Type hostActivityType = null, bool? shouldClearHistory = null)
			where TFragment : Fragment;

		void AssociateFragment<TFragmentHost, TFragment>(ScreenDefinition<TViewModel> screenDefinition, Func<TFragment> fragmentCreator)
			where TFragmentHost : AppCompatActivity
			where TFragment : Fragment;

		void AssociateDialogFragment<TDialogFragment>(ScreenDefinition<TViewModel> screenDefinition, Func<TDialogFragment> fragmentCreator, Type hostActivityType = null, bool? shouldClearHistory = null)
			where TDialogFragment : DialogFragment;

		void AssociateDialogFragment<TFragmentHost, TDialogFragment>(ScreenDefinition<TViewModel> screenDefinition, Func<TDialogFragment> fragmentCreator)
			where TFragmentHost : AppCompatActivity
			where TDialogFragment : DialogFragment;
	}
}