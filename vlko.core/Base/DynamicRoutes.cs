using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using vlko.core.Tools;

namespace vlko.core.Base
{
	public class DynamicRoutes : DynamicObject
	{
		readonly UrlHelper _helper;

		/// <summary>
		/// Initializes a new instance of the <see cref="DynamicRoutes"/> class.
		/// </summary>
		/// <param name="helper">The helper.</param>
		public DynamicRoutes(UrlHelper helper)
		{
			_helper = helper;
		}

		/// <summary>
		/// Provides the implementation for operations that get member values. Classes derived from the <see cref="T:System.Dynamic.DynamicObject"/> class can override this method to specify dynamic behavior for operations such as getting a value for a property.
		/// </summary>
		/// <param name="binder">Provides information about the object that called the dynamic operation. The binder.Name property provides the name of the member on which the dynamic operation is performed. For example, for the Console.WriteLine(sampleObject.SampleProperty) statement, where sampleObject is an instance of the class derived from the <see cref="T:System.Dynamic.DynamicObject"/> class, binder.Name returns "SampleProperty". The binder.IgnoreCase property specifies whether the member name is case-sensitive.</param>
		/// <param name="result">The result of the get operation. For example, if the method is called for a property, you can assign the property value to <paramref name="result"/>.</param>
		/// <returns>
		/// true if the operation is successful; otherwise, false. If this method returns false, the run-time binder of the language determines the behavior. (In most cases, a run-time exception is thrown.)
		/// </returns>
		public override bool TryGetMember(GetMemberBinder binder, out object result)
		{
			result = GetUrl(binder.Name, new object[0], null);
			return true;
		}

		/// <summary>
		/// Provides the implementation for operations that invoke a member. Classes derived from the <see cref="T:System.Dynamic.DynamicObject"/> class can override this method to specify dynamic behavior for operations such as calling a method.
		/// </summary>
		/// <param name="binder">Provides information about the dynamic operation. The binder.Name property provides the name of the member on which the dynamic operation is performed. For example, for the statement sampleObject.SampleMethod(100), where sampleObject is an instance of the class derived from the <see cref="T:System.Dynamic.DynamicObject"/> class, binder.Name returns "SampleMethod". The binder.IgnoreCase property specifies whether the member name is case-sensitive.</param>
		/// <param name="args">The arguments that are passed to the object member during the invoke operation. For example, for the statement sampleObject.SampleMethod(100), where sampleObject is derived from the <see cref="T:System.Dynamic.DynamicObject"/> class, <paramref name="args[0]"/> is equal to 100.</param>
		/// <param name="result">The result of the member invocation.</param>
		/// <returns>
		/// true if the operation is successful; otherwise, false. If this method returns false, the run-time binder of the language determines the behavior. (In most cases, a language-specific run-time exception is thrown.)
		/// </returns>
		public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
		{
			var info = binder.CallInfo;
			// accept single parameter as id or object or only named arguments as route values
			if (args.Length != 1 && info.ArgumentNames.Count != args.Length)
			{
				throw new InvalidOperationException("Please use named arguments to specify route values or single parameter as id or anonymous object to specify route values");
			}

			result = GetUrl(binder.Name, args, info.ArgumentNames);

			return true;

		}

		/// <summary>
		/// Resolves method name with arguments to url.
		/// </summary>
		/// <param name="methodName">Name of the method.</param>
		/// <param name="args">The args.</param>
		/// <param name="argumentNames">The argument names.</param>
		/// <returns>
		/// Url.
		/// </returns>
		private object GetUrl(string methodName, object[] args, ReadOnlyCollection<string> argumentNames)
		{
			DynamicRoutesResult result = new DynamicRoutesResult();


			// split it for name of route and path/url
			var stems = (methodName.Contains('_'))
			            	? methodName.Split('_')
			            	: StringTool.SplitUpperCase(methodName);


			//look up the route by name
			if (args.Length > 0)
			{
				// if single argument then id or anonymous object route values
				if (args.Length == 1 && (argumentNames == null || argumentNames.Count == 0))
				{
					//pass in the single argument...
					if (args[0] is string || args[0] is Guid|| args[0].GetType().IsPrimitive)
					{
						result.Add("id", args[0].ToString());
					}
					else
					{
						// load anonymous object
						foreach (PropertyDescriptor propertyDescriptor in TypeDescriptor.GetProperties(args[0]))
						{
							object obj = propertyDescriptor.GetValue(args[0]);
							result.Add(propertyDescriptor.Name, obj);
						}
					}
				}
				// if more parameters, then create route values collection based on name parameters
				else
				{
					for (int i = 0; i < args.Length; i++)
					{
						var name = argumentNames[i].ToLower();
						result.Add(name, args[i]);
					}
				}
			}

			var action = GetPartialName(stems, null, new[] {"of", "url"});
			var controller = GetPartialName(stems, "of", new[] { "from", "url" });
			var area = GetPartialName(stems, "from", new[] { "url" });

			if (controller != null)
			{
				result.Add("action", action);
				if (!controller.Equals("this", StringComparison.InvariantCultureIgnoreCase))
				{
					result.Add("controller", controller);
				}
				if (area == null || !area.Equals("this", StringComparison.InvariantCultureIgnoreCase))
				{
					result.Add("area", area ?? string.Empty);
				}
			}
			else
			{
				result.RouteName = action;
			}

			result.Url = _helper.RouteUrl(result.RouteName, result.RouteValues);
			//url or path?
			if (stems.Last().Equals("url", StringComparison.InvariantCultureIgnoreCase))
			{
				result.Url = Root(false) + result.Url;
			}

			return result;
		}

		private string GetPartialName(string[] stems, string startWord, string[] stopWords)
		{
			string result = null;
			bool started = startWord == null;
			for (int i = 0; i < stems.Length; i++)
			{
				if (started)
				{
					// if stop word we have full result
					if (stopWords.Contains(stems[i].ToLowerInvariant()))
					{
						break;
					}
					result += stems[i];
				}
				else
				{
					started = stems[i].Equals(startWord, StringComparison.InvariantCultureIgnoreCase);
				}
			}
			return result;
		}

		/// <summary>
		/// Get root of application.
		/// </summary>
		/// <param name="includeAppPath">if set to <c>true</c> [include app path].</param>
		/// <returns>Root of application.</returns>
		public string Root(bool includeAppPath = true)
		{
			var context = _helper.RequestContext.HttpContext;

			var port = context.Request.ServerVariables["SERVER_PORT"];
			if (port == null || port == "80" || port == "443")
				port = "";
			else
				port = ":" + port;

			var protocol = context.Request.ServerVariables["SERVER_PORT_SECURE"];
			if (protocol == null || protocol == "0")
				protocol = "http://";
			else
				protocol = "https://";

			var appPath = "";
			if (includeAppPath)
			{
				appPath = context.Request.ApplicationPath;
				if (appPath == "/")
					appPath = "";
			}

			var sOut = protocol + context.Request.ServerVariables["SERVER_NAME"] + port + appPath;
			return sOut;
		}
	}
}
