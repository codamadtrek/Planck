using System;
using System.Runtime.InteropServices;

namespace PTM.View
{
	internal sealed class MemoryMappedFile : IDisposable
	{
		#region Constructors

		private MemoryMappedFile( IntPtr memoryFileHandle )
		{
			mMemoryFileHandle = memoryFileHandle;
		}

		#endregion Constructors

		#region MemoryMappedFile Members

		public void Dispose()
		{
			if( mMemoryFileHandle != IntPtr.Zero )
			{
				if( CloseHandle( (uint)mMemoryFileHandle ) )
					mMemoryFileHandle = IntPtr.Zero;
			}
		}

		#endregion MemoryMappedFile Members

		#region Private Members

		private const int FILE_MAP_READ = 0x0004;
		private const int FILE_MAP_WRITE = 0x2;
		private IntPtr mMemoryFileHandle;

		[DllImport( "kernel32.dll", EntryPoint = "CloseHandle", SetLastError = true, CharSet = CharSet.Auto )]
		[return: MarshalAs( UnmanagedType.Bool )]
		private static extern bool CloseHandle( uint hHandle );

		[DllImport( "Kernel32.dll", EntryPoint = "CreateFileMapping", SetLastError = true, CharSet = CharSet.Auto )]
		private static extern IntPtr CreateFileMapping( uint hFile, IntPtr lpAttributes, uint flProtect, uint dwMaximumSizeHigh,
													   uint dwMaximumSizeLow, string lpName );

		[DllImport( "Kernel32.dll" )]
		private static extern IntPtr MapViewOfFile( IntPtr hFileMappingObject, uint dwDesiredAccess, uint dwFileOffsetHigh,
												   uint dwFileOffsetLow, uint dwNumberOfBytesToMap );

		[DllImport( "kernel32.dll", EntryPoint = "OpenFileMapping", SetLastError = true, CharSet = CharSet.Auto )]
		private static extern IntPtr OpenFileMapping( int dwDesiredAccess, bool bInheritHandle, String lpName );

		[DllImport( "Kernel32.dll", EntryPoint = "UnmapViewOfFile", SetLastError = true, CharSet = CharSet.Auto )]
		[return: MarshalAs( UnmanagedType.Bool )]
		private static extern bool UnmapViewOfFile( IntPtr lpBaseAddress );

		#endregion Private Members

		#region Internal Members

		internal static MemoryMappedFile CreateMMF( string fileName, FileAccess access, int size )
		{
			if( size < 0 )
			{
				throw new ArgumentException( "The size parameter should be a number greater than Zero." );
			} //if

			IntPtr memoryFileHandle = CreateFileMapping( 0xFFFFFFFF, IntPtr.Zero, (uint)access, 0, (uint)size, fileName );
			if( memoryFileHandle == IntPtr.Zero )
			{
				throw new SharedMemoryException( "Creating Shared Memory failed." );
			} //if

			return new MemoryMappedFile( memoryFileHandle );
		}

		internal static IntPtr ReadHandle( string fileName )
		{
			IntPtr mappedFileHandle = OpenFileMapping( (int)FileAccess.ReadWrite, false, fileName );
			if( mappedFileHandle == IntPtr.Zero )
			{
				throw new SharedMemoryException( "Opening the Shared Memory for Read failed." );
			} //if

			IntPtr mappedViewHandle = MapViewOfFile( mappedFileHandle, (uint)FILE_MAP_READ, 0, 0, 8 );
			if( mappedViewHandle == IntPtr.Zero )
			{
				throw new SharedMemoryException( "Creating a view of Shared Memory failed." );
			} //if

			IntPtr windowHandle = Marshal.ReadIntPtr( mappedViewHandle );
			if( windowHandle == IntPtr.Zero )
			{
				throw new ArgumentException( "Reading from the specified address in  Shared Memory failed." );
			} //if

			UnmapViewOfFile( mappedViewHandle );
			CloseHandle( (uint)mappedFileHandle );
			return windowHandle;
		}

		internal void WriteHandle( IntPtr windowHandle )
		{
			IntPtr mappedViewHandle = MapViewOfFile( mMemoryFileHandle, (uint)FILE_MAP_WRITE, 0, 0, 8 );
			if( mappedViewHandle == IntPtr.Zero )
			{
				throw new SharedMemoryException( "Creating a view of Shared Memory failed." );
			} //if

			Marshal.WriteIntPtr( mappedViewHandle, windowHandle );

			UnmapViewOfFile( mappedViewHandle );
			CloseHandle( (uint)mappedViewHandle );
		}

		#endregion Internal Members

		#region Other

		[FlagsAttribute]
		internal enum FileAccess : int
		{
			ReadOnly = 2,
			ReadWrite = 4
		}

		#endregion Other

	} //end of MemoryMappedFile class

	[Serializable]
	internal class SharedMemoryException : Exception
	{
		#region Constructors

		internal SharedMemoryException()
		{
		}

		internal SharedMemoryException( string message )
			: base( message )
		{
		}

		internal SharedMemoryException( string message, Exception inner )
			: base( message, inner )
		{
		}

		#endregion Constructors

	} //end of SharedMemoryException class
} //end of namespace