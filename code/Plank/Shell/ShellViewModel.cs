using LazyE9.Plank.Core;
using LazyE9.Plank.UI;

namespace LazyE9.Plank.Shell
{
	public class ShellViewModel : ViewModelBase
	{
		protected override void OnInitialize( object payload )
		{
			base.OnInitialize( payload );

			NavigationManager.Push( new TaskLogViewModel(), new LoginView() );
		}
	}
}
