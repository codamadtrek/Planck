using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;

using LazyE9.Plank.Core;
using LazyE9.Plank.Properties;

namespace LazyE9.Plank.Shell
{
	public class ShellNavigationManager : INotifyPropertyChanged
	{
		#region ShellNavigationManager Members

		private ContentControl mCurrentView;
		public ContentControl CurrentView
		{
			get
			{
				return mCurrentView;
			}
			set
			{
				if( !Equals( value, mCurrentView ) )
				{
					mCurrentView = value;
					OnPropertyChanged();
				}
			}
		}

		public void Pop()
		{
			var oldRegistration = mNavigationStack.Pop();
			var currentRegistration = mNavigationStack.Peek();

			CurrentView = currentRegistration.View;

			oldRegistration.View.DataContext = null;
			oldRegistration.ViewModel.Dispose();
		}

		public void Push( ViewModelBase viewModel, ContentControl view, object payload = null )
		{
			view.DataContext = viewModel;

			mNavigationStack.Push( new NavigationManagerRegistration
			{
				ViewModel = viewModel,
				View = view
			} );

			CurrentView = view;
			viewModel.Initialize( payload );
		}

		#endregion ShellNavigationManager Members

		#region Ëvents

		public event PropertyChangedEventHandler PropertyChanged;

		#endregion Ëvents

		#region Fields

		private readonly Stack<NavigationManagerRegistration> mNavigationStack = new Stack<NavigationManagerRegistration>();

		#endregion Fields

		#region Protected Members

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged( [CallerMemberName] string propertyName = null )
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if( handler != null )
			{
				handler( this, new PropertyChangedEventArgs( propertyName ) );
			}
		}

		#endregion Protected Members

		#region Other

		private class NavigationManagerRegistration
		{
			#region NavigationManagerRegistration Members

			public ContentControl View
			{
				get;
				set;
			}

			public ViewModelBase ViewModel
			{
				get;
				set;
			}

			#endregion NavigationManagerRegistration Members

		}

		#endregion Other

	}
}
