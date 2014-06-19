using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LazyE9.Plank.Core
{
	public class ServiceLocator
	{
		#region ServiceLocator Members

		private static readonly Lazy<ServiceLocator> mDefault = new Lazy<ServiceLocator>( () => new ServiceLocator() );
		public static ServiceLocator Default
		{
			get
			{
				return mDefault.Value;
			}
		}

		public void RegisterInstance<TService>( TService service )
			 where TService : class
		{
			if( mInstanceCache.ContainsKey( typeof( TService ) ) )
			{
				mInstanceCache[typeof( TService )] = service;
			}
			else
			{
				mInstanceCache.Add( typeof( TService ), service );
			}
		}

		public void RegisterType<TService>( Func<TService> serviceCreator )
			 where TService : class
		{
			if( mCreatorCache.ContainsKey( typeof( TService ) ) )
			{
				mCreatorCache[typeof( TService )] = serviceCreator;
			}
			else
			{
				mCreatorCache.Add( typeof( TService ), serviceCreator );
			}
		}

		public TService Resolve<TService>() where TService : class
		{
			object service;
			if( !mInstanceCache.TryGetValue( typeof( TService ), out service ) )
			{
				Delegate serviceCreator;
				if( mCreatorCache.TryGetValue( typeof( TService ), out serviceCreator ) )
				{
					service = ((Func<TService>)serviceCreator)();
				}
			}

			return service as TService;
		}

		public async Task<TService> ResolveAsync<TService>() where TService : class
		{
			return await Task.Run( () => Resolve<TService>() );
		}

		#endregion ServiceLocator Members

		#region Fields

		private readonly Dictionary<Type, Delegate> mCreatorCache = new Dictionary<Type, Delegate>();
		private readonly Dictionary<Type, object> mInstanceCache = new Dictionary<Type, object>();

		#endregion Fields

	}
}
