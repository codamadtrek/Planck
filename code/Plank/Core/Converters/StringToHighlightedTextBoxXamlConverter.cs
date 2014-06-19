using System;
using System.Globalization;
using System.IO;
using System.Security;
using System.Text.RegularExpressions;
using System.Windows.Data;
using System.Windows.Markup;
using System.Xml;

namespace LazyE9.Plank.Core.Converters
{

	/// <summary>
	/// Converts a string containing valid XAML into WPF objects.
	/// </summary>
	[ValueConversion( typeof( string ), typeof( object ) )]
	public sealed class StringToHighlightedTextBoxXamlConverter : IMultiValueConverter
	{
		#region StringToHighlightedTextBoxXamlConverter Members

		public object Convert( object[] values, Type targetType, object parameter, CultureInfo culture )
		{
			var input = values[0] as string;
			var searchText = values[1] as string;
			string outputText;

			const string TEXT_BLOCK_XAML = "<RichTextBox xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" Foreground=\"Black\" IsReadOnly=\"True\" BorderThickness=\"0\" Background=\"Transparent\"><FlowDocument><Paragraph>{0}</Paragraph></FlowDocument></RichTextBox>";

			if( input != null && !string.IsNullOrWhiteSpace( searchText ) )
			{
				searchText = string.Format( "({0})", Regex.Escape( searchText ) );
				var markedText = Regex.Replace( input, searchText, "|~S~|$1|~E~|", RegexOptions.IgnoreCase );
				string escapedXml = SecurityElement.Escape( markedText );

				string withTags = escapedXml
					.Replace( "|~S~|", "<Run Background=\"Yellow\">" )
					.Replace( "|~E~|", "</Run>" )
					.Replace( "\n", "<LineBreak/>" );

				outputText = string.Format( TEXT_BLOCK_XAML, withTags );
			}
			else if( input != null )
			{
				string escapedXml = SecurityElement.Escape( input );
				string withTags = escapedXml.Replace( "\n", "<LineBreak/>" );

				outputText = string.Format( TEXT_BLOCK_XAML, withTags );
			}
			else
			{
				outputText = string.Format( TEXT_BLOCK_XAML, string.Empty );
			}

			using( var stringReader = new StringReader( outputText ) )
			{
				using( XmlReader xmlReader = XmlReader.Create( stringReader ) )
				{
					return XamlReader.Load( xmlReader );
				}
			}
		}

		public object[] ConvertBack( object value, Type[] targetTypes, object parameter, CultureInfo culture )
		{
			throw new NotImplementedException();
		}

		#endregion StringToHighlightedTextBoxXamlConverter Members

	}

}
