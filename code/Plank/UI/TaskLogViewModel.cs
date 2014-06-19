using System.Threading.Tasks;

using LazyE9.Plank.Core;

namespace LazyE9.Plank.UI
{
	public class TaskLogViewModel : ViewModelBase
	{
		#region Protected Members

		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
			}
			base.Dispose( disposing );
		}

		protected async override void OnInitialize( object payload )
		{
			base.OnInitialize( payload );
			await Task.Run( () =>
			{
			} );
		}

		#endregion Protected Members

	}
}
