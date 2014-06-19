using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace LazyE9.Plank.Core.Converters
{
	[ValueConversion( typeof( string ), typeof( Visibility ) )]
	public class TextIsNotEmptyToVisibilityConverter : IValueConverter
	{
		#region TextIsNotEmptyToVisibilityConverter Members

		public object Convert( object value, Type targetType, object parameter, CultureInfo culture )
		{
			Visibility result = Visibility.Collapsed;
			if( value is string && !string.IsNullOrWhiteSpace( (string)value ) )
			{
				result = Visibility.Visible;
			}
			return result;
		}

		public object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture )
		{
			throw new NotImplementedException();
		}

		#endregion TextIsNotEmptyToVisibilityConverter Members

	}
}
