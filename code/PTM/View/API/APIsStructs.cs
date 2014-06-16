using System.Drawing;

namespace System.Runtime.InteropServices.APIs
{
	internal class APIsStructs
	{
		#region Other

		public enum ComboBoxButtonState
		{
			STATE_SYSTEM_NONE = 0,
			STATE_SYSTEM_INVISIBLE = 0x00008000,
			STATE_SYSTEM_PRESSED = 0x00000008
		}

		[StructLayout( LayoutKind.Sequential )]
		public struct COMBOBOXINFO
		{
			#region COMBOBOXINFO Members

			public ComboBoxButtonState buttonState;
			public Int32 cbSize;
			public IntPtr hwndCombo;
			public IntPtr hwndItem;
			public IntPtr hwndList;
			public RECT rcButton;
			public RECT rcItem;

			#endregion COMBOBOXINFO Members

		}

		[StructLayout( LayoutKind.Sequential, CharSet = CharSet.Auto )]
		internal struct DLLVERSIONINFO
		{
			#region Internal Members

			internal int cbSize;
			internal int dwBuildNumber;
			internal int dwMajorVersion;
			internal int dwMinorVersion;
			internal int dwPlatformID;

			#endregion Internal Members

		}

		[StructLayout( LayoutKind.Sequential, CharSet = CharSet.Auto )]
		internal struct DLLVERSIONINFO2
		{
			#region Private Members

			private ulong ullVersion;

			#endregion Private Members

			#region Internal Members

			internal DLLVERSIONINFO info1;
			internal int dwFlags;

			#endregion Internal Members

		}

		[StructLayout( LayoutKind.Sequential, CharSet = CharSet.Auto )]
		internal struct HDITEM
		{
			#region Internal Members

			internal APIsEnums.HeaderItemFlags mask;
			internal int cchTextMax;
			internal int cxy;
			internal int fmt;
			internal int iImage;
			internal int iOrder;
			internal int lParam;
			internal IntPtr hbm;
			internal IntPtr pszText;

			#endregion Internal Members

		}

		[StructLayoutAttribute( LayoutKind.Sequential )]
		internal struct LV_ITEM
		{
			#region Internal Members

			internal Int32 cchTextMax;
			internal Int32 iImage;
			internal Int32 iIndent;
			internal Int32 iItem;
			internal Int32 iSubItem;
			internal IntPtr lParam;
			internal APIsEnums.ListViewItemFlags mask;
			internal APIsEnums.ListViewItemStates state;
			internal APIsEnums.ListViewItemStates stateMask;
			internal String pszText;

			#endregion Internal Members

		}

		[StructLayoutAttribute( LayoutKind.Sequential )]
		internal struct LVHITTESTINFO
		{
			#region Internal Members

			internal int flags;
			internal Int32 iItem;
			internal Int32 iSubItem;
			internal POINTAPI pt;

			#endregion Internal Members

		}

		[StructLayout( LayoutKind.Sequential )]
		internal struct NMCUSTOMDRAW
		{
			#region Internal Members

			internal int dwDrawStage;
			internal IntPtr hdc;
			internal IntPtr lItemlParam;
			internal NMHDR hdr;
			internal RECT rc;
			internal uint dwItemSpec;
			internal uint uItemState;

			#endregion Internal Members

		}

		//		[StructLayout(LayoutKind.Sequential)]
		internal struct NMHDR
		{
			#region Internal Members

			internal int code;
			internal int idFrom;
			internal IntPtr hwndFrom;

			#endregion Internal Members

		}

		[StructLayout( LayoutKind.Sequential, Pack = 1 )]
		internal struct NMHEADER
		{
			#region Internal Members

			internal HDITEM pItem;
			internal int iButton;
			internal int iItem;
			internal NMHDR nmhdr;

			#endregion Internal Members

		}

		[StructLayout( LayoutKind.Sequential, CharSet = CharSet.Auto )]
		internal struct NMLISTVIEW
		{
			#region Internal Members

			internal int iItem;
			internal int iSubItem;
			internal IntPtr lParam;
			internal NMHDR nmhdr;
			internal POINTAPI ptAction;
			internal uint uChanged;
			internal uint uNewState;
			internal uint uOldState;

			internal bool Check
			{
				get
				{
					return !OldCheck && NewCheck;
				}
			}

			internal bool Focus
			{
				get
				{
					return !OldFocused && NewFocused;
				}
			}

			internal bool NewCheck
			{
				get
				{
					try
					{
						return
							uNewState >= 0x1000 ? ((uNewState & (uint)APIsEnums.ListViewItemStates.STATEIMAGEMASK) >> 12) - 1 > 0 : false;
					}
					catch
					{
						return false;
					}
				}
			}

			internal bool NewFocused
			{
				get
				{
					return
						((APIsEnums.ListViewItemStates)uNewState & APIsEnums.ListViewItemStates.FOCUSED) ==
						APIsEnums.ListViewItemStates.FOCUSED;
				}
			}

			internal bool NewSelected
			{
				get
				{
					return
						((APIsEnums.ListViewItemStates)uNewState & APIsEnums.ListViewItemStates.SELECTED) ==
						APIsEnums.ListViewItemStates.SELECTED;
				}
			}

			internal bool OldCheck
			{
				get
				{
					try
					{
						return
							uOldState >= 0x1000 ? ((uOldState & (uint)APIsEnums.ListViewItemStates.STATEIMAGEMASK) >> 12) - 1 > 0 : false;
					}
					catch
					{
						return false;
					}
				}
			}

			internal bool OldFocused
			{
				get
				{
					return
						((APIsEnums.ListViewItemStates)uOldState & APIsEnums.ListViewItemStates.FOCUSED) ==
						APIsEnums.ListViewItemStates.FOCUSED;
				}
			}

			internal bool OldSelected
			{
				get
				{
					return
						((APIsEnums.ListViewItemStates)uOldState & APIsEnums.ListViewItemStates.SELECTED) ==
						APIsEnums.ListViewItemStates.SELECTED;
				}
			}

			internal bool Select
			{
				get
				{
					return !OldSelected && NewSelected;
				}
			}

			internal bool UnCheck
			{
				get
				{
					return OldCheck && !NewCheck;
				}
			}

			internal bool UnFocus
			{
				get
				{
					return OldFocused && !NewFocused;
				}
			}

			internal bool UnSelect
			{
				get
				{
					return OldSelected && !NewSelected;
				}
			}

			#endregion Internal Members

		}

		[StructLayout( LayoutKind.Sequential )]
		internal struct NMLVCUSTOMDRAW
		{
			#region Internal Members

			internal int clrFace;
			internal int clrText;
			internal int clrTextBk;
			internal int dwItemType;
			internal int iIconEffect;
			internal int iIconPhase;
			internal int iPartId;
			internal int iStateId;
			internal int iSubItem;
			internal NMCUSTOMDRAW nmcd;
			internal RECT rcText;
			internal uint uAlign;

			#endregion Internal Members

		}

		[StructLayoutAttribute( LayoutKind.Sequential )]
		internal struct POINTAPI
		{
			#region Constructors

			internal POINTAPI( Point p )
			{
				x = p.X;
				y = p.Y;
			}

			internal POINTAPI( Int32 X, Int32 Y )
			{
				x = X;
				y = Y;
			}

			#endregion Constructors

			#region Internal Members

			internal Int32 x;
			internal Int32 y;

			#endregion Internal Members

		}

		[StructLayout( LayoutKind.Sequential )]
		internal struct RECT
		{
			#region Constructors

			internal RECT( Rectangle rectangle )
			{
				left = rectangle.Left;
				top = rectangle.Top;
				right = rectangle.Right;
				bottom = rectangle.Bottom;
			}

			#endregion Constructors

			#region Internal Members

			internal int bottom;
			internal int left;
			internal int right;
			internal int top;

			#endregion Internal Members

		}

		#endregion Other

	}
}