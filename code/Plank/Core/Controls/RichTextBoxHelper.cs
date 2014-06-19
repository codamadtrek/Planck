using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace LazyE9.Plank.Core.Controls
{
	public class RichTextBoxHelper : DependencyObject
	{
		#region RichTextBoxHelper Members

		public static FlowDocument GetDocument( DependencyObject obj )
		{
			return (FlowDocument)obj.GetValue( DocumentProperty );
		}

		public static void SetDocument( DependencyObject obj, FlowDocument value )
		{
			obj.SetValue( DocumentProperty, value );
		}

		#endregion RichTextBoxHelper Members

		#region Fields

		public static readonly DependencyProperty DocumentProperty =
		  DependencyProperty.RegisterAttached(
			"Document",
			typeof( FlowDocument ),
			typeof( RichTextBoxHelper ),
			new FrameworkPropertyMetadata
			{
				BindsTwoWayByDefault = true,
				PropertyChangedCallback = ( obj, eventArgs ) =>
				{
					if( eventArgs.NewValue != null )
					{
						var richTextBox = (RichTextBox)obj;
						richTextBox.Document = (FlowDocument)eventArgs.NewValue;
					}
				}
			} );

		#endregion Fields

	}


}
