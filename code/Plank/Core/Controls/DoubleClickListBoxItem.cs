using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace LazyE9.Plank.Core.Controls
{
	public class DoubleClickListBoxItem	: DependencyObject
	{
		#region RichTextBoxHelper Members

		public static ICommand GetDoubleClickCommand( DependencyObject obj )
		{
			return (ICommand)obj.GetValue( DoubleClickCommandProperty );
		}

		public static void SetDoubleClickCommand( DependencyObject obj, ICommand value )
		{
			obj.SetValue( DoubleClickCommandProperty, value );
		}

		#endregion RichTextBoxHelper Members

		#region Fields

		public static readonly DependencyProperty DoubleClickCommandProperty = DependencyProperty.RegisterAttached( "DoubleClickCommand", 
			typeof( ICommand ), typeof( DoubleClickListBoxItem ), new FrameworkPropertyMetadata( _HandleDoubleClickCommandPropertyChanged ) );

		private static void _HandleDoubleClickCommandPropertyChanged( DependencyObject dependencyObject, DependencyPropertyChangedEventArgs eventArgs )
		{
			var listBox = (ListBox)dependencyObject;
			listBox.MouseDoubleClick -= _HandleListBoxMouseDoubleClick;

			if( eventArgs.NewValue != null )
			{
				listBox.MouseDoubleClick += _HandleListBoxMouseDoubleClick;
			}
		}

		private static void _HandleListBoxMouseDoubleClick( object sender, MouseButtonEventArgs eventArgs )
		{
			var command = GetDoubleClickCommand( (ListBox)sender );
			if( command != null )
			{
				command.Execute( null );
			}
		}

		#endregion Fields

	}
}