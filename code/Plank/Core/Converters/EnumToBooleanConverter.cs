using System;
using System.Windows;
using System.Windows.Data;

namespace LazyE9.Plank.Core.Converters
{
	[ValueConversion( typeof( Enum ), typeof( bool ) )]
	public class EnumToBooleanConverter : IValueConverter
	{
		#region EnumToBooleanConverter Members

		public object Convert( object value, Type targetType, object parameter, System.Globalization.CultureInfo culture )
		{
			object convertedValue;
			var parameterString = parameter as string;
			if( parameterString == null )
			{
				convertedValue = DependencyProperty.UnsetValue;
			}

			else if( Enum.IsDefined( value.GetType(), value ) == false )
			{
				convertedValue = DependencyProperty.UnsetValue;
			}
			else
			{
				object parameterValue = Enum.Parse( value.GetType(), parameterString );

				convertedValue = parameterValue.Equals( value );
			}
			return convertedValue;
		}

		public object ConvertBack( object value, Type targetType, object parameter, System.Globalization.CultureInfo culture )
		{
			var parameterString = parameter as string;
			if( parameterString == null )
			{
				return DependencyProperty.UnsetValue;
			}

			return Enum.Parse( targetType, parameterString );
		}

		#endregion EnumToBooleanConverter Members

	}
}