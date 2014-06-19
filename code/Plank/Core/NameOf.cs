using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace LazyE9.Plank.Core
{
	/// <summary>
	/// Used to return the name of a property in its string format.
	/// This should be used anywhere you bind to a property
	/// </summary>
	/// <typeparam name="TObject">Name of Class</typeparam>
	/// <example>
	/// string propName = CUNameOf<SampleClass></SampleClass>.Property(p => p.Name);
	/// </example>
	public static class NameOf<TObject>
	{
		#region NameOf Members

		/// <summary>
		/// Returns the string representation of the property specified
		/// </summary>
		/// <typeparam name="TProp">Class name (specified </typeparam>
		/// <param name="expression">p => p.[PropertyName]</param>
		/// <returns>The string representation of the property</returns>
		public static string Path<TProp>( Expression<Func<TObject, TProp>> expression )
		{
			return NameOf.GetPath( expression );
		}

		/// <summary>
		/// Returns the string representation of the property specified
		/// </summary>
		/// <typeparam name="TProp">Class name (specified </typeparam>
		/// <param name="expression">p => p.[PropertyName]</param>
		/// <returns>The string representation of the property</returns>
		public static string Property<TProp>( Expression<Func<TObject, TProp>> expression )
		{
			return NameOf.GetName( expression );
		}

		/// <summary>
		/// Returns the string representation of the property specified
		/// </summary>
		/// <typeparam name="TProp">Class name (specified </typeparam>
		/// <param name="expression">() => this.Property</param>
		/// <returns>The string representation of the property</returns>
		public static string Property<TProp>( Expression<Func<TProp>> expression )
		{
			return NameOf.GetName( expression );
		}

		#endregion NameOf Members

	}
	/// <summary>
	/// Additional naming utilities
	/// </summary>
	public static class NameOf
	{
		#region NameOf Members

		///<summary>
		///Gets the name of a <see cref="LambdaExpression"/> by looking at the expression's
		///Body property.
		///</summary>
		///<param name="expression">The expression to get a name from</param>
		public static string GetName( LambdaExpression expression )
		{
			if( expression == null )
			{
				throw new ArgumentNullException( "expression" );
			}

			return GetStringRepresentation( expression.Body, SupportedGetNameTypes );
		}

		///<summary>
		/// Gets the name of a <see cref="ParameterExpression"/> by looking at the
		/// expression's Name property.
		///</summary>
		///<param name="expression">The expression to get a name from</param>
		public static string GetName( ParameterExpression expression )
		{
			if( expression == null )
			{
				throw new ArgumentNullException( "expression" );
			}

			return expression.Name;
		}

		///<summary>
		/// Gets the name of a <see cref="MemberExpression"/> by looking at the
		/// expression's Member property.
		///</summary>
		///<param name="expression">The expression to get a name from</param>
		public static string GetName( MemberExpression expression )
		{
			if( expression == null )
			{
				throw new ArgumentNullException( "expression" );
			}

			return expression.Member.Name;
		}

		///<summary>
		///Gets the name of a <see cref="UnaryExpression"/> by looking at the expression's
		///Operand property.
		///</summary>
		///<param name="expression">The expression to get a name from</param>
		public static string GetName( UnaryExpression expression )
		{
			if( expression == null )
			{
				throw new ArgumentNullException( "expression" );
			}

			return GetStringRepresentation( expression.Operand, SupportedGetNameTypes );
		}

		///<summary>
		///Gets a name from a lambda expression. This method allows you get the name of
		///something that returns a value.
		///</summary>
		///<param name="expression">The lambda expression that produces a value</param>
		///<typeparam name="TObject">The type of value to be produced</typeparam>
		///<returns>The name of the property or field that produces the value</returns>
		public static string GetName<TObject>( Expression<Func<TObject>> expression )
		{
			return GetStringRepresentation( expression, SupportedGetNameTypes );
		}

		///<summary>
		///Gets the name of a <see cref="UnaryExpression"/> by looking at the expression's
		///Operand property.
		///</summary>
		///<param name="expression">The expression to get a name from</param>
		public static string GetPath( UnaryExpression expression )
		{
			if( expression == null )
			{
				throw new ArgumentNullException( "expression" );
			}

			return GetStringRepresentation( expression.Operand, SupportedGetPathTypes );
		}

		///<summary>
		///Gets the name of a <see cref="LambdaExpression"/> by looking at the expression's
		///Body property.
		///</summary>
		///<param name="expression">The expression to get a name from</param>
		public static string GetPath( LambdaExpression expression )
		{
			if( expression == null )
			{
				throw new ArgumentNullException( "expression" );
			}

			return GetStringRepresentation( expression.Body, SupportedGetPathTypes );
		}

		///<summary>
		/// Gets the name of a <see cref="MemberExpression"/> by looking at the
		/// expression's Member property.
		///</summary>
		///<param name="expression">The expression to get a name from</param>
		public static string GetPath( MemberExpression expression )
		{
			if( expression == null )
			{
				throw new ArgumentNullException( "expression" );
			}

			var pathStack = new Stack<string>();

			Expression currentExpression = expression;
			while( currentExpression != null )
			{

				if( currentExpression is MethodCallExpression )
				{
					// this is usaully an indexer get property. We don't care
					// about the indexer, we just want the method object which
					// will end up being the property. Nothing to push on the
					// stack here just get the next thing and keep looping
					var methodCallExpression = ((MethodCallExpression)currentExpression);

					if( methodCallExpression.Method.Name == "_" )
					{
						currentExpression = methodCallExpression.Arguments[0];
					}
					else
					{
						currentExpression = methodCallExpression.Object;
					}
				}
				else if( currentExpression is BinaryExpression )
				{
					// this is usaully an indexer on an array. We don't care
					// about the indexer, we just want the left side which
					// will end up being the property. Nothing to push on the
					// stack here just get the next thing and keep looping
					var binaryExpression = (BinaryExpression)currentExpression;
					currentExpression = binaryExpression.Left;
				}
				else if( currentExpression is MemberExpression )
				{
					var memberExpression = ((MemberExpression)currentExpression);
					pathStack.Push( memberExpression.Member.Name );
					currentExpression = memberExpression.Expression;
				}
				else
				{
					break;
				}
			}

			return string.Join( ".", pathStack.ToArray() );
		}

		///<summary>
		///Gets a name from a lambda expression. This method allows you get the name of
		///something that returns a value.
		///</summary>
		///<param name="expression">The lambda expression that produces a value</param>
		///<typeparam name="TObject">The type of value to be produced</typeparam>
		///<returns>The name of the property or field that produces the value</returns>
		public static string GetPath<TObject>( Expression<Func<TObject>> expression )
		{
			return GetStringRepresentation( expression, SupportedGetPathTypes );
		}

		///<summary>
		///Gets the name of a <see cref="Expression"/> by using the other GetName methods
		///in this class if possible.
		///</summary>
		///<param name="expression">The expression to get a name from</param>
		///<param name="supportedTypes">Types allowed by this function</param>
		///<exception cref="ArgumentOutOfRangeException">
		///Thrown if there is handler for the type of expression passed in.
		///</exception>
		public static string GetStringRepresentation( Expression expression,
			IDictionary<ExpressionType, Func<Expression, string>> supportedTypes )
		{
			if( expression == null )
			{
				throw new ArgumentNullException( "expression" );
			}

			Func<Expression, string> stringProducer;
			ExpressionType expressionType = expression.NodeType;
			if( supportedTypes.TryGetValue( expressionType, out stringProducer ) )
			{
				return stringProducer( expression );
			}

			throw new ArgumentOutOfRangeException(
				string.Format( "Cannot get the path of a {0}", expressionType ) );
		}

		#endregion NameOf Members

		#region Fields

		public static readonly IDictionary<ExpressionType, Func<Expression, string>> SupportedGetNameTypes =
			new Dictionary<ExpressionType, Func<Expression, string>>
				{
					{ExpressionType.MemberAccess, expression => GetName((MemberExpression) expression)},
					{ExpressionType.Convert, expression => GetName((UnaryExpression) expression)},
					{ExpressionType.TypeAs, expression => GetName((UnaryExpression) expression)},
					{ExpressionType.Lambda, expression => GetName((LambdaExpression) expression)},
					{ExpressionType.Parameter, expression => GetName((ParameterExpression) expression)}
				};
		public static readonly IDictionary<ExpressionType, Func<Expression, string>> SupportedGetPathTypes =
			new Dictionary<ExpressionType, Func<Expression, string>>
				{
					{ExpressionType.MemberAccess, expression => GetPath((MemberExpression) expression)},
					{ExpressionType.Convert, expression => GetPath((UnaryExpression) expression)},
					{ExpressionType.Lambda, expression => GetPath((LambdaExpression) expression)}
				};

		#endregion Fields

	}
}
