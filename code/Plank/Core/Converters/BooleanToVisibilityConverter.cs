using System;
using System.Windows;
using System.Windows.Data;

namespace LazyE9.Plank.Core.Converters
{
	[ValueConversion( typeof( bool ), typeof( Visibility ) )]
	public class BooleanToVisibilityConverter : IValueConverter
	{
		#region Constructors

		public BooleanToVisibilityConverter()
		{
			TrueVisibility = Visibility.Visible;
			FalseVisibility = Visibility.Collapsed;
		}

		#endregion Constructors

		#region BooleanToVisibilityConverter Members

		public Visibility FalseVisibility
		{
			get;
			set;
		}

		public Visibility NullVisiblity
		{
			get;
			set;
		}

		public Visibility TrueVisibility
		{
			get;
			set;
		}

		public object Convert( object value, Type targetType, object parameter, System.Globalization.CultureInfo culture )
		{
			Visibility visibility = NullVisiblity;
			if( value != null )
			{
				visibility = System.Convert.ToBoolean( value )
					? TrueVisibility
					: FalseVisibility;
			}
			return visibility;
		}

		public object ConvertBack( object value, Type targetType, object parameter, System.Globalization.CultureInfo culture )
		{
			throw new NotImplementedException();
		}

		#endregion BooleanToVisibilityConverter Members

	}
}
