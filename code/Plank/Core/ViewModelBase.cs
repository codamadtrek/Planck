using System;

using LazyE9.Plank.Shell;

namespace LazyE9.Plank.Core
{
	public abstract class ViewModelBase : ModelBase, IDisposable
	{
		#region ViewModelBase Members

		private ShellNavigationManager mNavigationManager;
		public ShellNavigationManager NavigationManager
		{
			get
			{
				return mNavigationManager;
			}
			set
			{
				if( Equals( value, mNavigationManager ) )
				{
					return;
				}
				mNavigationManager = value;
				OnPropertyChanged();
			}
		}

		public void Dispose()
		{
			Dispose( true );
		}

		public void Initialize( object payload )
		{
			OnInitialize( payload );
		}

		#endregion ViewModelBase Members

		#region Protected Members

		protected virtual void Dispose( bool disposing )
		{

		}

		protected virtual void OnInitialize( object payload )
		{
			NavigationManager = ServiceLocator.Default.Resolve<ShellNavigationManager>();
		}

		#endregion Protected Members

	}
}
