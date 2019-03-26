using System;
using Android.Support.V4.App;
using Android.Support.V7.App;
using Xmf2.NavigationGraph.Core;

namespace Xmf2.NavigationGraph.Droid.Interfaces
{
	public interface IRegistrationPresenterService
	{
		void RegisterDefaultFragmentHost<TActivity>(bool shouldClearHistory = false)
			where TActivity : AppCompatActivity;

		void AssociateActivity<TActivity>(ScreenDefinition screenDefinition, bool shouldClearHistory = false)
			where TActivity : AppCompatActivity;

		void AssociateFragment<TFragment>(ScreenDefinition screenDefinition, Func<TFragment> fragmentCreator, Type hostActivityType = null, bool? shouldClearHistory = null)
			where TFragment : Fragment;

		void AssociateFragment<TFragmentHost, TFragment>(ScreenDefinition screenDefinition, Func<TFragment> fragmentCreator)
			where TFragmentHost : AppCompatActivity
			where TFragment : Fragment;

		void AssociateDialogFragment<TDialogFragment>(ScreenDefinition screenDefinition, Func<TDialogFragment> fragmentCreator, Type hostActivityType = null, bool? shouldClearHistory = null)
			where TDialogFragment : DialogFragment;

		void AssociateDialogFragment<TFragmentHost, TDialogFragment>(ScreenDefinition screenDefinition, Func<TDialogFragment> fragmentCreator)
			where TFragmentHost : AppCompatActivity
			where TDialogFragment : DialogFragment;
	}
}