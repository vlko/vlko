﻿using System;
using System.Linq;
using System.Security.Cryptography;
using vlko.core.Commands;
using vlko.core.RavenDB.Indexes;
using vlko.core.RavenDB.Repository;
using vlko.core.Repository;
using vlko.core.Roots;

namespace vlko.core.RavenDB.Commands
{
	public class UserAuthentication : CommandGroup<User>, IUserAuthentication
	{
		/// <summary>
		/// Generates the verify token.
		/// </summary>
		/// <returns>Verify token.</returns>
		private static string GenerateVerifyToken()
		{
			return string.Format("{0}-63j2{1}", Guid.NewGuid(), DateTime.Now.ToBinary());
		}

		/// <summary>
		/// Hashes the password.
		/// </summary>
		/// <param name="password">The password.</param>
		/// <returns></returns>
		internal static string HashPassword(string password)
		{
			Byte[] data = System.Text.Encoding.UTF8.GetBytes("af8q890^&XE#%^w4jhnzcvnas;kl" + password + "asdf hioafdfhsaasfaufh9wf 7q34    3r q89X%$WE%$%");
			Byte[] hash = new SHA256CryptoServiceProvider().ComputeHash(data);
			return Convert.ToBase64String(hash);
		}

		/// <summary>
		/// Creates the user.
		/// </summary>
		/// <param name="username">The username.</param>
		/// <param name="password">The password.</param>
		/// <param name="email">The email.</param>
		/// <param name="verifyToken">The verify token used for mailing user to verify.</param>
		/// <returns>Create user status.</returns>
		public CreateUserStatus CreateUser(string username, string password, string email, out string verifyToken)
		{
			verifyToken = null;

			// check for unique username
			if (SessionFactory<User>.IndexQuery<UsersByNameSortIndex>().Customize(x => x.WaitForNonStaleResults()).FirstOrDefault(user => user.Name == username) != null)
			{
				return CreateUserStatus.DuplicateUserName;
			}

			// check for unique email
			if (SessionFactory<User>.IndexQuery<UsersByNameSortIndex>().Customize(x => x.WaitForNonStaleResults()).FirstOrDefault(user => user.Email == email) != null)
			{
				return CreateUserStatus.DuplicateEmail;
			}

			verifyToken = GenerateVerifyToken();

			User newUser = new User()
							   {
								   Id = Guid.NewGuid(),
								   Name = username,
								   Email = email,
								   Password = HashPassword(password),
								   LastSeen = DateTime.Now,
								   Verified = false,
								   VerifyToken = verifyToken
							   };
			using (var tran = RepositoryFactory.StartTransaction())
			{
				SessionFactory<User>.Store(newUser);
				tran.Commit();
			}
			return CreateUserStatus.Success;
		}


		/// <summary>
		/// Validates the user.
		/// </summary>
		/// <param name="username">The username.</param>
		/// <param name="password">The password.</param>
		/// <returns>True if user validated; otherwise false.</returns>
		public ValidateUserStatus ValidateUser(string username, string password)
		{
			var userToValidate = SessionFactory<User>.IndexQuery<UsersByNameSortIndex>().FirstOrDefault(user => user.Name == username);

			if (userToValidate == null)
			{
				return ValidateUserStatus.NotExists;
			}

			if (!userToValidate.Verified)
			{
				return ValidateUserStatus.NotVerified;
			}

			if (userToValidate.Password != HashPassword(password))
			{
				return ValidateUserStatus.InvalidPassword;
			}

			userToValidate.LastSeen = DateTime.Now;
			using (var tran = RepositoryFactory.StartTransaction())
			{
				SessionFactory<User>.Store(userToValidate);
				tran.Commit();
			}
			return ValidateUserStatus.Success;
		}

		/// <summary>
		/// Changes the password.
		/// </summary>
		/// <param name="username">The username.</param>
		/// <param name="oldPassword">The old password.</param>
		/// <param name="newPassword">The new password.</param>
		/// <returns>
		/// True if old password valid and new was set. Otherwise false.
		/// </returns>
		public bool ChangePassword(string username, string oldPassword, string newPassword)
		{
			var userToChangePassword = SessionFactory<User>.IndexQuery<UsersByNameSortIndex>().FirstOrDefault(user => user.Name == username);
			if ((userToChangePassword != null)
				&& (userToChangePassword.Password == HashPassword(oldPassword)))
			{
				userToChangePassword.Password = HashPassword(newPassword);
				userToChangePassword.LastSeen = DateTime.Now;
				using (var tran = RepositoryFactory.StartTransaction())
				{
					SessionFactory<User>.Store(userToChangePassword);
					tran.Commit();
				}
				return true;
			}
			return false;
		}

		/// <summary>
		/// Verifies the token to user.
		/// </summary>
		/// <param name="verifyToken">The verify token.</param>
		/// <returns>
		/// User name if token valid; otherwise null;
		/// </returns>
		public string VerifyTokenToUser(string verifyToken)
		{
			var userWithToken = SessionFactory<User>.IndexQuery<UsersByNameSortIndex>().FirstOrDefault(user => user.VerifyToken == verifyToken);
			if (userWithToken != null)
			{
				return userWithToken.Name;
			}
			return null;
		}

		/// <summary>
		/// Confirms the registration.
		/// </summary>
		/// <param name="verifyToken">The verify token.</param>
		/// <returns>
		/// True if registration process succeed otherwise false;
		/// </returns>
		public bool ConfirmRegistration(string verifyToken)
		{
			var userWithToken = SessionFactory<User>.IndexQuery<UsersByNameSortIndex>().FirstOrDefault(user => user.VerifyToken == verifyToken);
			if (userWithToken != null && !userWithToken.Verified)
			{
				userWithToken.VerifyToken = null;
				userWithToken.Verified = true;
				userWithToken.LastSeen = DateTime.Now;
				using (var tran = RepositoryFactory.StartTransaction())
				{
					SessionFactory<User>.Store(userWithToken);
					tran.Commit();
				}
				return true;
			}
			return false;
		}

		/// <summary>
		/// Gets the reset password token.
		/// </summary>
		/// <param name="email">The email.</param>
		/// <param name="verifyToken">The verify token.</param>
		/// <returns>Reset user password status.</returns>
		public ResetUserPasswordStatus GetResetPasswordToken(string email, out string verifyToken)
		{
			verifyToken = null;

			var userToReset = SessionFactory<User>.IndexQuery<UsersByNameSortIndex>().FirstOrDefault(user => user.Email == email);

			if (userToReset != null)
			{
				if (!userToReset.Verified)
				{
					return ResetUserPasswordStatus.UserNotVerified;
				}

				if (userToReset.VerifyToken != null
					&& userToReset.LastSeen < DateTime.Now.AddHours(1) )
				{
					return ResetUserPasswordStatus.WaitOneHour;
				}

				verifyToken = GenerateVerifyToken();
				userToReset.VerifyToken = verifyToken;
				userToReset.LastSeen = DateTime.Now;

				using (var tran = RepositoryFactory.StartTransaction())
				{
					SessionFactory<User>.Store(userToReset);
					tran.Commit();
				}
				return ResetUserPasswordStatus.Success;
			}
			return ResetUserPasswordStatus.EmailNotExist;
		}

		/// <summary>
		/// Resets the password.
		/// </summary>
		/// <param name="username">The username.</param>
		/// <param name="verifyToken">The verify token.</param>
		/// <param name="newPassword">The new password.</param>
		/// <returns>True if password changed.</returns>
		public bool ResetPassword(string username, string verifyToken, string newPassword)
		{
			var userToReset = SessionFactory<User>.IndexQuery<UsersByNameSortIndex>().FirstOrDefault(user => user.Name == username);
			if (userToReset != null
				&& userToReset.Verified
				&& userToReset.VerifyToken != null
				&& userToReset.VerifyToken == verifyToken)
			{
				userToReset.VerifyToken = null;
				userToReset.Password = HashPassword(newPassword);
				userToReset.LastSeen = DateTime.Now;

				using (var tran = RepositoryFactory.StartTransaction())
				{
					SessionFactory<User>.Store(userToReset);
					tran.Commit();
				}
				return true;
			}
			return false;
		}
	}
}