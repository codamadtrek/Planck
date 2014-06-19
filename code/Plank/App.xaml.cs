using System.Windows;

using LazyE9.Plank.Core;
using LazyE9.Plank.Shell;

namespace LazyE9.Plank
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App 
	{
		protected override void OnStartup( StartupEventArgs eventArgs )
		{
			_RegisterServices();

			base.OnStartup( eventArgs );

			var vm = new ShellViewModel();
			vm.Initialize( null );

			var mainWindow = new ShellView
			{
				DataContext = vm
			};
			
			MainWindow = mainWindow;
			mainWindow.Show();
		}

		private void _RegisterServices()
		{
			ServiceLocator.Default.RegisterInstance( new ShellNavigationManager() );
		}
	}
}
