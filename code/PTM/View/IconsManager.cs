using System.Collections;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Windows.Forms;

namespace PTM.View
{
	internal static class IconsManager
	{
		#region Constructors

		#endregion Constructors

		#region IconsManager Members

		public const int DefaultTaskIconId = 1;
		public const int IdleTaskIconId = 0;

		public static void Initialize()
		{
			_LoadIconsFromResources();
		}

		#endregion IconsManager Members

		#region Private Members

		private static readonly Hashtable mIconsCommonTasks = new Hashtable();
		private static readonly ImageList mIconsList = new ImageList();

		private static Icon _GetFileIcon( string fileName )
		{
			//Icon ri;
			try
			{
				return IconHelper.GetFileIcon( fileName, IconHelper.IconSize.Small, false );
			}
			catch
			{
				return IconHelper.GetFileIcon( Application.ExecutablePath, IconHelper.IconSize.Small, false );
			} //try-catch
			//			if(ri!= null) return ri;
			//			ri = IconHandler.IconFromExtension(fileName, IconSize.Small);
			//			if(ri!= null) return ri;
			//			ri = IconHandler.IconFromExtensionShell(fileName, IconSize.Small);
			//			if(ri!= null) return ri;
			//			return null;
		}

		private static void _LoadIconsFromResources()
		{
			var resourceManager = new ResourceManager( "PTM.View.Controls.Icons", Assembly.GetExecutingAssembly() );

			Icon resIcon;
			int i = 0;
			do
			{
				resIcon = (Icon)resourceManager.GetObject(
									"Icon" + i.ToString( CultureInfo.InvariantCulture ) );
				if( resIcon != null )
				{
					mIconsList.Images.Add( resIcon );
					mIconsCommonTasks.Add( i, resIcon );
				} //if
				i++;
			} while( resIcon != null );

		}

		#endregion Private Members

		#region Internal Members

		internal static Hashtable CommonTaskIconsTable
		{
			get
			{
				return (Hashtable)mIconsCommonTasks.Clone();
			}
		}

		internal static ImageList IconsList
		{
			get
			{
				return mIconsList;
			}
		}

		internal static int GetIconFromFile( string fileName )
		{
			if( mIconsCommonTasks.Contains( fileName ) )
			{
				return (int)mIconsCommonTasks[fileName];
			}
			Icon icon = _GetFileIcon( fileName );
			mIconsList.Images.Add( icon );
			//iconsCommonTasks.Add(fileName, iconsList.Images.Count - 1);
			return mIconsList.Images.Count - 1;
		}

		#endregion Internal Members

	} //end of class IconsManager
} //end of namespace