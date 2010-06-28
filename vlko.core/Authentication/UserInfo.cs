using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using vlko.core.Models;
using vlko.core.Models.Action;

namespace vlko.core.Authentication
{
	public class UserInfo
	{
		private readonly IUserAuthenticationService _authenticationService;
		private User _user;

		/// <summary>
		/// Initializes a new instance of the <see cref="UserInfo"/> class.
		/// </summary>
		/// <param name="name">The name.</param>
		public UserInfo(string name)
		{
			Name = name;
			_authenticationService = IoC.IoC.Resolve<IUserAuthenticationService>();
		}

		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>The name.</value>
		public string Name { get; private set; }

		/// <summary>
		/// Determines whether [is in role] [the specified role name].
		/// </summary>
		/// <param name="roleName">Name of the role.</param>
		/// <returns>
		/// 	<c>true</c> if [is in role] [the specified role name]; otherwise, <c>false</c>.
		/// </returns>
		public bool IsInRole(string roleName)
		{
			return _authenticationService.IsUserInRole(Name, roleName);
		}

		/// <summary>
		/// Determines whether this instance is admin.
		/// </summary>
		/// <returns>
		/// 	<c>true</c> if this instance is admin; otherwise, <c>false</c>.
		/// </returns>
		public bool IsAdmin()
		{
			return _authenticationService.IsUserInRole(Name, AccountValidation.AdminRole);
		}

		/// <summary>
		/// Gets the user.
		/// </summary>
		/// <value>The user.</value>
		public User User
		{ 
			get
			{
				if (_user == null || !string.IsNullOrEmpty(Name))
				{
					_user = IoC.IoC.Resolve<IUserAction>().GetByName(Name);
				}
				return _user;
			}
		}
	}
}
