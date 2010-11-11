using vlko.core.Action;
using vlko.core.Roots;

namespace vlko.core.Authentication
{
	public class UserInfo
	{
		private readonly IUserAuthenticationService _authenticationService;
		private IUser _user;

		/// <summary>
		/// Initializes a new instance of the <see cref="UserInfo"/> class.
		/// </summary>
		/// <param name="name">The name.</param>
		public UserInfo(string name)
		{
			Name = name;
			_authenticationService = InversionOfControl.IoC.Resolve<IUserAuthenticationService>();
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
			return _authenticationService.IsUserInRole(Name, Settings.AdminRole);
		}

		/// <summary>
		/// Gets the user.
		/// </summary>
		/// <value>The user.</value>
		public IUser User
		{ 
			get
			{
				if (_user == null || !string.IsNullOrEmpty(Name))
				{
					_user = InversionOfControl.IoC.Resolve<IUserAction>().GetByName(Name);
				}
				return _user;
			}
		}
	}
}
