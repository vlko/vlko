using System;
using System.Reflection;
using Castle.DynamicProxy;

namespace vlko.BlogModule.RavenDB.Repository.ReferenceProxy
{
	public class ReferenceInterceptor : IInterceptor
	{
		private object _target;

		private readonly string _id;
		private readonly Type _type;
		private const string GetProxyNameMethodConst = "GetOriginalTypeBeforeDynamicProxy";

		/// <summary>
		/// Initializes a new instance of the <see cref="ReferenceInterceptor"/> class.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <param name="type">The type.</param>
		public ReferenceInterceptor(string id, Type type)
		{
			_id = id;
			_type = type;

		}

		/// <summary>
		/// Intercepts the specified invocation.
		/// </summary>
		/// <param name="invocation">The invocation.</param>
		public void Intercept(IInvocation invocation)
		{
			// check if we want to get original type name
			if (invocation.Method.Name == GetProxyNameMethodConst)
			{
				invocation.ReturnValue = _type;
				return;
			}

			// if no target yet
			if (_target == null)
			{
				if (IsPropertyGet(invocation.Method))
				{
					// no need to go to remote store if we just want id
					if (GetPropertyName(invocation.Method).Equals("id", StringComparison.OrdinalIgnoreCase))
					{
						var returnType = invocation.MethodInvocationTarget.ReturnType;
						if (returnType == typeof(string))
						{
							invocation.ReturnValue = _id;
						}
						else if (returnType == typeof(Guid))
						{
							invocation.ReturnValue = new Guid(_id);
						}
						return;
					}
				}
				// create instance and use in future
				_target = CreateInstance();
			}

			// we have target and try to invoke on it
			invocation.ReturnValue = invocation.Method.Invoke(_target, invocation.Arguments);
		}

		/// <summary>
		/// Creates the instance.
		/// </summary>
		/// <returns>Instance from RavenDB store.</returns>
		private object CreateInstance()
		{
			var session = SessionFactory.Current;
			MethodInfo method = session.GetType().GetMethod("Load", new Type[] { typeof(Guid) });
			MethodInfo genericMethod = method.MakeGenericMethod(new Type[] { _type });
			return genericMethod.Invoke(session, new object[] { new Guid(_id) });
		}

		/// <summary>
		/// Determines whether [is property get] [the specified method].
		/// </summary>
		/// <param name="method">The method.</param>
		/// <returns>
		/// 	<c>true</c> if [is property get] [the specified method]; otherwise, <c>false</c>.
		/// </returns>
		private static bool IsPropertyGet(MethodInfo method)
		{
			return method.IsSpecialName && method.Name.StartsWith("get_");
		}

		/// <summary>
		/// Determines whether [is property set] [the specified method].
		/// </summary>
		/// <param name="method">The method.</param>
		/// <returns>
		/// 	<c>true</c> if [is property set] [the specified method]; otherwise, <c>false</c>.
		/// </returns>
		private static bool IsPropertySet(MethodInfo method)
		{
			return method.IsSpecialName && method.Name.StartsWith("set_");
		}

		/// <summary>
		/// Gets the name of the property.
		/// </summary>
		/// <param name="method">The method.</param>
		/// <returns>Property name.</returns>
		private static string GetPropertyName(MethodInfo method)
		{
			return method.Name.Substring(4);
		}
	}
}