using System.ComponentModel;
using System.Runtime.CompilerServices;

using LazyE9.Plank.Properties;

namespace LazyE9.Plank.Core
{
	public abstract class ModelBase : INotifyPropertyChanged
	{
		#region Ëvents

		public event PropertyChangedEventHandler PropertyChanged;

		#endregion Ëvents

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

	}
}
