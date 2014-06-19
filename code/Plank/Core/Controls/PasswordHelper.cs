using System.Windows;
using System.Windows.Controls;

namespace LazyE9.Plank.Core.Controls
{
	/// <summary>
	/// This code was taken from http://wpftutorial.net/PasswordBox.html and modified slightly
	/// </summary>
	public static class PasswordHelper
	{
		#region PasswordHelper Members

		public static bool GetAttach( DependencyObject dependencyObject )
		{
			return (bool)dependencyObject.GetValue( AttachProperty );
		}

		public static string GetPassword( DependencyObject dependencyObject )
		{
			return (string)dependencyObject.GetValue( PasswordependencyObjectroperty );
		}

		public static void SetAttach( DependencyObject dependencyObject, bool value )
		{
			dependencyObject.SetValue( AttachProperty, value );
		}

		public static void SetPassword( DependencyObject dependencyObject, string value )
		{
			dependencyObject.SetValue( PasswordependencyObjectroperty, value );
		}

		#endregion PasswordHelper Members

		#region Fields

		public static readonly DependencyProperty PasswordependencyObjectroperty =
			DependencyProperty.RegisterAttached( "Password",
			typeof( string ), typeof( PasswordHelper ),
			new FrameworkPropertyMetadata( string.Empty, _OnPasswordependencyObjectropertyChanged ) );
		public static readonly DependencyProperty AttachProperty =
			DependencyProperty.RegisterAttached( "Attach",
			typeof( bool ), typeof( PasswordHelper ), new PropertyMetadata( false, _Attach ) );
		private static readonly DependencyProperty mIsUpdatingProperty =
		   DependencyProperty.RegisterAttached( "IsUpdating", typeof( bool ),
		   typeof( PasswordHelper ) );

		#endregion Fields

		#region Private Members

		private static void _Attach( DependencyObject sender,
			DependencyPropertyChangedEventArgs eventArgs )
		{
			var passwordBox = sender as PasswordBox;

			if( passwordBox == null )
			{
				return;
			}

			if( (bool)eventArgs.OldValue )
			{
				passwordBox.LostFocus -= _PasswordLostFocus;
			}

			if( (bool)eventArgs.NewValue )
			{
				passwordBox.LostFocus += _PasswordLostFocus;
			}
		}

		private static bool _GetIsUpdating( DependencyObject dependencyObject )
		{
			return (bool)dependencyObject.GetValue( mIsUpdatingProperty );
		}

		private static void _OnPasswordependencyObjectropertyChanged( DependencyObject sender,
			DependencyPropertyChangedEventArgs eventArgs )
		{
			var passwordBox = sender as PasswordBox;
			passwordBox.LostFocus -= _PasswordLostFocus;

			if( !_GetIsUpdating( passwordBox ) )
			{
				passwordBox.Password = (string)eventArgs.NewValue;
			}
			passwordBox.LostFocus += _PasswordLostFocus;
		}

		private static void _PasswordLostFocus( object sender, RoutedEventArgs eventArgs )
		{
			var passwordBox = sender as PasswordBox;
			_SetIsUpdating( passwordBox, true );
			SetPassword( passwordBox, passwordBox.Password );
			_SetIsUpdating( passwordBox, false );
		}

		private static void _SetIsUpdating( DependencyObject dependencyObject, bool value )
		{
			dependencyObject.SetValue( mIsUpdatingProperty, value );
		}

		#endregion Private Members

	}
}
