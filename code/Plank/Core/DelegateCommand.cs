using System;
using System.Windows.Input;

namespace LazyE9.Plank.Core
{
	public class DelegateCommand : ICommand
	{
		#region Constructors

		public DelegateCommand( Action execute, Func<bool> canExecute = null )
		{
			mExecute = execute;
			mCanExecute = canExecute;
		}

		#endregion Constructors

		#region DelegateCommand Members

		public bool CanExecute( object parameter )
		{
			var canExecute = true;
			if( mCanExecute != null )
			{
				canExecute = mCanExecute();
			}
			return canExecute;
		}

		public void Execute( object parameter )
		{
			if( mExecute != null )
			{
				mExecute();
			}
		}

		public void RaiseCanExecuteChanged()
		{
			OnCanExecuteChanged();
		}

		#endregion DelegateCommand Members

		#region Ëvents

		public event EventHandler CanExecuteChanged;

		#endregion Ëvents

		#region Fields

		private readonly Action mExecute;
		private readonly Func<bool> mCanExecute;

		#endregion Fields

		#region Protected Members

		protected virtual void OnCanExecuteChanged()
		{
			EventHandler handler = CanExecuteChanged;
			if( handler != null )
			{
				handler( this, EventArgs.Empty );
			}
		}

		#endregion Protected Members

	}
}
