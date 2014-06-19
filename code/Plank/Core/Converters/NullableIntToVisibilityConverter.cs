using System;
using System.Windows;
using System.Windows.Data;

namespace LazyE9.Plank.Core.Converters
{
	[ValueConversion( typeof( int? ), typeof( Visibility ) )]
	public class NullableIntToVisibilityConverter : IValueConverter
	{
		#region NullableIntToVisibilityConverter Members

		public bool Reverse
		{
			get;
			set;
		}

		public object Convert( object value, Type targetType, object parameter, System.Globalization.CultureInfo culture )
		{
			Visibility visibility;
			if( value == null )
			{
				visibility = Reverse
					? Visibility.Visible
					: Visibility.Collapsed;
			}
			else
			{
				visibility = Reverse
					? Visibility.Collapsed
					: Visibility.Visible;
			}
			return visibility;
		}

		public object ConvertBack( object value, Type targetType, object parameter, System.Globalization.CultureInfo culture )
		{
			throw new NotImplementedException();
		}

		#endregion NullableIntToVisibilityConverter Members

	}
}